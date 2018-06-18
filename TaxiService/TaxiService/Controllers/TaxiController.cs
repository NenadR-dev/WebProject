using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    public class TaxiController : Controller
    {
        // GET: Taxi
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult U(string id)
        {
            DataAccess db = DataAccess.CreateDb();

            return View("Profile",db.ClientDb.ToList().Find(p => p.Username == id));
        }

        public ActionResult OrderRide()
        {
            return View();
        }
    }
}