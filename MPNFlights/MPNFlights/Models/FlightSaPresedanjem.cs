using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPNFlights.Models
{
    public class FlightSaPresedanjem
    {
        public Airport Od { get; set; }
        public Airport Preko { get; set; }
        public Airport Do { get; set; }
      
        public Flight LetOdPreko;
        public Flight LetPrekoDo;

        //public string VremePresedanja { get; set; }

    }
}
