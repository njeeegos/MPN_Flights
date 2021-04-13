using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MPNFlights.Models
{
    public class QueryFlightsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Od:")]
        [Required(ErrorMessage = "Neophodno je uneti grad odakle se leti!")]
        public string CityFrom { get; set; }

        [Display(Name = "Do:")]
        [Required(ErrorMessage = "Neophodno je uneti grad dokle se leti!")]
        public string CityTo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy.}")]
        [Display(Name = "Datum polaska:")]
        [Required(ErrorMessage = "Neophodno je uneti datum polaska! (U formatu dd.MM.yyyy.)")]
        public string DepartureDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy.}")]
        [Display(Name = "Datum povratka:")]
        public string ReturnDate { get; set; }

        [Display(Name = "Direktno ili sa presedanjem")]
        public bool direktnoIliSaPresedanjem { get; set; }

        [Display(Name = "U jednom smeru")]
        public bool jednosmernaKarta { get; set; }


        public IEnumerable<Flight> FlightsList;
        public IEnumerable<Flight> FlightsListBack;
        public IEnumerable<FlightSaPresedanjem> Presedanja;
        public IEnumerable<FlightSaPresedanjem> PresedanjaNazad;

        public QueryFlightsViewModel()
        {
            FlightsList = new List<Flight>();
            FlightsListBack = new List<Flight>();
            Presedanja = new List<FlightSaPresedanjem>();
            PresedanjaNazad = new List<FlightSaPresedanjem>();
        }

    }
}
