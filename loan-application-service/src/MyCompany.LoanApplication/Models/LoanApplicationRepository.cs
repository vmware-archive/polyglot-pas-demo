﻿using System;
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
			var loan = await _db.Loans.FirstAsync(q => q.Id.Equals(id));
			return loan;
		}
		public async Task<List<LoanApplicationEntity>> ListAsync()
		{
			var loan = await _db.Loans.ToListAsync();
			return loan;
		}
		public async Task<LoanApplicationEntity> AddAsync(NewLoanApplication newLoanApp){
			var createApp = new Models.LoanApplicationEntity(){
				FullName = newLoanApp.name,
				Amount = newLoanApp.amount,
				LoanStatus = Models.LoanStatus.Pending
			};

			await _db.Loans.AddAsync(createApp);
			await _db.SaveChangesAsync();

			var newLoan = await GetAsync(createApp.Id);

			return newLoan;
		}
		public async void UpdateAsync(LoanApplication loanApp){
			var loanAs = loanApp.AsLoanApplicationEntity();

			var loan = await GetAsync(loanAs.Id);

			loan.LoanStatus = loanAs.LoanStatus;
			
			await _db.SaveChangesAsync();

			return;
		}
		public async void RemoveAsync(Guid id){
			var newLoan = await GetAsync(id);
			_db.Remove(newLoan);
			await _db.SaveChangesAsync();

			return;
		}
	}
}
