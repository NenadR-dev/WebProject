using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    public class ClientController : ApiController
    {
        [HttpPost,Route("api/Client/AddClient")]
        public IHttpActionResult AddClient(Client data)
        {
            DataAccess db = DataAccess.CreateDb();
            if (!db.driverDb.ToList().Exists(p => p.Username == data.Username)
                    && !db.clientDb.ToList().Exists(p => p.Username == data.Username)
                    && !db.DispacherDb.ToList().Exists(p => p.Username == data.Username))
            {
                data.RideList = new List<RideBase>();
                data.ID = db.clientDb.ToList().Count + 1;
                db.clientDb.Add(data);
            }
            db.SaveChanges();
            return Ok();
        }
    }
}
