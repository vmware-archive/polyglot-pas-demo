using System;
using Newtonsoft.Json;

namespace LoanApplication.Models
{
	public class LoanApplication
	{
		public int Id { get; set; }
		public string FullName { get; set; }
		public double Amount { get; set; }
		public LoanStatus LoanStatus { get; set; }

		public override string ToString()
		{
			return $"Loan[{this.Id},{this.FullName},{this.Amount},{this.LoanStatus}]";
		}
		public string AsJson()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
