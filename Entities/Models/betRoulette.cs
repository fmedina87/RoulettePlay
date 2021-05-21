using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoulettePlay.Entities.Models
{
    public class betRoulette
    {
        public int  idBetRoulette {get;set;}
        public int  idOpeningClosingRoulette { get; set; }
        public int  idRoulette { get; set; }
        public int betRouletteNumber { get; set; }
        public int betRouletteColourCode { get; set; }
        public string betRouletteColour { get; set; }
        public double betRouletteValue { get; set; }        
        public bool isWinner { get; set; }
        public bool paidValue { get; set; }
        public DateTime betCreationDate { get; set; }
        public string betUser { get; set; }
    }
}
