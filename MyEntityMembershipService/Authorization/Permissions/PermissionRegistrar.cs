using Common.RabbitMq.Premission;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MyEntityMembershipService.Authorization.AuthorizationService;
using MyEntityMembershipService.Authorization.Permissions.Dto;
using System.Reflection;

namespace MyEntityMembershipService.Authorization.Permissions
{
    public static class PermissionRegistrar
    {
        public static async Task RegisterPermissionAsync(this WebApplication app, Assembly controllerAssembly)
        {
            using var scope = app.Services.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
            var publishEndPoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var serviceName = controllerAssembly.GetName().Name ?? "Unknown";

            var controllers = controllerAssembly.GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract);

            var ServicePermission = new ServicePremmissionRegiterDto() { ServiceName = serviceName };
            foreach (var controller in controllers)
            {
                var controllerName = controller.Name.Replace("Controller", "");

                var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                foreach (var action in actions)
                {
                    var actionName = action.Name;
                    var httpMethod = GetHttpMethod(action) ?? "UNKNOWN";

                    ServicePermission.Permissions.Add(new PremissionRegisterDto()
                    {
                        Entity = controllerName,
                        Action = actionName,
                        Service = serviceName,
                        Method = httpMethod
                    });

                }
            }

            if (ServicePermission.Permissions.Any())
            {
                // publish to RabbitMQ
                await publishEndPoint.Publish(ServicePermission);
            }

        }
        private static string? GetHttpMethod(MethodInfo method)
        {

            var httpAttr = method.GetCustomAttributes<HttpMethodAttribute>().FirstOrDefault();
            return httpAttr?.HttpMethods.FirstOrDefault();
        }
    }
}
        
    
