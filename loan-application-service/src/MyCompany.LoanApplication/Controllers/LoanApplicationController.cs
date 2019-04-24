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
		public ActionResult<string> Get()
		{
			return "Hi There: " + _loanCheckerOptions.CheckApplicationPath;
		}
		
		// GET /5
		[HttpGet("{id}")]
		//[Authorize(Policy = "read.loans")]
		public async Task<ActionResult<Models.LoanApplication>> Get(int loanId){
			var loan = await _loans.GetAsync(loanId);

			if(loan != null){
				return new Models.LoanApplication() {
					Id = loan.Id,
					FullName = loan.FullName,
					Amount = loan.Amount,
					LoanStatus = loan.LoanStatus
				};
			}

			return NoContent();
		}
		
		// POST /apply
		[HttpPost("apply")]
		//[Authorize(Policy = "add.loans")]
		public async Task<IActionResult> ApplyForLoan([FromBody] Models.NewLoanApplication loan){
			try{
				bool isValid = await _loanCheckerservice.CheckApplicationAsync(loan);
			}catch(IOException io){ //http response code != 200
				_logger.LogError(io,"Error running loan check");
				return BadRequest();
			}catch(Exception ex){
				_logger.LogError(ex, "General error running loan check");
				return BadRequest();
			}

			return Ok();
		}

		// GET query
		/*[HttpGet("{id}")]
		//[Authorize(Policy = "read.loans")]
		public async Task<List<Models.LoanApplication>> Query()
		{
			throw new NotImplementedException();
			//var loans = await _loans.Search("");
			//var result = new List<Models.LoanApplication>();

			//foreach (var loan in loans)
			//	result.Add(new Models.LoanApplication(){
			//		Id = loan.Id,
			//		FullName = loan.FullName,
			//		Amount = loan.Amount,
			//		LoanStatus = loan.LoanStatus
			//	});

			//return result;
		}*/
	}
}
