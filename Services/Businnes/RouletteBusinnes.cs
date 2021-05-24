using RoulettePlay.Entities.Models;
using RoulettePlay.Services.DataBase;
using RoulettePlay.Services.Interfaces.Businnes;
using RoulettePlay.Services.Interfaces.Businnes.Actions;
using RoulettePlay.Services.Interfaces.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
namespace RoulettePlay.Services.Businnes
{
    public class rouletteBusinnes : DBCommand, IRoulette
    {
        private IDBAcces _dbAcces { get; }
        public rouletteBusinnes(IDBAcces dbAccess)
        {
            _context = dbAccess._context;
            _transaction = dbAccess._transaction;
            _dbAcces = dbAccess;
        }
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
                    _dbAcces.SaveChange();
                }
                else
                {
                    throw new Exception("No se obtuvo un identificador válido para la ruleta");
                }
            }
            catch (Exception ex)
            {

                throw new Exception(string.Format("Se presentó un error {0} al intentar crear la ruleta. ", ex.Message));
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
        public async Task<bool> RouletteOpen(int idRoulette)
        {
            bool isOpen = false;
            try
            {
                if (ValidateRouletteStatus(idRoulette))
                {
                    throw new Exception(string.Format("La ruleta {0} ya se encuentra abierta.", idRoulette));
                }
                else
                {
                    isOpen = await UpdateRoulette(idRoulette, true);
                    if (isOpen)
                    {
                        openingClosingRoulette objopeningClosingRoulette = new openingClosingRoulette();
                        objopeningClosingRoulette.idRoulette = idRoulette;
                        int Res = await _dbAcces._repository.openingClousingRoulete.createAsync(objopeningClosingRoulette);
                        _dbAcces.SaveChange();
                    }
                }
            }
            catch (Exception ex)
            {
                _dbAcces.DiscardChange();
                throw new Exception(string.Format("Se presentó un error {0} al intentar abrir la ruleta. ", ex.Message));
            }

            return isOpen;
        }
        /// <summary>
        /// this method is used for closing the roulette
        /// </summary>
        /// <param name="idRoulette">input parameter, is a roulette identifier for close</param>
        /// <returns>list of bet for roulette</returns>
        public async Task<List<betRoulette>> RouletteClose(int idRoulette)
        {
            List<betRoulette> lstbetRoulette = null;
            try
            {
                if (ValidateRouletteStatus(idRoulette))
                {
                    int idOpeningClosingRoulette = getOpenigHandle(idRoulette);

                    await _dbAcces._repository.openingClousingRoulete.openingClosingRouleteUpdate(idRoulette, idOpeningClosingRoulette);
                    lstbetRoulette = await _dbAcces._repository.BetRoulette.updateBetRoulette(idRoulette, idOpeningClosingRoulette);
                    bool isOpen = await UpdateRoulette(idRoulette, false);
                    if (isOpen)
                    {
                        _dbAcces.SaveChange();
                    }
                }
                else
                {
                    throw new Exception(string.Format("La ruleta {0} ya se encuentra cerrada.", idRoulette));
                }
            }
            catch (Exception ex)
            {
                _dbAcces.DiscardChange();
                throw new Exception(string.Format("Se presentó un error {0} al intentar cerrar la ruleta. ", ex.Message));
            }

            return lstbetRoulette;
        }
        /// <summary>
        /// Procedure for update roulette, this receive status fromo the others updates.
        /// </summary>
        /// <param name="idRoulette"></param>
        /// <param name="rouletteState"></param>
        /// <returns></returns>
        private async Task<bool> UpdateRoulette(int idRoulette, bool rouletteState)
        {
            bool isSuccesfull = false;
            try
            {
                string _Result = "";
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@idRoulette", idRoulette);
                lstParametros.Add("@rouletteState", rouletteState);
                lstParametros.Add("@updateType", 1);
                _Result = await commandExecuteDBAsync("SP_ROULETTE_UPDATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                if (Convert.ToInt32(_Result) > 0)
                {
                    isSuccesfull = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al consultar la ruleta {1}", ex.Message, idRoulette));
            }

            return objRoulette;
        }
        /// <summary>
        /// This function return the  list created roulettes 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Roulette>> ReadAllAsync()
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
        }

        #endregion        
        #endregion
        #region "Validations"
        /// <summary>
        /// procedure to validate if a roulette exists
        /// </summary>
        /// <param name="idRoulette">roulette identifier</param>
        /// <returns></returns>
        public bool ValidateExistance(int idRoulette)
        {
            bool isExists = false;
            try
            {
                Task<Roulette> objRoulette = null;
                objRoulette = ReadByIdAsync(idRoulette);
                if (objRoulette != null && objRoulette.Result != null && objRoulette.Result.idRoulette > 0)
                {
                    isExists = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isExists;
        }
        /// <summary>
        /// This method is used for validate the roluette  status 
        /// </summary>
        /// <param name="idRoulette">input parameter, is a roulette identifier for open</param>
        /// <returns>roulette status</returns>
        public bool ValidateRouletteStatus(int idRoulette)
        {
            bool isOpen = false;
            try
            {
                var objRoulette = ReadByIdAsync(idRoulette);
                if (!(objRoulette.Result is null) && objRoulette.Result.idRoulette > 0)
                {
                    isOpen = objRoulette.Result.rouletteState;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al validar esl estado de la ruleta. ", ex.Message));
            }

            return isOpen;
        }
        #endregion  
        #region Auxiliaries
        /// <summary>
        /// method to get the opening identifier for roulette
        /// </summary>
        /// <param name="idRoulette">identifier roulette</param>
        /// <returns></returns>
        public int getOpenigHandle(int idRoulette)
        {
            int idOpeningClosingRoulette = 0;
            try
            {
                var resul = _dbAcces._repository.openingClousingRoulete.ReadByIdAsync(idRoulette);
                if (resul != null && resul.Result != null && resul.Result.idOpeningClosingRoulette > 0)
                {
                    idOpeningClosingRoulette = resul.Result.idOpeningClosingRoulette;
                }
                else
                {
                    throw new Exception(string.Format("No se logró obtener el identificador de la apertura de la ruleta.", idRoulette));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idOpeningClosingRoulette;
        }
        #endregion
    }
}
