using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace LoanApplication.Services
{
	public class LoanCheckerService: ILoanCheckerService
	{
		private HttpClient _client;
		private IOptions<Services.LoanCheckerOptions> _loanCheckerOptions;
		private ILogger<LoanCheckerService> _logger;

		public LoanCheckerService(HttpClient client,
											IOptions<Services.LoanCheckerOptions> loanCheckerOptions,
											ILogger<LoanCheckerService> logger)
		{
			_client = client;
			_loanCheckerOptions = loanCheckerOptions;
			_logger = logger;
		}
		public async Task<string> ServiceHealthCheck(){
			HttpResponseMessage resp;

			try{
				resp = await _client.GetAsync(_loanCheckerOptions.Value.ServiceHealthPath);
			}catch(Exception){
				throw;
			}

			if (!resp.IsSuccessStatusCode){
				throw new IOException(string.Format("Service health check error, return http status code{0}", resp.StatusCode.ToString()));
			}

			return resp.ToString();
		}
		public async Task<Models.LoanApplication> CheckApprovalAsync(Models.LoanApplication loanApp){

			_logger.LogInformation("Sending loan for approval");
			_logger.LogInformation(loanApp.AsJson());

			var s_loanApp = new StringContent(loanApp.AsJson(), Encoding.UTF8, "application/json");

			HttpResponseMessage resp;

			try{
				resp = await _client.PostAsync(_loanCheckerOptions.Value.ApprovalCheckPath, s_loanApp);
			}catch(Exception){
				throw;
			}
			
			if (!resp.IsSuccessStatusCode){
				_logger.LogError("Error requesting approval, http code {StatusCode}, reason {ReasonPhrase}", resp);
				throw new IOException(string.Format("Error checking loan, return http status code {0}", resp.StatusCode));
			}

			string respContent = await resp.Content.ReadAsStringAsync();

			_logger.LogInformation("Approval service response");
			_logger.LogInformation(respContent);

			var respLoan = Models.LoanApplication.FromJson(respContent);

			return respLoan;
		}
	}
}
