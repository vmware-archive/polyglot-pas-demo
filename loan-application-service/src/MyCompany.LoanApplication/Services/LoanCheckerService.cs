using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LoanApplication.Services
{
	public class LoanCheckerService: ILoanCheckerService
	{
		private HttpClient _client;
		private IOptions<Services.LoanCheckerOptions> _loanCheckerOptions;

		public LoanCheckerService(HttpClient client,
											IOptions<Services.LoanCheckerOptions> loanCheckerOptions)
		{
			_client = client;
			_loanCheckerOptions = loanCheckerOptions;
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
			var s_loanApp = new StringContent(loanApp.AsJson(), Encoding.UTF8, "application/json");

			HttpResponseMessage resp;

			try{
				resp = await _client.PostAsync(_loanCheckerOptions.Value.ApprovalCheckPath, s_loanApp);
			}catch(Exception){
				throw;
			}
			
			if (!resp.IsSuccessStatusCode){
				throw new IOException(string.Format("Error checking loan, return http status code{0}", resp.StatusCode.ToString()));
			}

			var respLoan = JsonConvert.DeserializeObject<Models.LoanApplication>(resp.Content.ToString());

			return respLoan;
		}
	}
}
