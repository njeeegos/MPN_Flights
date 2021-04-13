using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPNFlights.Models
{
    public class CreateFlightViewModel
    {
        public Flight Flight { get; set; }

        [BindProperty]
        public List<Airport> Airports { get; set; }
        public int AirportFromId { get; set; }
        public int AirportToId { get; set; }
        public bool Disable { get; set; }

        public CreateFlightViewModel()
        {
            Airports = new List<Airport>();
        }

    }
}
