using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MPNFlights.Models;
using Neo4jClient;
using Neo4jClient.Cypher;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MPNFlights.Controllers
{
    public class FlightsController : Controller
    {
        private readonly GraphClient client;

        public FlightsController()
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "boskovic");
            try
            {
                client.Connect();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        // GET: FlightsController
        public ActionResult Index(QueryFlightsViewModel model)
        {
            if (model != null)
                model = new QueryFlightsViewModel();
            return View();
        }
        public ActionResult Admin(AdminFlightsViewModel model)
        {
            if (!ModelState.IsValid)
            {

            }
            else
            {
                var query = client.Cypher
                    .Match("(n:Airport)-[rel:FLIGHT_TO]->(m:Airport)")
                    .Return((n, rel, m) => new
                    {
                        Od = n.As<Airport>(),
                        Let = rel.As<Flight>(),
                        Do = m.As<Airport>()
                    }).Results.ToList();

                var flights = new List<Flight>(query.Count);


                foreach (var item in query)
                {
                    Flight f = item.Let;
                    f.From = item.Od;
                    f.To = item.Do;
                    flights.Add(f);

                }
                model.FlightsList = flights;
            }


            return View("Admin", model);
        }

        // GET: FlightsController/Details/5
        public ActionResult Details(int id)
        {
            var query = client.Cypher
                  .Match("(n:Airport)")
                  .Where((Airport n) => n.id == id)
                  .Return((n) => new { airport = n.As<Airport>() }).Results;
            Airport a = new Airport();
            foreach (var item in query)
            {
                a = item.airport;
            }
            return View("Details", a);
        }

        // GET: FlightsController/Create
        public ActionResult Create()
        {
            string airport = "";

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("airport", airport);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport) RETURN n", queryDict, CypherResultMode.Set);
            var airports = ((IRawGraphClient)client).ExecuteGetCypherResults<Airport>(query).ToList();

            CreateFlightViewModel viewModel = new CreateFlightViewModel()
            {
                Airports = airports
            };

            return View(viewModel);
        }

        // POST: FlightsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FlightsController/Edit/5
        public ActionResult Edit(int id)
        {
            var query = client.Cypher
                .Match("(n:Airport)-[rel:FLIGHT_TO]->(m:Airport)")
                .Where((Flight rel) => rel.id == id)
                .Return((n, rel, m) => new
                {
                    Od = n.As<Airport>(),
                    Let = rel.As<Flight>(),
                    Do = m.As<Airport>()
                }).Results.SingleOrDefault();


            CreateFlightViewModel viewModel = new CreateFlightViewModel();

            viewModel.Flight = query.Let;
            viewModel.AirportFromId = query.Od.id;
            viewModel.AirportToId = query.Do.id;
            viewModel.Airports.Add(query.Od);
            viewModel.Airports.Add(query.Do);
            viewModel.Disable = true;

            return View("Create", viewModel);
           
        }

        // POST: FlightsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Save(CreateFlightViewModel model)
        {
            int flightId = model.Flight.id;
            bool disable = model.Disable;
            int airportFrom = model.AirportFromId;
            int airportTo = model.AirportToId;
            string departureDate = model.Flight.DepartureDate;
            string departureTime = model.Flight.DepartureTime;
            string arrivalDate = model.Flight.ArrivalDate;
            string arrivalTime = model.Flight.ArrivalTime;
            int seatsAvailable = model.Flight.SeatsAvailable;
            int ticketPrice = model.Flight.TicketPrice;
            
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("airportFrom", airportFrom);
            queryDict.Add("airportTo", airportTo);
            queryDict.Add("departureDate", departureDate);
            queryDict.Add("departureTime", departureTime);
            queryDict.Add("arrivalDate", arrivalDate);
            queryDict.Add("arrivalTime", arrivalTime);
            queryDict.Add("seatsAvailable", seatsAvailable);
            queryDict.Add("ticketPrice", ticketPrice);
            queryDict.Add("flightId", flightId);

            var queryMax = new Neo4jClient.Cypher.CypherQuery("MATCH ()-[n:FLIGHT_TO]->() RETURN max(n.id)", queryDict, CypherResultMode.Set);
            String maxIdObj = ((IRawGraphClient)client).ExecuteGetCypherResults<String>(queryMax).SingleOrDefault();
            int maxId;
            if (maxIdObj == null)
                maxId = 0;
            else
                maxId = Int32.Parse(maxIdObj);

            var queryFrom = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport) WHERE n.id = {airportFrom} RETURN n",
                queryDict, CypherResultMode.Set);
            Airport apFrom = ((IRawGraphClient)client).ExecuteGetCypherResults<Airport>(queryFrom).SingleOrDefault();

            var queryTo = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport) WHERE n.id = {airportTo} RETURN n",
                queryDict, CypherResultMode.Set);
            Airport apTo = ((IRawGraphClient)client).ExecuteGetCypherResults<Airport>(queryTo).SingleOrDefault();

            model.Flight.From = apFrom;
            model.Flight.To = apTo;

            QueryFlightsViewModel viewModel = new QueryFlightsViewModel();
            var createQuery = new Neo4jClient.Cypher.CypherQuery("", queryDict, CypherResultMode.Set);

            if (disable == false)
            {
                createQuery = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport), (m:Airport) WHERE "
                                                                + "n.id = {airportFrom} AND m.id = {airportTo} "
                                                                + "CREATE (n)-[rel:FLIGHT_TO {id: " + ++maxId
                                                                + " , DepartureDate: '" + departureDate
                                                                + "', DepartureTime: '" + departureTime
                                                                + "', ArrivalDate: '" + arrivalDate
                                                                + "', ArrivalTime: '" + arrivalTime
                                                                + "', SeatsAvailable: " + seatsAvailable
                                                                + ", TicketPrice: " + ticketPrice
                                                                + "}]->(m) RETURN rel", queryDict, CypherResultMode.Set);

            }
            else
            {


                createQuery = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport)-[rel:FLIGHT_TO]->(m:Airport) WHERE "
                                                                + "rel.id = {flightId} "
                                                                + "SET rel.DepartureDate = \"" + departureDate
                                                                + "\", rel.DepartureTime = \"" + departureTime
                                                                + "\", rel.ArrivalDate = \"" + arrivalDate
                                                                + "\", rel.ArrivalTime = \"" + arrivalTime
                                                                + "\", rel.SeatsAvailable = " + seatsAvailable
                                                                + ", rel.TicketPrice = " + ticketPrice +
                                                                " RETURN rel", queryDict, CypherResultMode.Set);
            }
                List<Flight> create = ((IRawGraphClient)client).ExecuteGetCypherResults<Flight>(createQuery).ToList();



                var query1 = client.Cypher
                        .Match("(n:Airport)-[rel:FLIGHT_TO]->(m:Airport)")
                        .Return((n, rel, m) => new
                        {
                            Od = n.As<Airport>(),
                            Let = rel.As<Flight>(),
                            Do = m.As<Airport>()
                        }).Results.ToList();

                var flights = new List<Flight>(query1.Count);


                foreach (var item in query1)
                {
                    Flight f = item.Let;
                    f.From = item.Od;
                    f.To = item.Do;
                    flights.Add(f);

                }
                viewModel.FlightsList = flights;

            return RedirectToAction("Admin");
            //return RedirectToAction("Admin", "Flights", new { viewModel });
        }


        // GET: FlightsController/Delete/5
        public ActionResult Delete(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport)-[rel:FLIGHT_TO]->(m:Airport) WHERE rel.id = {id} DELETE rel",
                queryDict, CypherResultMode.Projection);
            List<Flight> flight = ((IRawGraphClient)client).ExecuteGetCypherResults<Flight>(query).ToList();


            QueryFlightsViewModel viewModel = new QueryFlightsViewModel();


            var query1 = client.Cypher
                    .Match("(n:Airport)-[rel:FLIGHT_TO]->(m:Airport)")
                    .Return((n, rel, m) => new
                    {
                        Od = n.As<Airport>(),
                        Let = rel.As<Flight>(),
                        Do = m.As<Airport>()
                    }).Results.ToList();

            var flights = new List<Flight>(query1.Count);


            foreach (var item in query1)
            {
                Flight f = item.Let;
                f.From = item.Od;
                f.To = item.Do;
                flights.Add(f);

            }
            viewModel.FlightsList = flights;

            //return RedirectToAction("Admin");
            return RedirectToAction("Admin", "Flights", new { viewModel });
        }

        // POST: FlightsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchFlights(QueryFlightsViewModel model)
        {
            if (!ModelState.IsValid)
            {

            }
            else
            {
                    var query = client.Cypher
                        .Match("(n:Airport)-[rel:FLIGHT_TO]->(m:Airport)")
                        .Where((Airport n) => n.city == model.CityFrom)
                        .AndWhere((Airport m) => m.city == model.CityTo)
                        .AndWhere((Flight rel) => rel.DepartureDate == model.DepartureDate)
                        .Return((n, rel, m) => new
                        {
                            Od = n.As<Airport>(),
                            Let = rel.As<Flight>(),
                            Do = m.As<Airport>()
                        }).Results.ToList();

                    var flights = new List<Flight>(query.Count);

                    foreach (var item in query)
                    {
                        Flight f = item.Let;
                        f.From = item.Od;
                        f.To = item.Do;
                        flights.Add(f);

                    }
                    model.FlightsList = flights;


                    if (!model.direktnoIliSaPresedanjem)
                    {
                        var queryLayover1 = client.Cypher
                            .Match("(n:Airport)-[rel:FLIGHT_TO]->(m:Airport)-[rel2:FLIGHT_TO]->(k:Airport)")
                            .Where((Airport n) => n.city == model.CityFrom)
                            .AndWhere((Airport k) => k.city == model.CityTo)
                            .AndWhere((Flight rel) => rel.DepartureDate == model.DepartureDate)
                            .AndWhere((Flight rel, Flight rel2) => rel.DepartureDate == rel2.DepartureDate)
                            .Return((n, m, k, rel, rel2) => new
                            {
                                Od = n.As<Airport>(),
                                Preko = m.As<Airport>(),
                                Do = k.As<Airport>(),
                                LetOdPreko = rel.As<Flight>(),
                                LetPrekoDo = rel2.As<Flight>()
                            }).Results.ToList();

                        var flightsSaPresedanjem = new List<FlightSaPresedanjem>(query.Count);
                        foreach (var item in queryLayover1)
                        {
                            FlightSaPresedanjem f = new FlightSaPresedanjem();
                            f.Od = item.Od;
                            f.Do = item.Do;
                            f.Preko = item.Preko;
                            f.LetOdPreko = item.LetOdPreko;
                            f.LetPrekoDo = item.LetPrekoDo;
                            flightsSaPresedanjem.Add(f);

                        }

                        model.Presedanja = flightsSaPresedanjem;

                    }

                //ako nije cekirano "jednosmernaKarta", treba da prikaze i povratne karte
                if (!model.jednosmernaKarta)
                {
                    if (String.Compare(model.ReturnDate, model.DepartureDate) > 0 )
                    {
                        var query1 = client.Cypher
                        .Match("(n:Airport)-[rel:FLIGHT_TO]->(m:Airport)")
                        .Where((Airport m) => m.city == model.CityFrom)
                        .AndWhere((Airport n) => n.city == model.CityTo)
                        .AndWhere((Flight rel) => rel.DepartureDate == model.ReturnDate)
                        .Return((m, rel, n) => new
                        {
                            Od = n.As<Airport>(),
                            Let = rel.As<Flight>(),
                            Do = m.As<Airport>()
                        }).Results.ToList();

                        var flightsBack = new List<Flight>(query1.Count);

                        foreach (var item in query1)
                        {
                            Flight f = item.Let;
                            f.From = item.Od;
                            f.To = item.Do;
                            flightsBack.Add(f);

                        }

                        model.FlightsListBack = flightsBack;

                        if (!model.direktnoIliSaPresedanjem)
                        {
                            var query2 = client.Cypher
                            .Match("(n:Airport)-[rel:FLIGHT_TO]->(m:Airport)-[rel2:FLIGHT_TO]->(k:Airport)")
                            .Where((Airport k) => k.city == model.CityFrom)
                            .AndWhere((Airport n) => n.city == model.CityTo)
                            .AndWhere((Flight rel) => rel.DepartureDate == model.ReturnDate)
                            .AndWhere((Flight rel, Flight rel2) => rel.DepartureDate == rel2.DepartureDate)
                            .Return((n, m, k, rel, rel2) => new
                            {
                                Od = n.As<Airport>(),
                                Preko = m.As<Airport>(),
                                Do = k.As<Airport>(),
                                LetOdPreko = rel.As<Flight>(),
                                LetPrekoDo = rel2.As<Flight>()
                            }).Results.ToList();

                            var flightsSaPresedanjemNazad = new List<FlightSaPresedanjem>(query2.Count);
                            foreach (var item in query2)
                            {
                                FlightSaPresedanjem f = new FlightSaPresedanjem();
                                f.Od = item.Od;
                                f.Do = item.Do;
                                f.Preko = item.Preko;
                                f.LetOdPreko = item.LetOdPreko;
                                f.LetPrekoDo = item.LetPrekoDo;
                                flightsSaPresedanjemNazad.Add(f);

                            }

                            model.PresedanjaNazad = flightsSaPresedanjemNazad;
                        }
                    }
                    else
                    {
                        var let = new List<Flight>();
                        var let2 = new List<FlightSaPresedanjem>();
                        model.FlightsList = let;
                        model.FlightsListBack = let;
                        model.Presedanja = let2;
                        model.PresedanjaNazad = let2;

                    }
                   
                }
            }
            return View("Index", model);
        }

        public ActionResult BookFlight(int id1, int? id2)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id1", id1);
            queryDict.Add("id2", id2);

            //var bookFlight= new Neo4jClient.Cypher.CypherQuery("", queryDict, CypherResultMode.Set);

            if (id2 == null)
            {
                 var bookFlight = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport)-[rel:FLIGHT_TO]->(m:Airport) WHERE rel.id = {id1} RETURN rel",
                    queryDict, CypherResultMode.Set);
                Flight flight = ((IRawGraphClient)client).ExecuteGetCypherResults<Flight>(bookFlight).SingleOrDefault();
                if (flight.SeatsAvailable > 0)
                {
                    flight.SeatsAvailable -= 1;
                }
                else
                {
                    ViewBag.Message = string.Format("Nema vise slobodnih mesta!");
                    return View("Index");
                }
                var seatsAvailable = flight.SeatsAvailable;

                var update = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport)-[rel:FLIGHT_TO]->(m:Airport) WHERE "
                                                                                + "rel.id = {id1} "
                                                                                + "SET rel.SeatsAvailable = " + seatsAvailable
                                                                                + " RETURN rel", queryDict, CypherResultMode.Set);

                Flight f = ((IRawGraphClient)client).ExecuteGetCypherResults<Flight>(update).SingleOrDefault();
            }
            //letovi sa presedanjem
            else
            {
                var bookFlight = client.Cypher
                            .Match("(n:Airport)-[rel:FLIGHT_TO]->(m:Airport)-[rel2:FLIGHT_TO]->(k:Airport)")
                            .Where((Flight rel) => rel.id == id1)
                            .AndWhere((Flight rel2) => rel2.id == id2)
                            .Return((rel, rel2) => new
                            {
                                LetOdPreko = rel.As<Flight>(),
                                LetPrekoDo = rel2.As<Flight>()
                            }).Results.SingleOrDefault();

                
                var let1 = new Flight();
                var let2 = new Flight();
                let1 = bookFlight.LetOdPreko;
                let2 = bookFlight.LetPrekoDo;

                if(let1.SeatsAvailable > 0 && let2.SeatsAvailable > 0)
                {
                    let1.SeatsAvailable -= 1;
                    let2.SeatsAvailable -= 1;
                }
                else
                {
                    ViewBag.Message = string.Format("Nema vise slobodnih mesta!");
                    return View("Index");
                }

                var seatsAvailable1 = let1.SeatsAvailable;
                var seatsAvailable2 = let2.SeatsAvailable;

                var update = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport)-[rel:FLIGHT_TO]->(m:Airport)-[rel2:FLIGHT_TO]->(k:Airport) WHERE "
                                                                                + "rel.id = {id1} AND rel2.id = {id2} "
                                                                                + "SET rel.SeatsAvailable = " + seatsAvailable1
                                                                                + " , rel2.SeatsAvailable = " + seatsAvailable2
                                                                                + " RETURN rel", queryDict, CypherResultMode.Set);
                
                Flight f = ((IRawGraphClient)client).ExecuteGetCypherResults<Flight>(update).SingleOrDefault();
            }


            ViewBag.Message = string.Format("Uspesno ste rezervisali kartu!");
            return View("Index");
        }
    }
}
