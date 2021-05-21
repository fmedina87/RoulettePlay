using RoulettePlay.Entities.Models;
using RoulettePlay.Services.DataBase;
using RoulettePlay.Services.Interfaces.Businnes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RoulettePlay.Services.Businnes
{
    public class betRouletteXNumberBusinnes: DBAcces, IbetRouletteXNumber
    {
        #region Create
        /// <summary>
        /// this method creates the association bet to bet number
        /// </summary>
        /// <param name="objRouletteXNumber">object whith the bet id and bet number</param>
        /// <returns>int value, indicate the identifier of the record</returns>

        public async Task<int> createAsync(betRouletteXNumber objRouletteXNumber)
        {
            int idBetRouletteXNumber = 0;
            try
            {
                string _Result = "";
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@idBetRoulette", objRouletteXNumber.idBetRoulette);
                lstParametros.Add("@BetRouletteColour", objRouletteXNumber.betRouletteNumber);
                lstParametros.Add("@createType", 1);
                _Result = await commandExecuteDBAsync("SP_BETROULETTEXCOLOR_CREATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                if (Convert.ToInt32(_Result) > 0)
                {
                    idBetRouletteXNumber = Convert.ToInt32(_Result);
                    SaveChange();
                }
                else
                {
                    throw new Exception("No se obtuvo un identificador valido para la apuesta por número.");
                }
            }
            catch (Exception ex)
            {
                DiscardChange();
                throw new Exception(string.Format("Se presentó un error  {0} al intentar crear la apuesta por número de la ruleta. ", ex.Message));
            }
            finally { Dispose(); }

            return idBetRouletteXNumber;
        }
        #endregion
    }
}
