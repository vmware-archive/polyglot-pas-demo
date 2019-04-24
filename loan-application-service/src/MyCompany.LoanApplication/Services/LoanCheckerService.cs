using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Options;

namespace LoanApplication.Services
{
	public class LoanCheckerService: ILoanCheckerService
	{
		private HttpClient _client;
		private Services.LoanCheckerOptions _loanCheckerOptions;

		public LoanCheckerService(HttpClient client,
											IOptions<Services.LoanCheckerOptions> loanCheckerOptions)
		{
			_client = client;
		}
		public async Task<bool> CheckApplicationAsync(Models.NewLoanApplication loanApp){
			var s_loanApp = new StringContent(loanApp.AsJson(), Encoding.UTF8, "application/json");

			HttpResponseMessage resp;

			try{
				resp = await _client.PostAsync(_loanCheckerOptions.CheckApplicationPath, s_loanApp);
			}catch(Exception){
				throw;
			}
			
			if (!resp.IsSuccessStatusCode){
				throw new IOException(string.Format("Error checking loan, return http status code{0}", resp.StatusCode.ToString()));
			}

			return true;
		}
	}
}
