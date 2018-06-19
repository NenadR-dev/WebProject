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
        private static String CarType = CarRole.Not_Specified;
        private static Client LoggedClient = null;

        DataAccess DB = DataAccess.CreateDb();

        [HttpPost,Route("api/Client/AddClient")]
        public IHttpActionResult AddClient(Client data)
        {
            if (!DB.DriverDb.ToList().Exists(p => p.Username == data.Username)
                    && !DB.ClientDb.ToList().Exists(p => p.Username == data.Username)
                    && !DB.DispacherDb.ToList().Exists(p => p.Username == data.Username))
            {
                data.RideList = new List<RideBase>();
                data.ID = DB.ClientDb.ToList().Count() + 1;
                DB.ClientDb.Add(data);
            }
            DB.SaveChanges();
            return Ok(data);
        }

        [HttpPost,Route("api/Client/Update")]
        public IHttpActionResult Update(Client data)
        {
            int id = DB.ClientDb.ToList().IndexOf(DB.ClientDb.ToList().Find(p => p.Username == data.Username));
            DB.ClientDb.ToList()[id].Password = data.Password;
            DB.ClientDb.ToList()[id].ContactPhone = data.ContactPhone;
            DB.ClientDb.ToList()[id].Email = data.Email;
            DB.ClientDb.ToList()[id].Firstname = data.Firstname;
            DB.ClientDb.ToList()[id].Lastname = data.Lastname;
            DB.ClientDb.ToList()[id].Gender = data.Gender;
            DB.ClientDb.ToList()[id].JMBG = data.JMBG;
            DB.SaveChanges();
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

        [HttpPost,Route("api/Client/AddCarType/{id:int}")]
        public IHttpActionResult AddCarType(int id)
        {
            if(id == 0)
            {
                CarType = CarRole.Not_Specified;
            }
            else if (id == 1)
            {
                CarType = CarRole.Sedan;
            }
            else
            {
                CarType = CarRole.Van;
            }
            return Ok();
        }

        [HttpPost,Route("api/Client/OrderRide")]
        public IHttpActionResult OrderRide(LoginBase data)
        {
            LoggedClient = DB.ClientDb.ToList().Find(p => p.Username == data.Username && p.Password == data.Password);

            LoggedClient.RideList = new List<RideBase>
            {
                new RideBase()
                {
                    CarType = CarType,
                    Status = RideStatus.Created,
                    RideClient = LoggedClient.ID,
                    CommentID = null,
                    Location = CurrentLocation,
                    Destination = Destination,
                    DispacherID = null,
                    RidePrice = 0,
                    RiderOrderDate = DateTime.Now.ToString(),
                    DriverID = null
                }
            };
            DB.ClientDb.ToList()[DB.ClientDb.ToList().IndexOf(DB.ClientDb.ToList().Find(p => p.Username == data.Username && p.Password == data.Password))] = LoggedClient;

            DB.SaveChanges();

            return Ok();
        }

        [HttpGet,Route("api/Client/getRides")]
        public IHttpActionResult getRides()
        {
            List<RideBase> rides = DB.RideDb.ToList();
            rides.ForEach(ride =>
            {
                ride.Location = DB.LocationDb.ToList().Find(p => p.ID == ride.Location.ID);
                ride.Destination = DB.LocationDb.ToList().Find(p => p.ID == ride.Destination.ID);
            });
            return Ok(rides);
        }

        [HttpPost,Route("api/Client/getComment{id:int}")]
        public IHttpActionResult getComment(int id)
        {
            return Ok(DB.CommentDb.ToList().Find(p=>p.ID == id));
        }
    }
}
