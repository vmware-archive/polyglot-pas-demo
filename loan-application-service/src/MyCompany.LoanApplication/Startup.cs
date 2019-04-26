using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

using LoanApplication.Models;

using Steeltoe.Management.CloudFoundry;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Discovery.Client;
using Steeltoe.Security.Authentication.CloudFoundry;
using Steeltoe.Management.Endpoint.Metrics;
using Steeltoe.Management.Exporter.Metrics;
using Steeltoe.Management.Tracing;
using Steeltoe.Management.Exporter.Tracing;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.CloudFoundry.Connector.SqlServer.EFCore;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Management.Endpoint.Refresh;

namespace LoanApplication{
	public class Startup{
		public Startup(IConfiguration configuration, IHostingEnvironment env){
			Configuration = configuration;
			Environment = env;
		}

		public IConfiguration Configuration { get; }
		public IHostingEnvironment Environment { get; }
		public ILogger Logger { get; }

		public void ConfigureServices(IServiceCollection services){
			services.AddOptions();
			
			if (Environment.IsDevelopment()){
				services.AddDbContext<LoanApplicationContext>(
								options => options.UseInMemoryDatabase("Loans"));
			}else if(Environment.IsStaging()){
				services.AddDbContext<LoanApplicationContext>(
						options => options.UseSqlServer(Configuration));
			}else{
				services.AddDbContext<LoanApplicationContext>(
						options => options.UseSqlServer(Configuration));
			}
			writeConfig();

			services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
			services.Configure<Services.LoanCheckerOptions>(Configuration.GetSection("loanApprovalService"));
			
			var opt = new Services.LoanCheckerOptions() {
				Address = Configuration["loanApprovalService:address"],
				ApprovalCheckPath = Configuration["loanApprovalService:approvalCheckPath"],
				Scheme = Configuration["loanApprovalService:scheme"],
				ServiceHealthPath = Configuration["loanApprovalService:serviceHealthPath"]
			};

			services.AddHttpClient("loanApplications", c =>{
				c.BaseAddress = new Uri(opt.Scheme + "://" + opt.Address);
			})
			.AddHttpMessageHandler<DiscoveryHttpMessageHandler>()
			.AddTypedClient<Services.ILoanCheckerService, Services.LoanCheckerService>();

			services.AddDiscoveryClient(Configuration);

			services.AddCloudFoundryActuators(Configuration);

			services.AddMetricsActuator(Configuration);
			services.AddMetricsForwarderExporter(Configuration);
			services.AddDistributedTracing(Configuration);
			services.AddZipkinExporter(Configuration);
			services.AddRefreshActuator(Configuration);

			services.ConfigureCloudFoundryOptions(Configuration);
			
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory){
			if (env.IsDevelopment()){
				app.UseDeveloperExceptionPage();
			}else if(env.IsStaging()){
				app.UseDeveloperExceptionPage();
				app.UseCloudFoundryActuators();
				app.UseMetricsExporter();
			}else{
				app.UseHsts();
				app.UseCloudFoundryActuators();
				app.UseMetricsExporter();
			}

			app.UseRefreshActuator();
			app.UseTracingExporter();
			app.UseMvc();
			app.UseDiscoveryClient();
		}
		private void writeChild(IConfigurationSection sec, int tabs)
		{
			string tab = "";
			for (int i = 0; i < tabs; i++)
				tab += "	";

			Console.WriteLine(tab + sec.Key + (string.IsNullOrEmpty(sec.Value) ? "" : ": " + sec.Value));
		}
		private void writeConfig(){
			Console.WriteLine("========================");
			foreach (IConfigurationSection a in Configuration.GetChildren())
			{
				writeChild(a, 0);

				foreach (IConfigurationSection b in a.GetChildren())
				{
					writeChild(b, 1);

					foreach (IConfigurationSection c in b.GetChildren())
					{
						writeChild(c, 2);

						foreach (IConfigurationSection d in c.GetChildren())
						{
							writeChild(d, 3);

							foreach (IConfigurationSection e in d.GetChildren())
							{
								writeChild(e, 4);
							}
						}
					}
				}
			}
			Console.WriteLine("========================");
		}
	}
}