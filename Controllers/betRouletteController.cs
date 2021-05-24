using Microsoft.AspNetCore.Mvc;
using RoulettePlay.Entities.Models;
using RoulettePlay.Services.Interfaces.Businnes;
using RoulettePlay.Services.Interfaces.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoulettePlay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class betRouletteController : ControllerBase
    {
        private readonly IDBAcces _IBetRoulette;
        public betRouletteController(IDBAcces betRoulette)
        {
            _IBetRoulette = betRoulette;
        }       
        [HttpPost("createBetRoulette")]
        public async Task<bool> createAsync(betRouletteClient objbetRouletteClient)
        {
            bool isSuccesfull = false;
            try
            {
                betRoulette objbetRoulette = new betRoulette();
                objbetRoulette.idRoulette = objbetRouletteClient.idRoulette;
                objbetRoulette.betRouletteNumber = objbetRouletteClient.betNumber;
                objbetRoulette.betRouletteColour = objbetRouletteClient.betColour;
                objbetRoulette.betRouletteValue = objbetRouletteClient.betValue;
                if (Request.Headers.TryGetValue("autenticated_User", out var headerValue))
                {
                    objbetRoulette.betUser = headerValue.FirstOrDefault();
                    int idRoulette = await _IBetRoulette._repository.BetRoulette.createAsync(objbetRoulette);
                    if (idRoulette > 0)
                    {
                        isSuccesfull = true;
                    }
                }
                else
                {
                    throw new Exception("Nó se logró obtener el usuario para la transacción. POr favor intentelo nuevamente.");
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSuccesfull;
        }

    }
}
