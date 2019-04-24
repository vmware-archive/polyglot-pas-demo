using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApplication.Models
{
	public interface ILoanApplicationRepository
	{
		IQueryable<LoanApplicationEntity> Search(string text);
		Task<LoanApplicationEntity> GetAsync(int id);
	}
}
