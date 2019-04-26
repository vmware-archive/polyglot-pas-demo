using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace LoanApplication.Models
{
	public class LoanApplicationEntity
	{
		[Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public Guid Id { get; set; }
		public string FullName { get; set; }
		public double Amount { get; set; }
		public LoanStatus LoanStatus { get; set; }

		public string AsJson()
		{
			return JsonConvert.SerializeObject(this);
		}
	}

	
}
