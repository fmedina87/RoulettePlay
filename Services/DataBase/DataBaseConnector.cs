using RoulettePlay.Services.Interfaces.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace RoulettePlay.Services.DataBase
{
    class DataBaseConnector: IDataBaseConnector
    {
        private SqlConnection _context { get; set; }
        private SqlTransaction _transaction { get; set; }

        public void openConnection()
        {
            var connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=ROULETTETEST;Persist Security Info=True;User ID=testuser;Password=T3stUs3r;";            
            _context = new SqlConnection();
            _context.ConnectionString = connectionString;
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
    }
}
