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
		Task<List<LoanApplicationEntity>> ListAsync();
		Task<LoanApplicationEntity> AddAsync(LoanApplicationEntity loan);
		void RemoveAsync(Guid id);
	}
}
