using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MPNFlights.Models
{
    public class AdminFlightsViewModel
    {
        public List<Flight> FlightsList;

        public AdminFlightsViewModel()
        {
            FlightsList = new List<Flight>();
        }
    }
}
