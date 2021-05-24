using System;
using System.Collections.Generic;
using System.Text;

namespace RoulettePlay.Services.Interfaces.Businnes
{
    public interface IServicesRepository
    {
        public IbetRoulette BetRoulette { get; }
        public IopeningClousingRoulete openingClousingRoulete { get; }
        public IbetRouletteXColour betRouletteXColour { get; }
        public IbetRouletteXNumber betRouletteXNumber { get; }
        public IRoulette Roulette { get; }        
    }
}
