using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MPNFlights.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace MPNFlights.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly GraphClient client;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public HomeController()
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

        public IActionResult Index()
        {
            //Dictionary<string, object> queryDict = new Dictionary<string, object>();
            //queryDict.Add("Nis", "Nis");
            //var query = new Neo4jClient.Cypher.CypherQuery("MATCH(n: Airport) RETURN n LIMIT 25", queryDict, CypherResultMode.Set);

            //List<Airport> aerodromi = ((IRawGraphClient)client).ExecuteGetCypherResults<Airport>(query).ToList();
            //return View(aerodromi.ToList());
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
