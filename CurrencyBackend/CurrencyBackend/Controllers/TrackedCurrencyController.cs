using CurrencyBackend.Models;
using CurrencySource.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CurrencyBackend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TrackedCurrencyController : ControllerBase
	{
        /// <summary>
        /// Включаем кэш
        /// </summary>
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        /// <summary>
        /// Список отслеживаемых
        /// </summary>
        public List<Currency> TrackedCurrencyList { get; set; }

        public TrackedCurrencyController(IMemoryCache memoryCache, IConfiguration config)
        {
            _cache = memoryCache;
            _config = config;
        }

        private void GetNewValues()
		{
            string uri = $"{_config.GetValue<string>("Kestrel:EndPoints:Http:Url")}/api/refreshcurrency";
            WebRequest request = WebRequest.Create(uri);
            request.Credentials = CredentialCache.DefaultCredentials;

            using (var response = request.GetResponse())
            {
                response.Close();
            }

            //Запишем в кэш дату обновления
            var cacheEntry = _cache.GetOrCreate(CacheKeys.TrackedDateValues, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromHours(25));
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(25);
                return DateTime.Now.Date;             
            });
        }

        //------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Получить список отслеживаемых из кэша
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Currency>>> Get()
        {
            var cacheEntry = _cache.GetOrCreate(CacheKeys.TrackedDateValues, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromHours(25));
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(25);
                return DateTime.Now.Date;
            });

            if (DateTime.Now.Date.Day != cacheEntry.Day)
            {
                //Если новый день - то рефрешим новые коитровки
                GetNewValues();
                return await Get(true);
            }
            else
            {
                //Если текущий день - то как обычно
                return GetOrCreateTrackedList();
            }
        }

        private List<Currency> GetOrCreateTrackedList()
        {
            var cacheEntry = _cache.GetOrCreate(CacheKeys.TrackedCurrencyList, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromHours(3));
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3);
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.TrackedCurrencyList.Join(db.CurrencyList, tc => tc.CharCode, c => c.CharCode, (tc, c) => c).ToList();
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
            GetNewValues();
            using (ApplicationContext db = new ApplicationContext())
            {
                List<Currency> list = await db.TrackedCurrencyList.Join(db.CurrencyList, tc => tc.CharCode, c => c.CharCode, (tc, c) => c).ToListAsync();
                //Обновленный список в кэш
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(3));
                cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3);
                _cache.Set(CacheKeys.TrackedCurrencyList, list, cacheEntryOptions);

                return list;
            }
        }

        //------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Добавить в список отслеживания
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Currency>>> Post(Currency currency)
        {
            if (currency == null)
            {
                return BadRequest();
            }

            using (ApplicationContext db = new ApplicationContext())
            {
                var curr = new CurrencyTracked();
                curr.CharCode = currency.CharCode;
                db.TrackedCurrencyList.Add(curr);
                await db.SaveChangesAsync();
            }

            List<Currency> list;
            if (!_cache.TryGetValue(CacheKeys.TrackedCurrencyList, out list))
            {
                list = GetOrCreateTrackedList();
            }

            var item = list.FirstOrDefault(x => x.CharCode == currency.CharCode);
            if (item == null)
            {
                list.Add(currency);
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(3));
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3);
            _cache.Set(CacheKeys.TrackedCurrencyList, list, cacheEntryOptions);

            return list;
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Удалить из списка отслеживаемых
        /// </summary>
        /// <param name="curr"></param>
        /// <returns></returns>
        [HttpDelete("{curr}")]
        public async Task<ActionResult<IEnumerable<Currency>>> Delete(string curr)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var currency = db.TrackedCurrencyList.FirstOrDefault(x => x.CharCode == curr.ToUpper());
                if (currency != null)
                {
                    db.TrackedCurrencyList.Remove(currency);
                    await db.SaveChangesAsync();

                    //Обновляем список в кэше
                    List<Currency> list;
                    if (!_cache.TryGetValue(CacheKeys.TrackedCurrencyList, out list))
                    {
                        list = GetOrCreateTrackedList();
                    }
                   
                    list.Remove(list.FirstOrDefault(x => x.CharCode == currency.CharCode));
                    
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(3));
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3);
                    _cache.Set(CacheKeys.TrackedCurrencyList, list, cacheEntryOptions);
                }

                return GetOrCreateTrackedList();
            }
        }
    }
}
