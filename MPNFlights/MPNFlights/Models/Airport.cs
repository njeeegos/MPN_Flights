using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MPNFlights.Models
{
    public class Airport
    { 
        public int id { get; set; }

        [Display(Name = "Skraćenica")]
        public string name { get; set; }

        [Display(Name = "Grad")]
        public string city { get; set; }

        [Display(Name = "Puno ime")]
        public string fullName { get; set; }

        [Display(Name = "Država")]
        public string country { get; set; }

    }
}
