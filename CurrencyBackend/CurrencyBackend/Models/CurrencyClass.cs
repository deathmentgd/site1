using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencySource.Models
{
	/// <summary>
	/// Класс списка валют из источника
	/// </summary>
	public class CurrencyCommon
	{
		/// <summary>
		/// Дата котировок
		/// </summary>
		public DateTime Date { get; set; }
		/// <summary>
		/// Список котировок
		/// </summary>
		public Dictionary<string, Currency> Valute { get; set; }
	}	

	/// <summary>
	/// Класс описания валюты
	/// </summary>
	public class Currency
	{
		[Key]
		/// <summary>
		/// Символьный код
		/// </summary>		
		public string CharCode { get; set; }
		/// <summary>
		/// Полное наименование
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Значение котировки
		/// </summary>
		public double Value { get; set; }
		/// <summary>
		/// Предыдущая котировка
		/// </summary>
		public double Previous { get; set; }
	}

	/// <summary>
	/// Класс описания валюты
	/// </summary>
	public class CurrencyTracked
	{
		[Key]
		/// <summary>
		/// Символьный код
		/// </summary>		
		public string CharCode { get; set; }
	}
}
