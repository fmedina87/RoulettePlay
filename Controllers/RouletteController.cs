using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoulettePlay.Entities.Models;
using RoulettePlay.Services.Interfaces.Businnes;
using RoulettePlay.Services.Interfaces.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RoulettePlay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RouletteController : ControllerBase
    {
        private readonly IDBAcces _Iroulette;
       // private readonly IRoulette _Iroulette;
        public RouletteController(IDBAcces Roulette)
        {
            _Iroulette = Roulette;
        }

        [HttpGet("getAllRoluette")]
        public async Task<List<Roulette>> ReadAll()
        {
            List<Roulette> lstRoluette = new List<Roulette>();
            try
            {
                lstRoluette = await _Iroulette._repository.Roulette.ReadAllAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstRoluette;
        }
        [HttpPost("createRoulette")]
        public async Task<int> createAsync()
        {
            int idRoulette = 0;
            try
            {
                idRoulette= await _Iroulette._repository.Roulette.createAsync(new Roulette());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idRoulette;            
        }
        [HttpPut("openRoulette")]
        public async Task<bool> RouletteOpen(int idRoulette)
        {
            bool isSuccesfull = false;
            try
            {
                isSuccesfull = await _Iroulette._repository.Roulette.RouletteOpen(idRoulette);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSuccesfull;
        }
        [HttpPut("closeRoulette")]

        public async Task<List<betRoulette>> RouletteClose(int idRoulette)
        {
            List<betRoulette> lstRoulette = null;
            try
            {
                lstRoulette =await  _Iroulette._repository.Roulette.RouletteClose(idRoulette);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstRoulette;
        }
    }
}
