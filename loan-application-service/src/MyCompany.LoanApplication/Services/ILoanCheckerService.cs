using System;
using System.Threading.Tasks;

namespace LoanApplication.Services
{
	public interface ILoanCheckerService
	{
		Task<bool> CheckApplicationAsync(Models.NewLoanApplication loanApp);
	}
}
