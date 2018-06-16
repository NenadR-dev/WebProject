using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet, Route("")]
        public RedirectResult Index()
        {
            var route = Request.RequestUri.AbsoluteUri;
            return Redirect(route + "Home");
        }

        public IHttpActionResult ValidateLogin(string Username,string Password)
        {
            DataAccess db = DataAccess.CreateDb();
            if(db.clientDb.ToList().Exists(p=>p.Username == Username && p.Password==Password))
            {
                return Ok("Client");
            }
            if (db.driverDb.ToList().Exists(p => p.Username == Username && p.Password == Password))
            {
                return Ok("Driver");
            }
            return NotFound();
        }
    }
}
