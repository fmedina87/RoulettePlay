using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RoulettePlay.Entities.Models;
namespace RoulettePlay.Services.Interfaces.Businnes.Actions
{
    public interface IReadbyId<T> where T : class
    {
        Task<T> ReadByIdAsync(int id);      
    }
}
