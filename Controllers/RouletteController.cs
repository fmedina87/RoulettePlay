using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoulettePlay.Entities.Models;
using RoulettePlay.Services.Interfaces.Businnes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoulettePlay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RouletteController : ControllerBase
    {
        private readonly IRoulette _Iroulette ;
        public RouletteController(IRoulette Roulette)
        {
            _Iroulette = Roulette;
        }

        [HttpGet]
        public async Task<List<Roulette>> Get()
        {
            List<Roulette> lstRoluette = new List<Roulette>();
            try
            {
                await _Iroulette.ReadAllsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstRoluette;
        }
    }
}
