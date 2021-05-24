using RoulettePlay.Entities.Models;
using RoulettePlay.Services.Interfaces.Businnes.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoulettePlay.Services.Interfaces.Businnes
{
    public interface IbetRoulette: ICreate<betRoulette,int>
    {
        Task<List<betRoulette>> updateBetRoulette(int idRoulette, int idOpeningClosingRoulette);
    }
}
