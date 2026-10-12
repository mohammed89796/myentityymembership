using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace MyEntityMembershipService.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthetication(this WebApplicationBuilder builder, IConfiguration configuration)
        {


            var identityUrl = configuration["ApiSettings:IdentityUrl"];
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.Authority = identityUrl;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsPrincipal = context.Principal;
                        var httpContext = context.HttpContext;
                        var tokenHandler = new JwtSecurityTokenHandler();

                        // Retrieve the raw token
                        var rawToken = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                        if (tokenHandler.CanReadToken(rawToken))
                        {
                            var token = tokenHandler.ReadJwtToken(rawToken);

                            Console.WriteLine("==== Token Details ====");
                            Console.WriteLine($"Issuer: {token.Issuer}");
                            Console.WriteLine($"Valid From: {token.ValidFrom}");
                            Console.WriteLine($"Valid To: {token.ValidTo}");
                            Console.WriteLine($"Algorithm: {token.Header.Alg}");

                            Console.WriteLine("\n==== Token Claims ====");
                            foreach (var claim in token.Claims)
                            {
                                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid JWT format.");
                        }

                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new { message = "This user is not authorized" });
                        return context.Response.WriteAsync(result);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new { message = "Access is forbidden for this user" });
                        return context.Response.WriteAsync(result);
                    }
                };
            });
            builder.Services.AddAuthorization(Options =>
            {
                Options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "x_inity_mobile_app_react"));
            });


            return builder;
        }
    }
}
