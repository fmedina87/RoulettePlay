using System;
using System.Collections.Generic;
using System.Text;

namespace RoulettePlay.Entities.Models
{
    public class openingClosingRoulette
    {
        public int idOpeningClosingRoulette { set; get; }
        public int idRoulette { set; get; }
        public DateTime dateOpeningRoulette { set; get; }
        public DateTime dateClosingRoulette { set; get; }
    }
}
