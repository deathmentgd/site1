using CurrencyBackend.Models;
using CurrencySource.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyBackend.Controllers
{
	/// <summary>
	/// Управление списком доступных валют для отслеживания
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class AvailableCurrencyController : ControllerBase
	{
		/// <summary>
		/// Кэш
		/// </summary>
		private readonly IMemoryCache _cache;
		/// <summary>
		/// Досупные валюты для отслеживания
		/// </summary>
		public List<Currency> AvailableCurrencyList { get; set; }

		public AvailableCurrencyController(IMemoryCache memoryCache)
		{
			_cache = memoryCache;
		}

		//---------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Получить список из кэша
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Currency>>> Get()
		{
			return GetOrCreateAvailableList();
		}

		private List<Currency> GetOrCreateAvailableList()
		{
			var cacheEntry = _cache.GetOrCreate(CacheKeys.AvailableCurrencyList, entry =>
			{
				entry.SetSlidingExpiration(TimeSpan.FromHours(3));
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3);
				using (ApplicationContext db = new ApplicationContext())
				{
					return db.CurrencyList.Where(x => !db.TrackedCurrencyList.Select(y => y.CharCode).Contains(x.CharCode)).ToList();
				}
			});
			return cacheEntry;
		}

		/// <summary>
		/// Получить список из базы
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[HttpGet("{value}")]
		public async Task<ActionResult<IEnumerable<Currency>>> Get(bool value)
		{
			using (ApplicationContext db = new ApplicationContext())
			{
				List<Currency> list = await db.CurrencyList.Where(x => !db.TrackedCurrencyList.Select(y => y.CharCode).Contains(x.CharCode)).ToListAsync();
				//Обновленный список в кэш
				var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(3));
				cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3);
				_cache.Set(CacheKeys.AvailableCurrencyList, list, cacheEntryOptions);

				return list;
			}
		}

		//-----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// При добавлении в список отслеживания - удалим валюту из списка доступных
		/// </summary>
		/// <param name="curr"></param>
		/// <returns></returns>
		[HttpDelete("{curr}")]
		public async Task<ActionResult<IEnumerable<Currency>>> Delete(string curr)
		{
			List<Currency> list;
			if (!_cache.TryGetValue(CacheKeys.AvailableCurrencyList, out list))
			{
				list = GetOrCreateAvailableList();
			}
			
			list.Remove(list.FirstOrDefault(x => x.CharCode == curr.ToUpper()));
			
			//Запишем результат в кэш
			var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(3));
			cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3);
			_cache.Set(CacheKeys.AvailableCurrencyList, list, cacheEntryOptions);			

			return list;
		}

	}
}
