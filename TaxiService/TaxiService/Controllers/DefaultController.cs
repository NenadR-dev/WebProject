using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using TaxiService.Models;
using TaxiService.Models.Base;

namespace TaxiService.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet, Route("")]
        [AllowAnonymous]
        public RedirectResult Index()
        {
            var route = Request.RequestUri.AbsoluteUri;
            return Redirect(route + "Home");
        }

        private static LoginBase currentUser = new LoginBase();

        [HttpPost,Route("api/Default/ValidateLogin")]
        public IHttpActionResult ValidateLogin(LoginBase data)
        {
            DataAccess db = DataAccess.CreateDb();
            if (db.ClientDb.ToList().Exists(p => p.Username == data.Username && p.Password == data.Password))
            {
                currentUser = data;
                currentUser.Role = db.ClientDb.ToList().Find(p => p.Username == data.Username).Role;
                db.ClientDb.ToList().Find(p => p.Username == data.Username).LoggedIn = true;
                currentUser.loggedIn = true;

                return Ok(currentUser);
            }
            if (db.DriverDb.ToList().Exists(p => p.Username == data.Username && p.Password == data.Password))
            {
                currentUser = data;
                currentUser.Role = db.DriverDb.ToList().Find(p => p.Username == data.Username).Role;
                db.DriverDb.ToList().Find(p => p.Username == data.Username).LoggedIn = true;
                currentUser.loggedIn = true;

                return Ok(currentUser);
            }
            return NotFound();
        }

        [HttpGet, Route("api/Default/getUser")]
        public LoginBase getUser()
        {
            return currentUser;
        }
    }
}
