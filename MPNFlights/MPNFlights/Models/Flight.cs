using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPNFlights.Models
{
    public class Flight
    {
        public int id { get; set; }
        public Airport From { get; set; }
        public Airport To { get; set; }
        public string DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public int SeatsAvailable { get; set; }
        public int TicketPrice { get; set; }

    }
}
