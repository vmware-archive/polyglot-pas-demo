using System;
using System.Threading.Tasks;

namespace LoanApplication.Services
{
	public interface ILoanCheckerService
	{
		Task<bool> CheckApprovalAsync(Models.NewLoanApplication loanApp);
		Task<string> ServiceHealthCheck();
	}
}
