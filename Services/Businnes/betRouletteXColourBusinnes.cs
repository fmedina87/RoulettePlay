using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RoulettePlay.Entities.Models;
using RoulettePlay.Services.DataBase;
using RoulettePlay.Services.Interfaces.Businnes;
using RoulettePlay.Services.Interfaces.DataBase;

namespace RoulettePlay.Services.Businnes
{
    public class betRouletteXColourBusinnes : DBCommand, IbetRouletteXColour
    {
        private IDBAcces _dbAcces;
        public betRouletteXColourBusinnes(IDBAcces dbAccess)
        {
            _context = dbAccess._context;
            _transaction = dbAccess._transaction;
            _dbAcces = dbAccess;
        }
        #region Create
        /// <summary>
        /// this method creates the association bet to bet number
        /// </summary>
        /// <param name="objRouletteXColour">object whith the bet id and bet number</param>
        /// <returns>int value, indicate the identifier of the record</returns>
        public async Task<int> createAsync(betRouletteXColour objRouletteXColour)
        {
            int idBetRouletteXNumber = 0;
            try
            {
                string _Result = "";
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();                
                lstParametros.Add("@idBetRoulette", objRouletteXColour.idBetRoulette);
                lstParametros.Add("@BetRouletteColourCode", objRouletteXColour.BetRouletteColourCode);
                lstParametros.Add("@createType", 1);
                _Result = await commandExecuteDBAsync("SP_BETROULETTEXCOLOR_CREATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                if (Convert.ToInt32(_Result) > 0)
                {
                    idBetRouletteXNumber = Convert.ToInt32(_Result);
                }
                else
                {
                    throw new Exception("No se obtuvo un identificador valido para la apuesta por color.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error  {0} al intentar crear la apuesta por color de la ruleta. ", ex.Message));
            }

            return idBetRouletteXNumber;
        }
        #endregion
    }
}
