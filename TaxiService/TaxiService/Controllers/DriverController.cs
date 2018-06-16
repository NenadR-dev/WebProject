using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaxiService.Models;

namespace TaxiService.Controllers
{
    public class DriverController : ApiController
    {
        public IHttpActionResult AddClient(Driver data)
        {
            DataAccess db = DataAccess.CreateDb();
            if (!db.driverDb.ToList().Exists(p => p.Username == data.Username)
                && !db.clientDb.ToList().Exists(p => p.Username == data.Username)
                && !db.DispacherDb.ToList().Exists(p => p.Username == data.Username))
            {
                data.RideList = new List<RideBase>();
                data.Car = new CarBase();
                data.Location = new LocationBase();
                data.ID = db.driverDb.ToList().Count + 1;
                db.driverDb.Add(data);
            }
            db.SaveChanges();
            return Ok();
        }
    }
}
