using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LoanApplication.Models
{
	public class LoanApplication
	{
		private readonly ILogger _logger = null;

		public string id { get; }
		public string name { get; }
		public double amount { get; }
		public string status { get; }

		public LoanApplication(string id,
				string name,
				double amount,
				string status){

			if (string.IsNullOrEmpty(id))
				throw new NullReferenceException("id");

			if (string.IsNullOrEmpty(status))
				throw new NullReferenceException("status");

			if (string.IsNullOrEmpty(name))
				throw new NullReferenceException("name");

			this.id = id;
			this.amount = amount;
			this.name = name;
			this.status = status;
		}
		public override string ToString()
		{
			return $"LoanApplication[{id},{name},{amount},{status}]";
		}
		public string AsJson()
		{
			string ret = string.Format(@"{{""id"":""{0}"",""name"":""{1}"",""amount"":""{2}"",""status"":""{3}""}}", id, name, amount, status);
			
			return ret;
		}
		public LoanApplicationEntity AsLoanApplicationEntity(){
			return new LoanApplicationEntity(){
				Id = Guid.Parse(id),
				FullName = name,
				Amount = amount,
				LoanStatus = (LoanStatus)Enum.Parse(typeof(LoanStatus), status)
			};
		}
		public static LoanApplication FromJson(string json){
			LoanApplication loan = JsonConvert.DeserializeObject<LoanApplication>(json);

			return loan;
		}
	}
}
