using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.IO;

namespace LoanApplication.Controllers{
	[Route("")]
	[ApiController]
	public class LoanApplicationController : ControllerBase{
		private CloudFoundryApplicationOptions _appOptions;
		private CloudFoundryServicesOptions _serviceOptions;
		private readonly ILogger<LoanApplicationController> _logger;
		private Models.ILoanApplicationRepository _loans;
		private Services.ILoanCheckerService _loanCheckerservice;
		private Services.LoanCheckerOptions _loanCheckerOptions;

		public LoanApplicationController(IOptions<CloudFoundryApplicationOptions> appOptions,
											IOptions<CloudFoundryServicesOptions> serviceOptions,
											ILogger<LoanApplicationController> logger,
											Models.ILoanApplicationRepository loans,
											Services.ILoanCheckerService loanCheckerService,
											IOptions<Services.LoanCheckerOptions> loanCheckerOptions)
		{
			_appOptions = appOptions.Value;
			_serviceOptions = serviceOptions.Value;
			_logger = logger;
			_loans = loans;
			_loanCheckerservice = loanCheckerService;
			_loanCheckerOptions = loanCheckerOptions.Value;
		}

		[HttpGet]
		public async Task<ActionResult<string>> Get(){
			var ret = await _loanCheckerservice.ServiceHealthCheck();
			return "Hi There. Service status: " + ret;
		}
		
		// GET /<id>
		[HttpGet("{id}")]
		//[Authorize(Policy = "read.loans")]
		public async Task<ActionResult<Models.LoanApplication>> Get(Guid loanId){
			var loan = await _loans.GetAsync(loanId);

			if(loan != null){
				return new Models.LoanApplication() {
					id = loan.Id.ToString(),
					name = loan.FullName,
					amount = loan.Amount,
					status = loan.LoanStatus
				};
			}

			return NoContent();
		}
		
		// POST /apply
		[HttpPost("apply")]
		//[Authorize(Policy = "add.loans")]
		public async Task<ActionResult<Models.LoanApplication>> ApplyForLoan([FromBody] Models.NewLoanApplication newApp){
			var loanApp = new Models.LoanApplicationEntity(){
				FullName = newApp.name,
				Amount = newApp.amount,
				LoanStatus = Models.LoanStatus.Pending
			};

			//Add the new entry to get id
			var loan = await _loans.AddAsync(loanApp);

			//check for approval
			Models.LoanApplicationEntity loanApproval = null;
			try{
				loanApproval = await _loanCheckerservice.CheckApprovalAsync(loan);
			}catch(IOException io){ //http response code != 200
				_logger.LogError(io,"Error running loan check");
				return BadRequest();
			}catch(Exception ex){
				_logger.LogError(ex, "General error running loan check");
				return BadRequest();
			}

			if (loanApproval == null) {
				_logger.LogError("returned loan entity was null");
				return BadRequest();
			}
			
			return new Models.LoanApplication() {
				id = loanApproval.Id.ToString(),
				name = loanApproval.FullName,
				amount = loanApproval.Amount,
				status = loanApproval.LoanStatus
			};
		}

		// GET /list
		[HttpGet("list")]
		//[Authorize(Policy = "read.loans")]
		public async Task<List<Models.LoanApplication>> List()
		{
			var loans = await _loans.ListAsync();
			var result = new List<Models.LoanApplication>();

			foreach (var loan in loans)
				result.Add(new Models.LoanApplication(){
					id = loan.Id.ToString(),
					name = loan.FullName,
					amount = loan.Amount,
					status = loan.LoanStatus
				});

			return result;
		}

		// GET /list
		[HttpDelete("{id}")]
		//[Authorize(Policy = "read.loans")]
		public void Remove(Guid id)
		{
			_loans.RemoveAsync(id);

			return;
		}
	}
}
