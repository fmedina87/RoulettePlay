using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoulettePlay.Services.Interfaces.Businnes.Actions
{
    public interface IReadAll<T> where T : class
    {
        Task<T> ReadAllAsync();
    }
}
