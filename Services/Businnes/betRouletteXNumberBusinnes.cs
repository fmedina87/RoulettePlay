using RoulettePlay.Entities.Models;
using RoulettePlay.Services.DataBase;
using RoulettePlay.Services.Interfaces.Businnes;
using RoulettePlay.Services.Interfaces.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RoulettePlay.Services.Businnes
{
    public class betRouletteXNumberBusinnes: DBCommand, IbetRouletteXNumber
    {
        private IDBAcces _dbAcces;
        public betRouletteXNumberBusinnes(IDBAcces dbAccess)
        {
            _context = dbAccess._context;
            _transaction = dbAccess._transaction;
            _dbAcces = dbAccess;
        }
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
                lstParametros.Add("@betRouletteNumber", objRouletteXNumber.betRouletteNumber);
                lstParametros.Add("@createType", 1);
                _Result = await commandExecuteDBAsync("SP_BETROULETTEXNUMBER_CREATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                if (Convert.ToInt32(_Result) > 0)
                {
                    idBetRouletteXNumber = Convert.ToInt32(_Result);
                }
                else
                {
                    throw new Exception("No se obtuvo un identificador valido para la apuesta por número.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error  {0} al intentar crear la apuesta por número de la ruleta. ", ex.Message));
            }

            return idBetRouletteXNumber;
        }
        #endregion
    }
}
