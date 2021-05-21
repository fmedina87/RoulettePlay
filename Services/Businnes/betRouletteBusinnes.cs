using RoulettePlay.Entities.Models;
using RoulettePlay.Services.DataBase;
using RoulettePlay.Services.Interfaces.Businnes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RoulettePlay.Services.Businnes
{
    public class betRouletteBusinnes : DBAcces, IbetRoulette
    {
        IbetRouletteXColour _betRoulettexColour { get; }
        IbetRouletteXNumber _betRouletteXNumber { get; }
        #region Update
        /// <summary>
        /// procedure to call the various update methods
        /// </summary>
        /// <param name="idRoulette">Roultte identifier</param>
        /// <param name="idOpeningClosingRoulette">Openning roulette identifier</param>
        /// <returns>list of betRoulettes</returns>
        public List<betRoulette> updateBetRoulette(int idRoulette, int idOpeningClosingRoulette)
        {
            List<betRoulette> lstRoulette = new List<betRoulette>();
            try
            {
                bool isSuccesfull = false;
                int betRouletteNumber = ObtainWinnerNumber();
                isSuccesfull = BetRouletteWinnersByNumber(idRoulette, betRouletteNumber);
                isSuccesfull = updateBetRouletteWinnersByColour(idRoulette, betRouletteNumber);
                isSuccesfull = updateBetRouletteLossers(idRoulette);
                lstRoulette = ReadBetRoulettebyIdOpeningClosingRoulette(idOpeningClosingRoulette).Result;
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
        private bool BetRouletteWinnersByNumber(int idRoulette, int betRouletteNumber)
        {
            bool isSuccesfull = false;
            try
            {
                double amountPaymentValue = ObtainBetAmountValuexNumber();
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@amountPaymentValue", amountPaymentValue);
                lstParametros.Add("@betRouletteNumber", betRouletteNumber);
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
        private bool updateBetRouletteWinnersByColour(int idRoulette, int betRouletteNumber)
        {
            bool isSuccesfull = false;
            try
            {
                double amountPaymentValue = ObtainBetAmountValuexColour();
                int BetRouletteColour = ObtainWinnerColorCode(betRouletteNumber);
                Dictionary<string, object> lstParametros = new Dictionary<string, object>();
                lstParametros.Add("@amountPaymentValue", amountPaymentValue);
                lstParametros.Add("@BetRouletteColour", BetRouletteColour);
                lstParametros.Add("@idRoulette", idRoulette);
                lstParametros.Add("@updateType", 2);
                isSuccesfull = updateBetRoulette(lstParametros).Result;
            }
            catch (Exception ex)
            {
                DiscardChange();
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
                DiscardChange();
                throw new Exception(string.Format("Se presentó un error {0} al intentar actualizar  la apuestas perdedoras. ", ex.Message));
            }
            finally
            {
                Dispose();
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
        #region Create
        public async Task<int> createAsync(betRoulette objbetRoulette)
        {
            int idBetRoulette = 0;
            try
            {
                if (ObjectValidate(objbetRoulette))
                {
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
                        SaveChange();
                        if (objbetRoulette.betRouletteNumber > 0)
                        {
                            betRouletteXNumber objBetRouletteNumber = new betRouletteXNumber();
                            objBetRouletteNumber.betRouletteNumber = objbetRoulette.betRouletteNumber;
                            objBetRouletteNumber.idBetRoulette = idBetRoulette;
                            int idBetRouletteXNumber = await _betRouletteXNumber.createAsync(objBetRouletteNumber);
                        }
                        else if (string.IsNullOrEmpty(objbetRoulette.betRouletteColour))
                        {
                            betRouletteXColour objBetRouletteColour = new betRouletteXColour();
                            objBetRouletteColour.BetRouletteColour = ObtainBetColorCode(objbetRoulette.betRouletteColour);
                            objBetRouletteColour.idBetRoulette = idBetRoulette;
                            int idBetRoulettexColour = await _betRoulettexColour.createAsync(objBetRouletteColour);
                        }
                    }
                    else
                    {
                        throw new Exception("No se obtuvo un identificador válido para la apuesta.");
                    }
                }
            }
            catch (Exception ex)
            {
                DiscardChange();
                throw new Exception(string.Format("Se presentó un error {0} al intentar crear la apuesta. ", ex.Message));
            }
            finally
            {
                Dispose();
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
            finally
            {
                Dispose();
            }
            return lstBetRoulette;
        }
        #endregion
        #region Validaciones
        public bool ObjectValidate(betRoulette objbetRoultte)
        {
            bool isSuccesfull = true;
            try
            {
                if (objbetRoultte.betRouletteValue > 10000)
                {
                    throw new Exception("El valor de la apuesta no puede ser mayor de 10.000, por favor intente nuevamente.");
                }
                else if (objbetRoultte.betRouletteValue <= 0)
                {
                    throw new Exception("El valor de la apuesta debe ser mayor de 0, por favor intente nuevamente.");
                }
                else if (objbetRoultte.betRouletteNumber < 0 && objbetRoultte.betRouletteNumber > 36)
                {
                    throw new Exception("El número apostado debe estar entre  0 a 36, por favor intente nuevamente.");
                }
                else if (!objbetRoultte.betRouletteColour.ToUpper().Equals("ROJO".ToUpper()) && !objbetRoultte.betRouletteColour.ToUpper().Equals("NEGRO".ToUpper()))
                {
                    throw new Exception(string.Format("El color {0}  no es valido, por favor intente nuevamente.", objbetRoultte.betRouletteColour));
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
        public int ObtainBetColorCode(string betRouletteColourName)
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
                    BetRouletteColour = 1;
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
        /// this method read the resources file obtaining the amount value for calculate the bet payment  for number
        /// </summary>
        /// <returns>double value indicate the amount of bet payment </returns>
        public double ObtainBetAmountValuexNumber()
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
        /// this method read the resources file obtaining the amount value for calculate the bet payment  for color
        /// </summary>
        /// <returns>double value indicate the amount of bet payment </returns>
        public double ObtainBetAmountValuexColour()
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
        private int ObtainWinnerNumber()
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
        private int ObtainWinnerColorCode(int winnerNumber)
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
