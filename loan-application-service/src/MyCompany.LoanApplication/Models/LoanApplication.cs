using System;
using Newtonsoft.Json;

namespace LoanApplication.Models
{
	public class LoanApplication
	{
		public string id { get; set; }
		public string name { get; set; }
		public double amount { get; set; }
		public LoanStatus status { get; set; }

		public override string ToString()
		{
			return $"Loan[{this.id},{this.name},{this.amount},{this.status}]";
		}
		public string AsJson()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
