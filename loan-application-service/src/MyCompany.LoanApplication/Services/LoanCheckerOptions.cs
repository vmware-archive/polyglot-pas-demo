using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApplication.Services
{
	public class LoanCheckerOptions
	{
		public string Scheme { get; set; } = "http";
		public string Address { get; set; }
		public string CheckApplicationPath { get; set; }
	}
}
