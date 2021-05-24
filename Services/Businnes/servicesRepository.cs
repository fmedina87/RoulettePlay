using RoulettePlay.Services.Interfaces.Businnes;
using RoulettePlay.Services.Interfaces.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace RoulettePlay.Services.Businnes
{
    public class servicesRepository : IServicesRepository
    {
        #region Interfaces
        public IbetRoulette BetRoulette { get; }
        public IopeningClousingRoulete openingClousingRoulete { get; }
        public IbetRouletteXColour betRouletteXColour { get; }
        public IbetRouletteXNumber betRouletteXNumber { get; }
        public IRoulette Roulette { get; }
        #endregion

        #region BusinnesImplementation
        public servicesRepository(IDBAcces dbAccess)
        {

            BetRoulette = new betRouletteBusinnes(dbAccess);
            Roulette = new rouletteBusinnes(dbAccess);
            openingClousingRoulete = new openingClousingRouleteBusinnes(dbAccess);
            betRouletteXColour = new betRouletteXColourBusinnes(dbAccess);
            betRouletteXNumber = new betRouletteXNumberBusinnes(dbAccess);
        }
        #endregion
    }
}
