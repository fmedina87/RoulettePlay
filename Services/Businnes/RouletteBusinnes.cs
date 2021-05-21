using RoulettePlay.Entities.Models;
using RoulettePlay.Services.DataBase;
using RoulettePlay.Services.Interfaces.Businnes;
using RoulettePlay.Services.Interfaces.Businnes.Actions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
namespace RoulettePlay.Services.Businnes
{
    public class RouletteBusinnes : DBAcces, IRoulette
    {
        public IbetRoulette _BetRoulette { get; }
        public IopeningClousingRoulete _openingClousingRoulete { get; }
        #region "Actions"
        #region Create
        /// <summary>
        ///  This method is used for create a new roulettes
        /// </summary>
        /// <returns>Id of a new roulette</returns>
        public async Task<int> createAsync(Roulette objRoulette)
        {
            int idRoulette = 0;
            try
            {
                string _Result = "";
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@createType", 1);
                _Result = await commandExecuteDBAsync("SP_ROULETTE_CREATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                if (Convert.ToInt32(_Result) > 0)
                {
                    idRoulette = Convert.ToInt32(_Result);
                    SaveChange();
                }
                else
                {
                    throw new Exception("No se obtuvo un identificador válido para la ruleta");
                }
            }
            catch (Exception ex)
            {
                DiscardChange();
                throw new Exception(string.Format("Se presentó un error {0} al intentar crear la ruleta. ", ex.Message));
            }
            finally
            {
                Dispose();
            }
            return idRoulette;
        }
        #endregion
        #region Update
        /// <summary>
        /// this method is used for opening a roulette
        /// </summary>
        /// <param name="idRoulette">input parameter, is a roulette identifier for open</param>
        /// <returns>boolean value, indicates status of opennign roulette</returns>
        public Task<bool> RouletteOpen(int idRoulette)
        {
            Task<bool> isOpen = null;
            try
            {
                if (ValidateRouletteStatus(idRoulette))
                {
                    throw new Exception(string.Format("La ruleta {0} ya se encuentra abierta.", idRoulette));
                }
                else
                {
                    isOpen = UpdateRoulette(idRoulette, true);
                    if (isOpen.Result)
                    {
                        openingClosingRoulette objopeningClosingRoulette = new openingClosingRoulette();
                        objopeningClosingRoulette.idRoulette = idRoulette;
                        int idobjopeningClosingRoulette = _openingClousingRoulete.createAsync(objopeningClosingRoulette).Result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al intentar abrir la ruleta. ", ex.Message));
            }

            return isOpen;
        }
        /// <summary>
        /// this method is used for closing the roulette
        /// </summary>
        /// <param name="idRoulette">input parameter, is a roulette identifier for close</param>
        /// <returns>list of bet for roulette</returns>
        public Task<bool> RouletteClose(int idRoulette)
        {
            Task<bool> isOpen = null;
            try
            {
                if (ValidateRouletteStatus(idRoulette))
                {
                    isOpen = UpdateRoulette(idRoulette, false);
                }
                else
                {
                    throw new Exception(string.Format("La ruleta {0} ya se encuentra cerrada.", idRoulette));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al intentar cerrar la ruleta. ", ex.Message));
            }
            return isOpen;
        }
        private async Task<bool> UpdateRoulette(int idRoulette, bool rouletteState)
        {
            bool isSuccesfull = false;
            try
            {
                string _Result = "";
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@idRoulette", idRoulette);
                lstParametros.Add("@rouletteState", rouletteState);
                lstParametros.Add("@@updateType", 1);
                _Result = await commandExecuteDBAsync("SP_ROULETTE_UPDATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                if (Convert.ToInt32(_Result) > 0)
                {
                    SaveChange();
                    isSuccesfull = true;
                }
            }
            catch (Exception ex)
            {
                DiscardChange();
                throw ex;
            }
            finally
            {
                Dispose();
            }

            return isSuccesfull;
        }
        #endregion
        #region Read
        /// <summary>
        /// this function return an object by idRoulette
        /// </summary>
        /// <param name="idRoulette">IdRoulette</param>
        /// <returns></returns>
        public async Task<Roulette> ReadByIdAsync(int idRoulette)
        {
            Roulette objRoulette = new Roulette();
            try
            {
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@idRoulette", idRoulette);
                lstParametros.Add("@queryType", 2);
                objRoulette = Utilities.MapObjectInstance<Roulette>(await commandExecuteDBAsync("SP_ROULETTE_READ", lstParametros)).FirstOrDefault();

                return objRoulette;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al consultar la ruleta {1}", ex.Message, idRoulette));
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// This function return the  list created roulettes 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Roulette>> ReadAllsync()
        {
            List<Roulette> lstRoulette = new List<Roulette>();
            try
            {
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@queryType", 1);
                lstRoulette = Utilities.MapObjectInstance<Roulette>(await commandExecuteDBAsync("SP_ROULETTE_READ", lstParametros));
                return lstRoulette;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al consultar las ruletas", ex.Message));
            }
            finally
            {
                Dispose();
            }
        }

        #endregion        
        #endregion
        #region "Validations"
        /// <summary>
        /// This method is used for validate the roluette  status 
        /// </summary>
        /// <param name="idRoulette">input parameter, is a roulette identifier for open</param>
        /// <returns>roulette status</returns>
        private bool ValidateRouletteStatus(int idRoulette)
        {
            bool isOpen = false;
            try
            {
                var objRoulette = ReadByIdAsync(idRoulette);
                if (!(objRoulette is null) && objRoulette.Result.idRoulette > 0)
                {
                    isOpen = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al validar esl estado de la ruleta. ", ex.Message));
            }

            return isOpen;
        }
        #endregion

    }
}
