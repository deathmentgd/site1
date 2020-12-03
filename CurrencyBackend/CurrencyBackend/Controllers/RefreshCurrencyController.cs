using CurrencyBackend.Models;
using CurrencySource.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyBackend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RefreshCurrencyController : ControllerBase
	{
		private readonly IConfiguration _config;
		public RefreshCurrencyController(IConfiguration config)
		{
			_config = config;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Currency>>> Get()
		{
			CurrencyCommon result;
			string uri = _config.GetValue<string>("UrlForNewValues"); 

			WebRequest request = WebRequest.Create(uri);
			request.Credentials = CredentialCache.DefaultCredentials;

			using (var response = request.GetResponse())
			{
				using (Stream dataStream = response.GetResponseStream())
				{
					StreamReader reader = new StreamReader(dataStream);

					string responseFromServer = reader.ReadToEnd();
					result = JsonConvert.DeserializeObject<CurrencyCommon>(responseFromServer);
				}				
				response.Close();
			}

			using (ApplicationContext db = new ApplicationContext())
			{
				db.CurrencyList.UpdateRange(result.Valute.Select(x => x.Value));
				await db.SaveChangesAsync();
			}
			
			return Ok();
		}
	}
}
