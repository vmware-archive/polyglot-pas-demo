using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Models
{
	public class LoanApplicationRepository : ILoanApplicationRepository
	{
		private LoanApplicationContext _db;

		public LoanApplicationRepository(LoanApplicationContext db)
		{
			_db = db;
		}

		public IQueryable<LoanApplicationEntity> Search(string FullName)
		{
			var loans = _db.Loans.Where(q => q.FullName.ToLower().Equals(FullName.ToLower()));
			return loans;
		}
		public async Task<LoanApplicationEntity> GetAsync(int loanId)
		{
			var loan = await _db.Loans.FirstOrDefaultAsync(q => q.Id == loanId);
			return loan;
		}
	}
}
