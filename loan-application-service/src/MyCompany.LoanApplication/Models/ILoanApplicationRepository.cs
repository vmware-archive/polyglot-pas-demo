using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApplication.Models
{
	public interface ILoanApplicationRepository
	{
		IQueryable<LoanApplicationEntity> SearchByName(string fullName);
		Task<LoanApplicationEntity> GetAsync(Guid id);
		LoanApplicationEntity Get(Guid id);
		Task<List<LoanApplicationEntity>> ListAsync();
		Task<LoanApplicationEntity> AddAsync(NewLoanApplication loan);
		LoanApplicationEntity Add(NewLoanApplication loan);
		void Remove(Guid id);
		void Update(LoanApplication loanApp);
	}
}
