using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TaxiService.Models;
using TaxiService.Models.Security;

namespace TaxiService.Controllers
{
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

        public UserBase getUserFromDB()
        {
            DataAccess db = DataAccess.CreateDb();
            if (AuthUser.Role == UserRole.ClientRole)
            {
                return db.ClientDb.ToList().Find(p => p.Username == AuthUser.Username);
            }
            else if (AuthUser.Role == UserRole.DriverRole)
            {
                return db.DriverDb.ToList().Find(p => p.Username == AuthUser.Username);
            }
            else if (AuthUser.Role == UserRole.DispacherRole)
            {
                return db.DispacherDb.ToList().Find(p => p.Username == AuthUser.Username);
            }
            else
                return null;
        }

    }
}