using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencySource.Models
{
	public class CbrDailyGetter
	{
		public static Task<CbrDaily> GetValuesAsync()
		{
			return Task.Run(() =>
			{
				CbrDaily result = null;
				var jsonString = string.Empty;
				try
				{
					using (var webClient = new System.Net.WebClient())
					{
						jsonString = webClient.DownloadString("https://www.cbr-xml-daily.ru/daily_json.js");
					}

					result = JsonConvert.DeserializeObject<CbrDaily>(jsonString);
				}
				catch
				{

				}
				return result;
			});
		}
	}
}
