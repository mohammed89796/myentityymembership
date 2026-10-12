using Elastic.Apm.SerilogEnricher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;

namespace Common.Logging
{
	public static class SeriLogger
	{
		public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
				 (context, configuration) =>
				 {
					 var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
					 var elasticLogName = context.Configuration.GetValue<string>("ElasticConfiguration:LogName");
					 if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
						 configuration.Enrich.FromLogContext()
													//.Enrich.WithMachineName()
													.MinimumLevel.Information()
													.WriteTo.Console()
													//.Enrich.WithElasticApmCorrelationInfo()
													.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Warning)
													.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
													.Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
													.Enrich.WithProperty("HostName", System.Net.Dns.GetHostName())
													.Enrich.WithProperty("GeoLocation", "");
					 else
                         configuration.Enrich.FromLogContext()
                                                    .Enrich.WithMachineName()
                                                    .MinimumLevel.Information()
                                                    .WriteTo.Console()
                                                    .Enrich.WithElasticApmCorrelationInfo()
													.WriteTo.Elasticsearch(
														new ElasticsearchSinkOptions(new Uri(elasticUri))
														{
															IndexFormat = $"{elasticLogName}-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.Now.AddHours(2):yyyy-MM-dd}",
															AutoRegisterTemplate = true,
															NumberOfShards = 1,
															NumberOfReplicas = 1
														})

													.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Warning)
                                                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                                                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                                                    .Enrich.WithProperty("HostName", System.Net.Dns.GetHostName())
                                                    .Enrich.WithProperty("GeoLocation", "")
                                                    .ReadFrom.Configuration(context.Configuration);

                 };

	}
}
