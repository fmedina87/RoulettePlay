using Microsoft.Extensions.Configuration;
using RoulettePlay.Services.Businnes;
using RoulettePlay.Services.Interfaces.Businnes;
using RoulettePlay.Services.Interfaces.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace RoulettePlay.Services.DataBase
{
   public class DBAcces: IDBAcces
    {
        public SqlConnection _context { get; set; }
        public SqlTransaction _transaction { get; set; }
        public IServicesRepository _repository { get; set; }   
        public IConfiguration _Configuration { get; }
        public DBAcces(IConfiguration config)
        {
            try
            {
                _Configuration = config;
                 _context = openConnection();
                _transaction = _context.BeginTransaction();
                _repository = new servicesRepository(this);
            }
            catch (Exception)
            {
                throw;
            }
           
        }
        /// <summary>
        /// get the connection string from the api config
        /// </summary>
        /// <returns></returns>
        private string getConnectionString()
        {
            string connectionString = string.Empty;
            try
            {
                connectionString = _Configuration.GetConnectionString("SqlConnectionString");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Se presentó un problema al obtener la cadena de conexión. Por favor verique el archivo de configuración.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return connectionString;
        }
        /// <summary>
        /// Open connection
        /// </summary>
        private SqlConnection openConnection()
        {
            try
            {
                string connectionString = getConnectionString();
                var context = new SqlConnection(connectionString);
                context.Open();
                return context;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Dispose resources of conection DB
        /// </summary>
        public void Dispose()
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Commit transaction
        /// </summary>
        public void SaveChange()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// discard changes 
        /// </summary>
        public void DiscardChange()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                }
            }
            catch (Exception)
            {             
            }
        }
        ~DBAcces()
        {
            Dispose();
        }
    }
}
