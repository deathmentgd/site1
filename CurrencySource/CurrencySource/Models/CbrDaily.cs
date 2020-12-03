using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencySource.Models
{
	public class CbrDaily
	{
		public DateTime Date { get; set; }
		public Dictionary<string, Currency> Valute { get; set; }
	}	

	public class Currency
	{
		public string CharCode { get; set; }
		public string Name { get; set; }
		public double Value { get; set; }
		public double Previous { get; set; }
	}
}
