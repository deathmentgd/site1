using CurrencySource.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencySource.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CurrencyController : ControllerBase
	{
        public CurrencyController()
        {

        }

        [HttpGet]
        public async Task<ActionResult<CbrDaily>> Get()
        {
            var result = await CbrDailyGetter.GetValuesAsync();
            return result;
        }

    }
}

