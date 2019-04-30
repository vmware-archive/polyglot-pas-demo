using System;
using System.Threading.Tasks;

namespace LoanApplication.Services
{
	public interface ILoanCheckerService
	{
		Task<Models.LoanApplication> CheckApprovalAsync(Models.LoanApplication loanApp);
		Task<string> ServiceHealthCheck();
	}
}
