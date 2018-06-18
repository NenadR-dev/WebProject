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
        private static LocationBase CurrentLocation = null;
        private static LocationBase Destination = null;
        private static CarRole CarType = CarRole.Not_Specified;
        private static Client LoggedClient = null;

        [HttpPost,Route("api/Client/AddClient")]
        public IHttpActionResult AddClient(Client data)
        {
            DataAccess db = DataAccess.CreateDb();
            if (!db.DriverDb.ToList().Exists(p => p.Username == data.Username)
                    && !db.ClientDb.ToList().Exists(p => p.Username == data.Username)
                    && !db.DispacherDb.ToList().Exists(p => p.Username == data.Username))
            {
                data.RideList = new List<RideBase>();
                data.ID = db.ClientDb.ToList().Count() + 1;
                db.ClientDb.Add(data);
            }
            db.SaveChanges();
            return Ok(data);
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
            return Ok(data);
        }

        [HttpPost,Route("api/Client/AddLocation")]
        public IHttpActionResult AddLocation(LocationBase data)
        {
            CurrentLocation = data;
            CurrentLocation.XCoordinate = 0;
            CurrentLocation.YCoordinate = 10;
            return Ok();
        }
        
        [HttpPost,Route("api/Client/AddDestination")]
        public IHttpActionResult AddDestination(LocationBase data)
        {
            Destination = data;
            Destination.XCoordinate = 10;
            Destination.YCoordinate = 0;
            return Ok();
        }

        [HttpPost,Route("api/Client/AddCarType")]
        public IHttpActionResult AddCarType(int data)
        {
            CarType = (CarRole)data;
            return Ok();
        }

        [HttpPost,Route("api/Client/OrderRide")]
        public IHttpActionResult OrderRide(LoginBase data)
        {
            DataAccess db = DataAccess.CreateDb();
            LoggedClient = db.ClientDb.ToList().Find(p => p.Username == data.Username && p.Password == data.Password);
            List<Client> temp = new List<Client>();
            temp.Add(LoggedClient);

            LoggedClient.RideList = new List<RideBase>
            {
                new RideBase()
                {
                    CarType = CarType,
                    Status = RideStatus.Created,
                    RideClient = LoggedClient.ID,
                    RideClients = temp,
                    CommentID = null,
                    Location = CurrentLocation,
                    Destination = Destination,
                    DispacherID = null,
                    RidePrice = 500,
                    RiderOrderDate = DateTime.Now.ToShortDateString(),
                    DriverID = null
                }
            };
            db.ClientDb.ToList()[db.ClientDb.ToList().IndexOf(db.ClientDb.ToList().Find(p => p.Username == data.Username && p.Password == data.Password))] = LoggedClient;

            db.SaveChanges();

            return Ok();
        }
    }
}
