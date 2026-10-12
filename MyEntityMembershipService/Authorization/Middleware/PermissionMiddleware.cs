
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using MyEntityMembershipService.Authorization.Permissions.Dto;
using Serilog;
using System.Reflection;
using System.Security.Claims;
using IAuthorizationService = MyEntityMembershipService.Authorization.AuthorizationService.IAuthorizationService;

namespace MyEntityMembershipService.Authorization.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
 

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthorizationService permissionChecker)
        {
            var endpoint = context.GetEndpoint();


            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            // Skip anonymous endpoints
            if (endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                await _next(context);
                return;
            }
            var authorizeAttributes = endpoint.Metadata.GetOrderedMetadata<AuthorizeAttribute>();

            if (authorizeAttributes == null || !authorizeAttributes.Any())
            {
                await _next(context);
                return;
            }

            var user = context.User;

            var isClientCredentials = user?.Identity?.IsAuthenticated == true &&
                          user.HasClaim(c => c.Type == "client_id") && // has client_id
                          !user.HasClaim(c => c.Type == ClaimTypes.NameIdentifier); // doesn't have user id


            if (isClientCredentials)
            {
                var clientId = user?.FindFirst("client_id")?.Value;
                Log.Information("Client credentials authenticated for client_id: {ClientId}", clientId);


                await _next(context);
                return;
            }
            if (user?.IsInRole("SuperAdmin") ?? false)
            {
                await _next(context);
                return;
            }
            //if(user?.IsInRole("Admin") ?? false)
            //{
            //    var userTenantId = user.FindFirst("tenant_id")?.Value;
            //    //var routeTenantId = context.Request.RouteValues["tenantId"]?.ToString(); 
            //    // var routeTenantId = context.Request.Query["tenantId"].ToString();
            //    // var routeTenantId = context.Request.Headers["X-Tenant-Id"].ToString();
            //    if (string.IsNullOrWhiteSpace(userTenantId) || string.IsNullOrWhiteSpace(routeTenantId))
            //    {
            //        context.Response.StatusCode = StatusCodes.Status400BadRequest;
            //        await context.Response.WriteAsync("Missing tenant information.");
            //        return;
            //    }

            //    if (!string.Equals(userTenantId, routeTenantId, StringComparison.OrdinalIgnoreCase))
            //    {
            //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //        await context.Response.WriteAsync("Forbidden: Tenant mismatch.");
            //        return;
            //    }

            //    await _next(context);
            //    return;


            //}
            var requiredRoles = authorizeAttributes
                                .Where(attr => !string.IsNullOrWhiteSpace(attr.Roles))
                                .SelectMany(attr => attr.Roles?.Split(',') ?? Array.Empty<string>())
                                .Select(role => role.Trim())
                                .Distinct()
                                .ToList();



            if (requiredRoles.Any())
            {
                var userRoles = user?.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList() ?? new List<string>();

                var hasRequiredRole = userRoles.Intersect(requiredRoles).Any();

                if (hasRequiredRole)
                {
                    await _next(context);
                    return;
                }
            }

            var policyAttributes = endpoint.Metadata.GetOrderedMetadata<CustomPolicyAttribute>();


            var policies = policyAttributes
                          .SelectMany(att => att.PolicyName)
                          .Distinct()
                          .ToList();






            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            //if (policies.Any())
            //{
            //    var hasRequiredPolicy = await permissionChecker.EvaluateAsync(userId, policies);
            //    if (!hasRequiredPolicy)
            //    {
            //        Log.Warning("User {UserId} was denied access to {Policies}", userId, policies);
            //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //        await context.Response.WriteAsync("Forbidden: Insufficient polices.");
            //        return;
            //    }


            //}




            // Extract controller/action
            var descriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (descriptor == null)
            {
                await _next(context);
                return;
            }

            var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
            var controller = descriptor.ControllerName;
            var action = descriptor.ActionName;
            var method = context.Request.Method;

            var permissionCode = $"{serviceName}.{controller}.{action}.{method}";

            var authRequest = new AuthorizationRequestDto
            {
                UserId = userId,
                PermissionCode = permissionCode,
                PoliciesName = policies!
            };


            var hasPermission = await permissionChecker.IsAuthAsync(authRequest);

            if (!hasPermission)
            {
                Log.Warning("User {UserId} was denied access to {PermissionCode}", userId, permissionCode);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Insufficient permission.");
                return;
            }

            await _next(context);
        }
    }
}
