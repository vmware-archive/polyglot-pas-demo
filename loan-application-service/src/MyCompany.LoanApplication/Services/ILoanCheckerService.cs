using System;
using System.Threading.Tasks;

namespace LoanApplication.Services
{
	public interface ILoanCheckerService
	{
		Task<Models.LoanApplicationEntity> CheckApprovalAsync(Models.LoanApplicationEntity loanApp);
		Task<string> ServiceHealthCheck();
	}
}
