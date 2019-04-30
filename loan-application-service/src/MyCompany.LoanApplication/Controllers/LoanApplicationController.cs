using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

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
		private IHostingEnvironment _env;

		public LoanApplicationController(IOptions<CloudFoundryApplicationOptions> appOptions,
											IOptions<CloudFoundryServicesOptions> serviceOptions,
											ILogger<LoanApplicationController> logger,
											Models.ILoanApplicationRepository loans,
											Services.ILoanCheckerService loanCheckerService,
											IOptions<Services.LoanCheckerOptions> loanCheckerOptions,
											IHostingEnvironment env)
		{
			_appOptions = appOptions.Value;
			_serviceOptions = serviceOptions.Value;
			_logger = logger;
			_loans = loans;
			_loanCheckerservice = loanCheckerService;
			_loanCheckerOptions = loanCheckerOptions.Value;
			_env = env;
		}

		[HttpGet]
		public async Task<ActionResult<string>> Get(){
			if (_env.IsDevelopment())
				return "This is development.";

			var ret = await _loanCheckerservice.ServiceHealthCheck();
			return "Hi There. Service status: " + ret;
		}
		
		// GET /<id>
		[HttpGet("{loanId}")]
		public async Task<ActionResult<Models.LoanApplication>> Get(Guid loanId){
			Models.LoanApplicationEntity loan = null;

			try{
				loan = await _loans.GetAsync(loanId);
			}catch(ArgumentNullException){
				return NoContent();
			}catch(Exception ex){
				_logger.LogError(ex, "General error retriving loan");
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			return loan.AsLoanApplication();
		}
		
		// POST /apply
		[HttpPost("apply")]
		public async Task<ActionResult<Models.LoanApplication>> ApplyForLoan([FromBody] Models.NewLoanApplication newApp){
			if (!ModelState.IsValid){
				return BadRequest(ModelState);
			}

			if (string.IsNullOrEmpty(newApp.name))
				return BadRequest("Missing loan applicant name");

			//Add the new entry to get id
			var loan = await _loans.AddAsync(newApp);

			if(_env.IsDevelopment())
				return Models.LoanApplication.FromJson(loan.AsLoanApplication().AsJson());

			//check for approval
			Models.LoanApplication loanApproval = null;
			try{
				loanApproval = await _loanCheckerservice.CheckApprovalAsync(loan.AsLoanApplication());
			}catch(IOException io){ //http response code != 200
				_logger.LogError(io,"Error running loan check");
				return StatusCode(StatusCodes.Status500InternalServerError);
			}catch(Exception ex){
				_logger.LogError(ex, "General error running loan check");
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			if (loanApproval == null) {
				_logger.LogError("returned loan entity was null");
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			//save new status
			try{
				_loans.UpdateAsync(loanApproval);
			}catch(ArgumentNullException an){
				_logger.LogError(an, "Error updating loan status after check for approval, argument {0} value was not found",an.ParamName);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}catch(Exception ex){
				_logger.LogError(ex, "General error updating loan status after check for approval");
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			return loanApproval;
		}

		// GET /list
		[HttpGet("list")]
		public async Task<List<Models.LoanApplication>> List()
		{
			var loans = await _loans.ListAsync();
			var result = new List<Models.LoanApplication>();

			foreach (var loan in loans)
				result.Add(loan.AsLoanApplication());

			return result;
		}

		// GET /list
		[HttpDelete("{id}")]
		public void Remove(Guid id)
		{
			_loans.RemoveAsync(id);

			return;
		}
	}
}
