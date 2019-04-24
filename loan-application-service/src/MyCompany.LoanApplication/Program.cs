using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Logging;

namespace LoanApplication{
	public class Program{
		public static void Main(string[] args){
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseCloudFoundryHosting()
				//.AddCloudFoundry()
				.UseStartup<Startup>()
				.ConfigureAppConfiguration((hostingContext, config) =>{
					ILoggerFactory fac = GetLoggerFactory();
					var env = hostingContext.HostingEnvironment;

					config.AddConfigServer(env, fac);
				})
				.ConfigureLogging((builderContext, loggingBuilder) =>{
					loggingBuilder.AddDynamicConsole();
				})
				;

		public static ILoggerFactory GetLoggerFactory(){
			IServiceCollection serviceCollection = new ServiceCollection();
			serviceCollection.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Information));
			serviceCollection.AddLogging(builder => builder.AddConsole((opts) =>{
				opts.DisableColors = true;
			}));
			serviceCollection.AddLogging(builder => builder.AddDebug());
			return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
		}
	}
}