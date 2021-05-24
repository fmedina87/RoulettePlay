using RoulettePlay.Services.Interfaces.Businnes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace RoulettePlay.Services.Interfaces.DataBase
{
    public interface IDBAcces : IDisposable
    {
        public SqlConnection _context { get; set; }
        public SqlTransaction _transaction { get; set; }
        IServicesRepository _repository { get; set; }
        public void SaveChange();
        public void DiscardChange();
        
    }
}
