using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RoulettePlay.Entities.Models;
using RoulettePlay.Services.Interfaces.Businnes.Actions;
namespace RoulettePlay.Services.Interfaces.Businnes
{
    public interface IRoulette : ICreate<Roulette, int>, IReadbyId<Roulette>, IReadAll<List<Roulette>>
    {
        Task<bool> RouletteOpen(int idRoulette);
        Task<List<betRoulette>> RouletteClose(int idRoulette);
        int getOpenigHandle(int idRoulette);
        bool ValidateExistance(int idRoulette);
        bool ValidateRouletteStatus(int idRoulette);
    }
}
