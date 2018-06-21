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
        [AllowAnonymous]
        public ActionResult Index()
        {
            
            ViewBag.Title = "Home Page";

            return View();
        }
        [AllowAnonymous]
        public ActionResult Register(int id)
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Signup()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
    }
}
