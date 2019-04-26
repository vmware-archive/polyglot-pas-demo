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

		public IQueryable<LoanApplicationEntity> SearchByName(string fullName)
		{
			var loans = _db.Loans.Where(q => q.FullName.ToLower().Equals(fullName.ToLower()));
			return loans;
		}
		public async Task<LoanApplicationEntity> GetAsync(Guid id)
		{
			var loan = await _db.Loans.FirstOrDefaultAsync(q => q.Id == id);
			return loan;
		}
		public async Task<List<LoanApplicationEntity>> ListAsync()
		{
			var loan = await _db.Loans.ToListAsync();
			return loan;
		}
		public async Task<LoanApplicationEntity> AddAsync(LoanApplicationEntity loan)
		{
			await _db.Loans.AddAsync(loan);
			await _db.SaveChangesAsync();

			var newLoan = await GetAsync(loan.Id);

			return newLoan;
		}
		public async void RemoveAsync(Guid id){
			var newLoan = await GetAsync(id);
			_db.Remove(newLoan);
			await _db.SaveChangesAsync();

			return;
		}
	}
}
