using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;
using TaxiService.Models;
using TaxiService.Models.Security;

namespace TaxiService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TaxiController : Controller
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
        private static DataAccess DB = DataAccess.CreateDb();
        // GET: Taxi
        [Authorize]
        public ActionResult Index()
        {
            if(AuthUser.Role == UserRole.ClientRole)
            {
                return View("ClientIndex");
            }
            else if(AuthUser.Role == UserRole.DriverRole)
            {
                return View("DriverIndex");
            }
            else if(AuthUser.Role == UserRole.DispacherRole)
            {
                return View("DispacherIndex");
            }
            else
            {
                return View("TaxiError");
            }
            
        }
        [Authorize]
        public ActionResult U()
        {
            return View("Profile",getUserFromDB());
        }
        [Authorize]
        public ActionResult OrderRide()
        {
            return View();
        }
        [Authorize]
        public ActionResult AddDriver()
        {
            return View();
        }

        [Authorize]
        public ActionResult ManageClients()
        {
            if(AuthUser.Role == UserRole.DispacherRole)
            {
                return View();
            }
            else
            {
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [Authorize]
        public ActionResult FinishRide()
        {
            return View();
        }
        [Authorize]
        public ActionResult SetLocation()
        {
            return View();
        }

        public UserBase getUserFromDB()
        {
            lock (DB)
            {

                if (AuthUser.Role == UserRole.ClientRole)
                {
                    return DB.ClientDb.ToList().Find(p => p.Username == AuthUser.Username);
                }
                else if (AuthUser.Role == UserRole.DriverRole)
                {
                    return DB.DriverDb.ToList().Find(p => p.Username == AuthUser.Username);
                }
                else if (AuthUser.Role == UserRole.DispacherRole)
                {
                    return DB.DispacherDb.ToList().Find(p => p.Username == AuthUser.Username);
                }
                else
                    return null;
            }
        }

    }
}