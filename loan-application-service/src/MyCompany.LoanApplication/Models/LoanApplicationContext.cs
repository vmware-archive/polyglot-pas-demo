using System;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Models
{
	public class LoanApplicationContext : DbContext {
		public LoanApplicationContext(DbContextOptions<LoanApplicationContext> options) : base(options){ }

		public DbSet<LoanApplicationEntity> Loans { get; set; }
	}
}
