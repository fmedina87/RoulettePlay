using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoulettePlay.Entities.Models
{
    public class Roulette
    {
        public int idRoulette { get; set; }
        public bool rouletteState { get; set; }
        public DateTime rouletteCreationDate { get; set; }
    }
}
