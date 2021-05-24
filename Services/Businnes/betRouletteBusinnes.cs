using RoulettePlay.Entities.Models;
using RoulettePlay.Services.DataBase;
using RoulettePlay.Services.Interfaces.Businnes;
using RoulettePlay.Services.Interfaces.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Resources;
using System.Threading.Tasks;

namespace RoulettePlay.Services.Businnes
{
    public class betRouletteBusinnes : DBCommand, IbetRoulette
    {
        private IDBAcces _dbAcces;
        public betRouletteBusinnes(IDBAcces dbAccess)
        {
            _context = dbAccess._context;
            _transaction = dbAccess._transaction;
            _dbAcces = dbAccess;
        }
        #region Update
        /// <summary>
        /// procedure to call the various update methods
        /// </summary>
        /// <param name="idRoulette">Roultte identifier</param>
        /// <param name="idOpeningClosingRoulette">Openning roulette identifier</param>
        /// <returns>list of betRoulettes</returns>
        public async Task<List<betRoulette>> updateBetRoulette(int idRoulette, int idOpeningClosingRoulette)
        {
            List<betRoulette> lstRoulette = null;
            try
            {
                bool isSuccesfull = false;
                int betNumberWinner = getWinnerNumber();
                isSuccesfull = BetRouletteWinnersByNumber(idRoulette, betNumberWinner);
                isSuccesfull = await updateBetRouletteWinnersByColour(idRoulette, betNumberWinner);
                isSuccesfull = updateBetRouletteLossers(idRoulette);
                lstRoulette = await ReadBetRoulettebyIdOpeningClosingRoulette(idOpeningClosingRoulette);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstRoulette;
        }
        /// <summary>
        /// procedure to update winning bets by number
        /// </summary>
        /// <param name="idRoulette">identifier of roulete</param>
        /// <param name="betRouletteNumber">Winner bet number</param>
        /// <returns>Boolean value, indicates successful or failed process</returns>
        private bool BetRouletteWinnersByNumber(int idRoulette, int betNumberWinner)
        {
            bool isSuccesfull = false;
            try
            {
                double amountPaymentValue = getBetAmountValuexNumber();
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@amountPaymentValue", amountPaymentValue);
                lstParametros.Add("@betNumberWinner", betNumberWinner);
                lstParametros.Add("@idRoulette", idRoulette);
                lstParametros.Add("@updateType", 1);
                isSuccesfull = updateBetRoulette(lstParametros).Result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al intentar actualizar  la apuesta por número. ", ex.Message));
            }

            return isSuccesfull;
        }
        /// <summary>
        /// procedure to update winning bets by colour
        /// </summary>
        /// <param name="idRoulette">identifier of roulete</param>
        /// <param name="betRouletteColourName">Winner bet number</param>
        /// <returns>Boolean value, indicates successful or failed process</returns>
        private async Task<bool> updateBetRouletteWinnersByColour(int idRoulette, int betRouletteNumber)
        {
            bool isSuccesfull = false;
            try
            {
                double amountPaymentValue = getBetAmountValuexColour();
                int BetRouletteColourCode = getWinnerColorCode(betRouletteNumber);
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@amountPaymentValue", amountPaymentValue);
                lstParametros.Add("@BetRouletteColourCode", BetRouletteColourCode);
                lstParametros.Add("@idRoulette", idRoulette);
                lstParametros.Add("@updateType", 2);
                isSuccesfull = await updateBetRoulette(lstParametros);
            }
            catch (Exception ex)
            {
                _dbAcces.DiscardChange();
                throw new Exception(string.Format("Se presentó un error {0} al intentar actualizar  la apuesta por color. ", ex.Message));
            }

            return isSuccesfull;
        }

        /// <summary>
        /// update bet roulette lossers
        /// </summary>
        /// <param name="idRoulette">identifier of roulete</param>      
        /// <returns>Boolean value, indicates successful or failed process</returns>
        private bool updateBetRouletteLossers(int idRoulette)
        {
            bool isSuccesfull;
            try
            {
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@idRoulette", idRoulette);
                lstParametros.Add("@updateType", 3);
                isSuccesfull = updateBetRoulette(lstParametros).Result;
            }
            catch (Exception ex)
            {
                _dbAcces.DiscardChange();
                throw new Exception(string.Format("Se presentó un error {0} al intentar actualizar  la apuestas perdedoras. ", ex.Message));
            }

            return isSuccesfull;
        }
        /// <summary>
        /// method to update the data of the roulette bets
        /// </summary>
        /// <param name="lstParametros">inputs parameters</param>
        /// <returns></returns>
        private async Task<bool> updateBetRoulette(Dictionary<string, object> lstParametros)
        {
            bool isSuccesfull = false;
            try
            {
                string _Result = string.Empty;
                _Result = await commandExecuteDBAsync("SP_BETROULETTE_UPDATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                if (Convert.ToInt32(_Result) > 0)
                {                    
                    isSuccesfull = true;
                }
            }
            catch (Exception ex)
            {
                _dbAcces.DiscardChange();
                throw ex;
            }

            return isSuccesfull;
        }
        #endregion
        #region Create
        /// <summary>
        /// procedure to enter a bet
        /// </summary>
        /// <param name="objbetRoulette">object with the bet information</param>
        /// <returns></returns>
        public async Task<int> createAsync(betRoulette objbetRoulette)
        {
            int idBetRoulette = 0;
            try
            {
                if (ObjectValidate(objbetRoulette))
                {
                    if (_dbAcces._repository.Roulette.ValidateRouletteStatus(objbetRoulette.idRoulette))
                    {
                        if (!_dbAcces._repository.Roulette.ValidateExistance(objbetRoulette.idRoulette))
                        {
                            throw new Exception("No se encontró una ruleta creada con el identificador asociado. Por favor valide e intente nuevamente.");
                        }
                        objbetRoulette.idOpeningClosingRoulette = _dbAcces._repository.Roulette.getOpenigHandle(objbetRoulette.idRoulette);
                        Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                        lstParametros.Add("@idOpeningClosingRoulette", objbetRoulette.idOpeningClosingRoulette);
                        lstParametros.Add("@betRouletteValue", objbetRoulette.betRouletteValue);
                        lstParametros.Add("@betUser", objbetRoulette.betUser);
                        lstParametros.Add("@createType", 1);
                        string _Result = string.Empty;
                        _Result = await commandExecuteDBAsync("SP_BETROULETTE_CREATE", lstParametros, new SqlParameter() { ParameterName = "@Result", Value = _Result });
                        if (Convert.ToInt32(_Result) > 0)
                        {
                            idBetRoulette = Convert.ToInt32(_Result);
                            if (!string.IsNullOrEmpty(objbetRoulette.betRouletteColour))
                            {

                                betRouletteXColour objBetRouletteColour = new betRouletteXColour();
                                objBetRouletteColour.BetRouletteColourCode = getBetColorCode(objbetRoulette.betRouletteColour);
                                objBetRouletteColour.idBetRoulette = idBetRoulette;
                                int idBetRoulettexColour = await _dbAcces._repository.betRouletteXColour.createAsync(objBetRouletteColour);
                            }
                            else
                            {
                                betRouletteXNumber objBetRouletteNumber = new betRouletteXNumber();
                                objBetRouletteNumber.betRouletteNumber = objbetRoulette.betRouletteNumber;
                                objBetRouletteNumber.idBetRoulette = idBetRoulette;
                                int idBetRouletteXNumber = await _dbAcces._repository.betRouletteXNumber.createAsync(objBetRouletteNumber);

                            }
                            _dbAcces.SaveChange();
                        }
                        else
                        {
                            throw new Exception("No se obtuvo un identificador válido para la apuesta.");

                        }
                    }
                    else
                    {
                        throw new Exception("La ruleta se encuentra cerrada, no se puede realizar la apuesta. Por favor valide e intente nuevamente.");
                    }

                }
            }
            catch (Exception ex)
            {
                _dbAcces.DiscardChange();
                throw new Exception(string.Format("Se presentó un error {0} al intentar crear la apuesta. ", ex.Message));
            }

            return idBetRoulette;
        }
        #endregion
        #region Read
        /// <summary>
        /// Procedure that returns the bet records associated with the last roulette game
        /// </summary>
        /// <param name="idRoulette"></param>
        /// <returns></returns>
        private async Task<List<betRoulette>> ReadBetRoulettebyIdOpeningClosingRoulette(int idOpeningClosingRoulette)
        {
            List<betRoulette> lstBetRoulette = new List<betRoulette>();
            try
            {
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@idOpeningClosingRoulette", idOpeningClosingRoulette);
                lstParametros.Add("@queryType", 1);
                lstBetRoulette = Utilities.MapObjectInstance<betRoulette>(await commandExecuteDBAsync("SP_BETROULETTE_READ", lstParametros));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al consultar las apuestas para la ruleta. ", ex.Message));
            }

            return lstBetRoulette;
        }
        #endregion
        #region Validaciones
        public bool ObjectValidate(betRoulette objbetRoulette)
        {
            bool isSuccesfull = true;
            try
            {
                bool betByColour = false;
                if (!string.IsNullOrEmpty(objbetRoulette.betRouletteColour))
                {
                    betByColour = true;
                }
                if (objbetRoulette.idRoulette <= 0)
                {
                    throw new Exception("El identificador de la ruleta no es valido, por favor intente nuevamente.");
                }
                else if (objbetRoulette.betRouletteValue > 10000)
                {
                    throw new Exception("El valor de la apuesta no puede ser mayor de 10.000, por favor intente nuevamente.");
                }
                else if (objbetRoulette.betRouletteValue <= 0)
                {
                    throw new Exception("El valor de la apuesta debe ser mayor de 0, por favor intente nuevamente.");
                }
                else if (!betByColour && (objbetRoulette.betRouletteNumber < 0 || objbetRoulette.betRouletteNumber > 36))
                {
                    throw new Exception("El número apostado debe estar entre  0 a 36, por favor intente nuevamente.");
                }
                else if (betByColour && !objbetRoulette.betRouletteColour.ToUpper().Equals("ROJO".ToUpper()) && !objbetRoulette.betRouletteColour.ToUpper().Equals("NEGRO".ToUpper()))
                {
                    throw new Exception(string.Format("El color {0}  no es valido, por favor intente nuevamente.", objbetRoulette.betRouletteColour));
                }
                if (!_dbAcces._repository.Roulette.ValidateExistance(objbetRoulette.idRoulette))
                {
                    throw new Exception("No se encontró una ruleta creada con el identificador asociado. Por favor valide e intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccesfull;
        }

        #endregion
        #region Auxiliaries
        /// <summary>
        /// procedure that returns the bet color code
        /// </summary>
        /// <param name="betRouletteColourName">name of color for the bet</param>
        /// <returns></returns>
        public int getBetColorCode(string betRouletteColourName)
        {
            int BetRouletteColour = 0;
            try
            {
                if (betRouletteColourName.ToUpper().Equals("rojo".ToUpper()))
                {
                    BetRouletteColour = 1;
                }
                else if (betRouletteColourName.ToUpper().Equals("negro".ToUpper()))
                {
                    BetRouletteColour = 2;
                }
                else
                {
                    throw new Exception("El color seleccionado no es valido.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return BetRouletteColour;
        }
        /// <summary>
        /// this method read the resources file get the amount value for calculate the bet payment  for number
        /// </summary>
        /// <returns>double value indicate the amount of bet payment </returns>
        public double getBetAmountValuexNumber()
        {
            double betAmountValue = 0;
            try
            {
                betAmountValue = Double.Parse(Environment.GetEnvironmentVariable("amountPaymentNumber"));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error  {0} al obtener el monto para calcular el pago de la apuesta por número. ", ex.Message));
            }

            return betAmountValue;
        }
        /// <summary>
        /// this method read the resources file get the amount value for calculate the bet payment  for color
        /// </summary>
        /// <returns>double value indicate the amount of bet payment </returns>
        public double getBetAmountValuexColour()
        {
            double betAmountValue = 0;
            try
            {
                betAmountValue = Double.Parse(Environment.GetEnvironmentVariable("amountPaymentColour"));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error  {0} al obtener el monto para calcular el pago de la apuesta por color. ", ex.Message));
            }

            return betAmountValue;
        }
        /// <summary>
        /// this method determinate winner number
        /// </summary>        
        /// <returns>winner colour number</returns>
        private int getWinnerNumber()
        {
            int winnerNumber = 0;
            try
            {
                Random rnd = new Random();
                winnerNumber = rnd.Next(0, 36);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al obtener el número ganador de la ruleta. ", ex.Message));
            }
            return winnerNumber;
        }
        /// <summary>
        /// this method determinate winner colour code
        /// </summary>
        /// <param name="winnerNumber">the number winner</param>
        /// <returns>winner colour code</returns>
        private int getWinnerColorCode(int winnerNumber)
        {
            int ColorCode = 2;
            try
            {
                if (winnerNumber % 2 == 0)
                {
                    ColorCode = 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Se presentó un error {0} al obtener el código del color ganador de la ruleta. ", ex.Message));
            }
            return ColorCode;
        }
        #endregion
    }
}
