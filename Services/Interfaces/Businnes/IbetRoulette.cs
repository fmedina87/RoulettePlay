using RoulettePlay.Entities.Models;
using RoulettePlay.Services.Interfaces.Businnes.Actions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoulettePlay.Services.Interfaces.Businnes
{
    public interface IbetRoulette: ICreate<betRoulette,int>
    {
        List<betRoulette> updateBetRoulette(int idRoulette, int idOpeningClosingRoulette);
    }
}
