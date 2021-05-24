using RoulettePlay.Entities.Models;
using RoulettePlay.Services.Interfaces.Businnes.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoulettePlay.Services.Interfaces.Businnes
{
   public interface IopeningClousingRoulete : ICreate<openingClosingRoulette, int>, IReadbyId<openingClosingRoulette>
    {
         Task<bool> openingClosingRouleteUpdate(int idRoulette, int idOpeningClosingRoulette);
    }
}
