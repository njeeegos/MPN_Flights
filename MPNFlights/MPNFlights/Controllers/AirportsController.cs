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
    public class AirportsController : Controller
    {
        private readonly GraphClient client;

        public AirportsController()
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

        // GET: AirportsController
        public ActionResult Index()
        {
            string airport = "";

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("airport", airport);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport) RETURN n ORDER BY n.country", queryDict, CypherResultMode.Set);
            var airports = ((IRawGraphClient)client).ExecuteGetCypherResults<Airport>(query).ToList();

            return View(airports);
        }

        // GET: AirportsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AirportsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AirportsController/Create
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

        // GET: AirportsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AirportsController/Edit/5
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

        // GET: AirportsController/Delete/5
        public ActionResult Delete(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport) WHERE n.id = {id} DETACH DELETE n",
                queryDict, CypherResultMode.Projection);
            List<Flight> flight = ((IRawGraphClient)client).ExecuteGetCypherResults<Flight>(query).ToList();

            return RedirectToAction("Index", "Airports");
        }

        // POST: AirportsController/Delete/5
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
        public ActionResult Save(Airport model)
        {
            int id = model.id;
            string city = model.city;
            string country = model.country;
            string name = model.name;
            string fullName = model.fullName;

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("city", city);
            queryDict.Add("coutnry", country);
            queryDict.Add("name", name);
            queryDict.Add("fullName", fullName);

            var queryMax = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Airport) RETURN max(n.id)", queryDict, CypherResultMode.Set);
            String maxIdObj = ((IRawGraphClient)client).ExecuteGetCypherResults<String>(queryMax).SingleOrDefault();
            int maxId;
            if (maxIdObj == null)
                maxId = 0;
            else
                maxId = Int32.Parse(maxIdObj);

            var createQuery = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Airport {id: " + ++maxId
                                                                + " , city: '" + city
                                                                + "', country: '" + country
                                                                + "', name: '" + name
                                                                + "', fullName: '" + fullName
                                                                + "'}) RETURN n", queryDict, CypherResultMode.Set);

            List<Airport> create = ((IRawGraphClient)client).ExecuteGetCypherResults<Airport>(createQuery).ToList();

            return RedirectToAction("Index");
        }
    }
}
