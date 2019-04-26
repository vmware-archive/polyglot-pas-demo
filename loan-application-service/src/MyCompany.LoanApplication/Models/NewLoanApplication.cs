using System;
using Newtonsoft.Json;

namespace LoanApplication.Models
{
	public class NewLoanApplication
	{
		public string name { get; set; }
		public double amount { get; set; }
		public override string ToString()
		{
			return $"Loan[{this.name},{this.amount}]";
		}
		public string AsJson()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
