using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LoanApplication.Models
{
	public class LoanApplicationRepository : ILoanApplicationRepository
	{
		private LoanApplicationContext _db;
		private readonly ILogger<LoanApplicationRepository> _logger;
		private readonly IOptions<Services.LoansConfigurationOptions> _loansConfig;

		public LoanApplicationRepository(LoanApplicationContext db,
					ILogger<LoanApplicationRepository> logger,
					IOptions<Services.LoansConfigurationOptions> loansConfig)
		{
			_db = db;
			_logger = logger;
			_loansConfig = loansConfig;
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
		public LoanApplicationEntity Get(Guid id)
		{
			var loan = _db.Loans.First(q => q.Id.Equals(id));
			return loan;
		}
		public async Task<List<LoanApplicationEntity>> ListAsync()
		{
			var loan = await _db.Loans.ToListAsync();
			return loan;
		}
		public async Task<LoanApplicationEntity> AddAsync(NewLoanApplication newLoanApp){
			if(newLoanApp.amount > _loansConfig.Value.max_loan_amount){
				throw new ArgumentOutOfRangeException("amount");
			}

			var createApp = new Models.LoanApplicationEntity(){
				FullName = newLoanApp.name,
				Amount = newLoanApp.amount,
				LoanStatus = Models.LoanStatus.PENDING
			};

			await _db.Loans.AddAsync(createApp);
			await _db.SaveChangesAsync();
			
			return createApp;
		}
		public LoanApplicationEntity Add(NewLoanApplication newLoanApp){
			if(newLoanApp.amount > _loansConfig.Value.max_loan_amount){
				_logger.LogInformation("Loan rejected due to amount > max");
				throw new ArgumentOutOfRangeException("amount");
			}

			var createApp = new Models.LoanApplicationEntity(){
				FullName = newLoanApp.name,
				Amount = newLoanApp.amount,
				LoanStatus = Models.LoanStatus.PENDING
			};

			_db.Loans.Add(createApp);
			_db.SaveChanges();
			
			return createApp;
		}
		public async void UpdateAsync(LoanApplication loanApp){
			var loanAs = loanApp.AsLoanApplicationEntity();

			var loan = await _db.Loans.FirstAsync(q => q.Id.Equals(loanAs.Id));

			loan.LoanStatus = loanAs.LoanStatus;
			
			_db.SaveChanges();

			return;
		}
		public  void Update(LoanApplication loanApp){
			var loanAs = loanApp.AsLoanApplicationEntity();

			var loan =  _db.Loans.First(q => q.Id.Equals(loanAs.Id));

			loan.LoanStatus = loanAs.LoanStatus;
			
			_db.SaveChanges();

			return;
		}
		public async void RemoveAsync(Guid id){
			LoanApplicationEntity loan = _db.Loans.Find(id);
			
			_db.Remove(loan);
			await _db.SaveChangesAsync();

			return;
		}
		public void Remove(Guid id){
			LoanApplicationEntity loan = _db.Loans.Find(id);
			
			_db.Remove(loan);
			_db.SaveChanges();

			return;
		}
	}
}
