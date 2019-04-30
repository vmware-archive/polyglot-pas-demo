using System;
using Newtonsoft.Json;

namespace LoanApplication.Models
{
	public class LoanApplication
	{
		public string id { get; set; }
		public string name { get; set; }
		public double amount { get; set; }
		public string status { get; set; }

		public override string ToString()
		{
			return $"Loan[{this.id},{this.name},{this.amount},{this.status}]";
		}
		public string AsJson()
		{
			return JsonConvert.SerializeObject(this);
		}

		public LoanApplicationEntity AsLoanApplicationEntity(){
			if (string.IsNullOrEmpty(this.id))
				throw new NullReferenceException("Id");

			if (string.IsNullOrEmpty(this.status))
				throw new NullReferenceException("status");

			if (string.IsNullOrEmpty(this.name))
				throw new NullReferenceException("name");

			return new LoanApplicationEntity(){
				Id = Guid.Parse(this.id),
				FullName = this.name,
				Amount = this.amount,
				LoanStatus = (LoanStatus)Enum.Parse(typeof(LoanStatus), this.status)
			};
		}
	}
}
