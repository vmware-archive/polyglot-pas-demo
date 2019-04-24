using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanApplication.Models
{
	public class LoanApplicationEntity
	{
		[Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
		public string FullName { get; set; }
		public double Amount { get; set; }
		public LoanStatus LoanStatus { get; set; }
	}

	
}
