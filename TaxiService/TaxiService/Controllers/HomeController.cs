using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace TaxiService.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Register(int id)
        {
            TempData["reg"] = (id == 1) ? "Client" : "Driver";
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
    }
}
