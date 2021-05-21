using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoulettePlay.Services.Interfaces.Businnes.Actions
{
    public interface ICreate<T, Y> where T : class
    {   
        Task<Y> createAsync(T t);
    }
}
