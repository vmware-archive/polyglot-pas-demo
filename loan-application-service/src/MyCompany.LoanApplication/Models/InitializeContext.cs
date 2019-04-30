using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LoanApplication.Models
{
	public static class InitializeContext
	{
		public static async Task InitializeLoansAsync(IServiceProvider serviceProvider){
			if (serviceProvider == null){
				throw new ArgumentNullException("serviceProvider");
			}

			using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope()){
				var db = serviceScope.ServiceProvider.GetService<LoanApplicationContext>();
				//dotnet ef migrations InitialCommit
				//await db.Database.MigrateAsync();
				bool isCreated = await db.Database.EnsureCreatedAsync();

				if (!isCreated)
					throw new Exception("Database count not be initialized");

				db.Add(new LoanApplicationEntity() {
					Amount = 0,
					FullName = "Test Test",
					Id = Guid.Empty,
					LoanStatus = LoanStatus.Rejected
				});

				await db.SaveChangesAsync();
			}
		}
	}
}
