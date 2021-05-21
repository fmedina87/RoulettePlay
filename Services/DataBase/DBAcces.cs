﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RoulettePlay.Services.DataBase
{
    public abstract class DBAcces
    {

        private SqlConnection _context { get; set; }
        private SqlTransaction _transaction { get; set; }        
       
        private void openConnection()
        {
            var connectionString = @"Server=localhost\SQLEXPRESS;Database=ROULETTETEST;Trusted_Connection=True;";
            _context= new SqlConnection(connectionString);
            _context.Open();
            _transaction = _context.BeginTransaction();
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
            if (_transaction != null)
            {
                _context.Close();
                _context.Dispose();
            }            
        }

        public void SaveChange()
        {
            _transaction.Commit();            
        }
        public void DiscardChange()
        {
            _transaction.Rollback();
        }
        /// <summary>
        /// function used for return result value for update or create
        /// </summary>
        /// <param name="spName">stored procedure name</param>
        /// <param name="lstInputParameters">store procedure parameter</param>
        /// <param name="outPutParameter">out put parameter</param>
        /// <returns></returns>
        public async Task<string> commandExecuteDBAsync(string spName, Dictionary<string, object> lstInputParameters, SqlParameter outPutParameter)
        {
            string valorParametroSalida = string.Empty;
            try
            {
                openConnection();
                SqlCommand comando = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = spName, Connection = _context, Transaction = _transaction };
                if (lstInputParameters.Count > 0)
                {
                    foreach (var item in lstInputParameters)
                    {
                        comando.Parameters.Add(new SqlParameter() { ParameterName = item.Key, Value = item.Value, IsNullable = true });
                    }
                }
                outPutParameter.Direction = ParameterDirection.Output;
                outPutParameter.IsNullable = true;
                comando.Parameters.Add(outPutParameter);                
                try
                {
                    var reader = await comando.ExecuteNonQueryAsync();
                    valorParametroSalida = Convert.ToString(comando.Parameters[outPutParameter.ParameterName].Value);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }                
            }
            catch (Exception ex)
            { throw ex; }
            return valorParametroSalida;
        }
        /// <summary>
        /// function used for return result value for update or create
        /// </summary>
        /// <param name="spName">stored procedure name</param>
        /// <param name="lstInputParameters">store procedure parameter</param>
        public async Task<DataTable> commandExecuteDBAsync(string spName, Dictionary<string, object> lstInputParameters)
        {
            var dt = new DataTable();
            try
            {
                openConnection();
                SqlCommand comando = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = spName, Connection = _context, Transaction = _transaction };
                if (lstInputParameters.Count > 0)
                {
                    foreach (var item in lstInputParameters)
                    {
                        comando.Parameters.Add(new SqlParameter() { ParameterName = item.Key, Value = item.Value, IsNullable = true });
                    }
                }
                var reader = await comando.ExecuteReaderAsync();
                dt.Load(reader);                
                return dt;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
