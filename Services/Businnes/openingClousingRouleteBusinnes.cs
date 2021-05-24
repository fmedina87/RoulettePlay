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
    public class openingClousingRouleteBusinnes: DBCommand, IopeningClousingRoulete
    {
        private IDBAcces _dbAcces { get; }
        public openingClousingRouleteBusinnes(IDBAcces dbAccess)
        {
            _context = dbAccess._context;
            _transaction = dbAccess._transaction;
            _dbAcces = dbAccess;
        }
        #region Create
        /// <summary>
        /// this method is for create history opening and closing of the roulettes
        /// </summary>
        /// <param name="idRoulette">roulette identifier</param>
        /// <returns>int value identifier of this</returns>
        public async Task<int> createAsync(openingClosingRoulette objOpeningClosingRoulette)
        {
            int idOpeningClousingRoulete = 0;
            try
            {
                string _Result = "";
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@idRoulette", objOpeningClosingRoulette.idRoulette);
                lstParametros.Add("@createType", 1);
                _Result = await commandExecuteDBAsync("SP_OPENNINGCLOSINGROULETTE_CREATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                if (Convert.ToInt32(_Result) > 0)
                {
                    idOpeningClousingRoulete = Convert.ToInt32(_Result);
                }
                else
                {
                    throw new Exception("No se obtuvo un identificador válido.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error  {0} al intentar crear la apertura de la ruleta. ", ex.Message));
            }

            return idOpeningClousingRoulete;
        }
        #endregion
        #region Update
        /// <summary>
        /// this method is for update history opening and closing of the roulettes
        /// </summary>
        /// <param name="idRoulette">roulette identifier</param>
        /// <returns>bolean value indicates if update succesfull</returns>
        public async Task<bool> openingClosingRouleteUpdate(int idRoulette, int idOpeningClosingRoulette)
        {
            bool isUpdated = false;
            try
            {
                string _Result = "";
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@idRoulette", idRoulette);
                lstParametros.Add("@idOpeningClosingRoulette", idOpeningClosingRoulette);
                lstParametros.Add("@updateType", 1);               
                _Result = await commandExecuteDBAsync("SP_OPENNINGCLOSINGROULETTE_UPDATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                if (Convert.ToInt32(_Result) > 0)
                {
                    isUpdated = true;
                }                              
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error  {0} al intentar actualizar el cierre de la ruleta. ", ex.Message));
            }

            return isUpdated;
        }
        #endregion
        #region Read
        /// <summary>
        /// method to read the last roulette opening record
        /// </summary>
        /// <param name="idRoulette">Roulette identifier</param>
        /// <returns></returns>
        public async Task<openingClosingRoulette> ReadByIdAsync(int idRoulette)
        {
            openingClosingRoulette objRoulette = new openingClosingRoulette();
            try
            {
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@idRoulette", idRoulette);
                lstParametros.Add("@queryType", 1);
                objRoulette = Utilities.MapObjectInstance<openingClosingRoulette>(await commandExecuteDBAsync("SP_OPENINGCLOSINGROULETTE_READ", lstParametros)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al consultar la ruleta {1}", ex.Message, idRoulette));
            }
           
            return objRoulette;
        }
        #endregion
    }
}
