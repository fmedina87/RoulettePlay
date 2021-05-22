using System;
using System.Collections.Generic;
using System.Text;

namespace RoulettePlay.Services.Interfaces.DataBase
{
    public interface IDataBaseConnector 
    {
        void SaveChange();
        void DiscardChange();
        void Dispose();
        void openConnection();
    }
}
