using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaxiService.Models;
using TaxiService.Models.Base;

namespace TaxiService.Controllers
{
    public class ClientController : ApiController
    {

        [HttpPost,Route("api/Client/AddClient")]
        public IHttpActionResult AddClient(Client data)
        {
            DataAccess db = DataAccess.CreateDb();
            if (!db.DriverDb.ToList().Exists(p => p.Username == data.Username)
                    && !db.ClientDb.ToList().Exists(p => p.Username == data.Username)
                    && !db.DispacherDb.ToList().Exists(p => p.Username == data.Username))
            {
                data.RideList = new List<RideBase>();
                data.LoggedIn = false;
                data.ID = db.ClientDb.ToList().Count() + 1;
                db.ClientDb.Add(data);
            }
            db.SaveChanges();
            return Ok();
        }

        [HttpPost,Route("api/Client/Update")]
        public IHttpActionResult Update(Client data)
        {
            DataAccess db = DataAccess.CreateDb();
            int id = db.ClientDb.ToList().IndexOf(db.ClientDb.ToList().Find(p => p.Username == data.Username));
            db.ClientDb.ToList()[id].Password = data.Password;
            db.ClientDb.ToList()[id].ContactPhone = data.ContactPhone;
            db.ClientDb.ToList()[id].Email = data.Email;
            db.ClientDb.ToList()[id].Firstname = data.Firstname;
            db.ClientDb.ToList()[id].Lastname = data.Lastname;
            db.ClientDb.ToList()[id].Gender = data.Gender;
            db.ClientDb.ToList()[id].JMBG = data.JMBG;
            db.SaveChanges();
            return Ok();
        }
        
        [HttpPost,Route("api/Client/LogOff")]
        public IHttpActionResult LogOff(LoginBase data)
        {
            DataAccess db = DataAccess.CreateDb();
            db.ClientDb.ToList().Find(p => p.Username == data.Username).LoggedIn = false;
            return Ok();
        }

        [HttpPost,Route("api/Client/OrderRide")]
        public IHttpActionResult OrderRide(LoginBase data)
        {
            
            return Ok();
        }
    }
}
