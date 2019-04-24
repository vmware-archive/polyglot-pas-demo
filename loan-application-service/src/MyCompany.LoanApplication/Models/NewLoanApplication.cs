using System;
using Newtonsoft.Json;

namespace LoanApplication.Models
{
	public class NewLoanApplication
	{
		string FullName { get; set; }
		double Amount { get; set; }
		public override string ToString()
		{
			return $"Loan[{this.FullName},{this.Amount}]";
		}
		public string AsJson()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
