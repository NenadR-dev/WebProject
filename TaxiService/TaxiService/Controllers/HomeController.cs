using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;
using TaxiService.Models.Security;

namespace TaxiService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeController : Controller
    {
        public CustomPrincipal AuthUser
        {
            get
            {
                if (System.Web.HttpContext.Current.User != null)
                {
                    return System.Web.HttpContext.Current.User as CustomPrincipal;
                }
                else
                {
                    return null;
                }
            }
        }


        [AllowAnonymous]
        public ActionResult Index()
        {
            
            ViewBag.Title = "Home Page";

            return View();
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (AuthUser == null)
                return View();
            else
                return RedirectToAction("Index", "Taxi");
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
    }
}
