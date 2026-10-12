using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Common.Logging;

using Elastic.Apm.AspNetCore.DiagnosticListener;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.EntityFrameworkCore;
using Hangfire;
using Hangfire.SqlServer;
using MassTransit;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MyEntityMembershipService.Authorization.Middleware;
using MyEntityMembershipService.Authorization.Permissions;
using MyEntityMembershipService.Consumers;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.DTOs.Membership.EmailsDtos;
using MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.SwaggerConfig;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace MyEntityMembershipService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            // Initialize Serilog Logger
            //Log.Logger = new LoggerConfiguration()
            //        .MinimumLevel.Debug()
            //        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //        .MinimumLevel.Override("Elasticsearch.Net", LogEventLevel.Information) // Set a higher level here
            //        .Enrich.FromLogContext()
            //        .WriteTo.Console()
            //        .CreateLogger();

            //Log.Information("Starting web host");

            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();

            /// <summary>
            /// Configures Serilog for logging. Serilog is a diagnostic logging library for .NET applications.
            /// </summary>
            builder.Host.UseSerilog(SeriLogger.Configure);
            //builder.Services.AddElasticApmForAspNetCore(new HttpDiagnosticsSubscriber(), new EfCoreDiagnosticsSubscriber(), new AspNetCoreDiagnosticSubscriber());

            builder.AddAppAuthetication(builder.Configuration);

            IConfigurationBuilder Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);


            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

            // Add Hangfire server
            builder.Services.AddHangfireServer();

            var configuration = Configuration.Build();


            var server = Environment.GetEnvironmentVariable("Db_Server") ?? builder.Configuration.GetConnectionString("DatabaseConnection:Server");
            var database = Environment.GetEnvironmentVariable("Db_Database") ?? builder.Configuration.GetConnectionString("DatabaseConnection:Database");
            var userId = Environment.GetEnvironmentVariable("Db_Uid") ?? builder.Configuration.GetConnectionString("DatabaseConnection:UserId");
            var password = Environment.GetEnvironmentVariable("Db_Pwd") ?? builder.Configuration.GetConnectionString("DatabaseConnection:Password");
            var port = Environment.GetEnvironmentVariable("Db_Port") ?? builder.Configuration.GetConnectionString("DatabaseConnection:Port");
            var connectionTimeout = Environment.GetEnvironmentVariable("Db_ConnectionTimeOut") ?? builder.Configuration.GetConnectionString("ConnectionTimeout");
            var allowUserVariables = Environment.GetEnvironmentVariable("Db_AllowUserVariables") ?? builder.Configuration.GetConnectionString("AllowUserVariables");
            var connectionString = "";
            builder.Services.AddDbContext<EntityMembershipDbContext>(options =>
            {
                //var server = ;
                //var database = ;
                //var userId = ;
                //var password = ;
                //var port = ;


                connectionString = $"server={server};database={database};Uid={userId};Password={password};Port={port}";

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });


            var serverHangFire = Environment.GetEnvironmentVariable("Db_Server_hangfire");
            var databaseHangfire = Environment.GetEnvironmentVariable("Db_DataBase_hangfire");
            var userIdHangFire = Environment.GetEnvironmentVariable("Db_Uid_hangfire");
            var passwordHangFire = Environment.GetEnvironmentVariable("Db_Pwd_hangfire");
            var portHangFire = Environment.GetEnvironmentVariable("Db_Port_hangfire");
            var connectionStringHangFire = //"Server=sts.x-inity.com,7900;Database=hangfireDbserver;User Id=sa;Password=x-inity@Master2o23!@#;MultipleActiveResultSets=true; TrustServerCertificate=True;Connection Timeout=90";
            //"Server=sts.x-inity.com,7900;Database=hangfireDbserver;UId=sa;password=Trusted_Connection=True;MultipleActiveResultSets=true";
            $"server={serverHangFire},{portHangFire};database={databaseHangfire};Uid={userIdHangFire};Password={passwordHangFire};TrustServerCertificate=true";

            Log.Error("HangFire Connection String: {ConnectionString}", connectionStringHangFire);
            // Corrected Hangfire configuration
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings().UseSqlServerStorage(connectionStringHangFire, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(1),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = false, // Set to true for better performance if safe
                    PrepareSchemaIfNecessary = true,
                    DashboardJobListLimit = 50000,
                    SchemaName = "Hangfire"
                }));

            builder.Services.AddHangfireServer(options =>
            {
                options.ServerName = "MyHangfireServer";
                options.WorkerCount = Environment.ProcessorCount * 5;
            });
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

            builder.Services.Configure<PaymobSettings>(builder.Configuration.GetSection("PaymobSettings"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            // Add services to the container.
            //builder.Services.AddAuthentication("Bearer")
            // .AddJwtBearer("Bearer", options =>
            // {
            //     options.Authority = "https://sts.x-inity.com";
            //     options.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidateAudience = false
            //     };
            // });

            //builder.Services.AddAuthorization(Options =>
            //{
            //    Options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "x_inity_mobile_app_react"));
            //});
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.AddAppAuthetication();
            builder.Services.AddApplicationServices();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen(options =>
            {
                options.DocumentFilter<SwaggerDocumentFilter>();

                options.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date",
                    Example = new OpenApiString("2024-06-01")
                });
                // Add your security definition and requirement here
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
                // Prevent schema ID conflicts when multiple types have the same name
                options.CustomSchemaIds(type => type.FullName);
            });

            builder.Services.AddHttpClient();

            builder.Services.AddHttpContextAccessor();


            builder.Services.AddMemoryCache();

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            // Configure compression level
            builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            builder.Services.AddMassTransit(x =>
            {

                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumer<InjuryStatusUpdatedConsumer, InjuryStatusUpdatedConsumerDefinition>();
                x.AddConsumer<UserLevelUpdatedConsumer, UserLevelUpdatedConsumerDefinition>();
                x.AddConsumer<UserSportDeletedConsumer, UserSportDeletedConsumerDefinition>();
                x.AddConsumer<AddProductEventConsumer, AddProductEventConsumerDefinition>();



                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(builder.Configuration["RabbitMq:Host"]!), h =>
                    {
                        h.Username(builder.Configuration["RabbitMq:Username"]!);
                        h.Password(builder.Configuration["RabbitMq:Password"]!);
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
            #region Swagger

            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigOptions>();

            builder.Services.AddSwaggerGen(options =>
            {
                options.DocumentFilter<SwaggerDocumentFilter>();
            });
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.SubstituteApiVersionInUrl = true;
            });
            #endregion

            var app = builder.Build();
            app.UseSerilogRequestLogging();
            _ = app.RegisterPermissionAsync(Assembly.GetExecutingAssembly());

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<EntityMembershipDbContext>();

                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                    await context.Database.MigrateAsync();
                await SeedData.Initialize(services);
            }

            // Use Hangfire dashboard
            app.UseHangfireDashboard("/hangfire");

            // Use Hangfire server
            //app.UseHangfireServer();

            // Configure the HTTP request pipeline.
            var versionDescProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var desc in versionDescProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"Membership API - {desc.GroupName.ToUpper()}");
                }
            });
          


            /// <summary>
            /// Adds middleware to the application pipeline.
            /// </summary>

            //app.UseAllElasticApm(builder.Configuration);


            app.UseHttpsRedirection();
            app.UseCors(builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            app.UseMiddleware<TenantMiddleware>();

            app.UseAuthorization();

            app.UseResponseCaching();
            app.UseResponseCompression();

            app.MapControllers();

            //app.UseMiddleware<PermissionMiddleware>();

            app.Run();
        }
    }
}