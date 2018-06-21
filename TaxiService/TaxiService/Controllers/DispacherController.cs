using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TaxiService.Models;
using TaxiService.Models.Base;
using TaxiService.Models.Security;

namespace TaxiService.Controllers
{
    public class DispacherController : ApiController
    {
        private static LocationBase CurrentLocation = null;
        private static LocationBase Destination = null;
        private static String CarType = CarRole.Not_Specified;
        public CustomPrincipal AuthUser
        {
            get
            {
                if (HttpContext.Current.User != null)
                {
                    return HttpContext.Current.User as CustomPrincipal;
                }
                else
                {
                    return null;
                }
            }
        }

        DataAccess DB = DataAccess.CreateDb();

        [HttpPost, Route("api/Dispacher/getDriver{id:int}")]
        public IHttpActionResult getDriver(int id)
        {
            lock (DB)
            {
                Driver driver = DB.DriverDb.ToList().Find(p => p.ID == id);
                return Ok(driver);
            }
        }

        [HttpPost, Route("api/Dispacher/AddDriver")]
        public IHttpActionResult AddDriver(AssignBase data)
        {
            if (!DB.DriverDb.ToList().Exists(p => p.Username == data.Driver.Username)
                    && !DB.ClientDb.ToList().Exists(p => p.Username == data.Driver.Username)
                    && !DB.DispacherDb.ToList().Exists(p => p.Username == data.Driver.Username))
            {
                data.Car.Owner = data.Driver;
                DB.CarDb.Add(data.Car);
                data.Driver.RideList = new List<RideBase>();
                data.Driver.ID = DB.DriverDb.ToList().Count() + 1;
                data.Driver.Role = UserRole.DriverRole;
                data.Driver.CarID = data.Car.ID;
                DB.UserDb.Add(new LoginBase()
                {
                    Username = data.Driver.Username,
                    Password = data.Driver.Password,
                    Role = data.Driver.Role
                });
                DB.DriverDb.Add(data.Driver);
            }
            DB.SaveChanges();
            return Ok(data.Driver);
        }

        [HttpPost, Route("api/Dispacher/Update")]
        public IHttpActionResult Update(Dispacher data)
        {
            int id = DB.DispacherDb.ToList().IndexOf(DB.DispacherDb.ToList().Find(p => p.Username == data.Username));
            DB.DispacherDb.ToList()[id].Password = data.Password;
            DB.DispacherDb.ToList()[id].ContactPhone = data.ContactPhone;
            DB.DispacherDb.ToList()[id].Email = data.Email;
            DB.DispacherDb.ToList()[id].Firstname = data.Firstname;
            DB.DispacherDb.ToList()[id].Lastname = data.Lastname;
            DB.DispacherDb.ToList()[id].Gender = data.Gender;
            DB.DispacherDb.ToList()[id].JMBG = data.JMBG;
            DB.SaveChanges();
            return Ok();
        }

        [HttpPost, Route("api/Dispacher/LogOff")]
        public IHttpActionResult LogOff(LoginBase data)
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return Ok();
        }

        [HttpPost, Route("api/Dispacher/AddLocation")]
        public IHttpActionResult AddLocation(LocationBase data)
        {
            CurrentLocation = data;
            CurrentLocation.XCoordinate = 0;
            CurrentLocation.YCoordinate = 10;
            return Ok();
        }

        [HttpPost, Route("api/Dispacher/AddDestination")]
        public IHttpActionResult AddDestination(LocationBase data)
        {
            Destination = data;
            Destination.XCoordinate = 10;
            Destination.YCoordinate = 0;
            return Ok();
        }

        [HttpPost, Route("api/Dispacher/AddCarType/{id:int}")]
        public IHttpActionResult AddCarType(int id)
        {
            if (id == 0)
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

        [HttpPost, Route("api/Dispacher/OrderRide")]
        public IHttpActionResult OrderRide()
        {
            try
            {
                Dispacher Admin = DB.DispacherDb.ToList().Find(p => p.Username == AuthUser.Username);

                Admin.RideList = new List<RideBase>
            {
                new RideBase()
                {
                    CarType = CarType,
                    Status = RideStatus.Formated,
                    RideClient = 0,
                    CommentID = 0,
                    Location = CurrentLocation,
                    Destination = Destination,
                    AdminID = Admin.ID,
                    RidePrice = 0,
                    RiderOrderDate = DateTime.Now.ToString(),
                    TaxiRiderID = 0
                }
            };
                DB.DispacherDb.ToList()[DB.DispacherDb.ToList().IndexOf(DB.DispacherDb.ToList().Find(p => p.Username == AuthUser.Username))] = Admin;

                DB.SaveChanges();

                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet, Route("api/Dispacher/getRides")]
        public IHttpActionResult getRides()
        {
            lock (DB)
            {
                List<RideBase> rides = new List<RideBase>();
                rides = DB.RideDb.ToList();
                Dispacher user = DB.DispacherDb.ToList().Find(p => p.Username == AuthUser.Username);
                if (rides.Count != 0)
                {
                    rides.ForEach(ride =>
                    {
                        ride.Location = DB.LocationDb.ToList().Find(p => p.ID == ride.Location.ID);
                        ride.Destination = DB.LocationDb.ToList().Find(p => p.ID == ride.Destination.ID);
                    });
                    List<RideBase> result = new List<RideBase>();
                    result = rides.Where(p => p.AdminID != user.ID).ToList();
                    return Ok(result);
                }
                return NotFound();
            }
        }
        [HttpGet, Route("api/Dispacher/getDispacherRides")]
        public IHttpActionResult getDispacherRides()
        {
            lock (DB)
            {
                List<RideBase> rides = new List<RideBase>();
                rides = DB.RideDb.ToList();
                Dispacher user = DB.DispacherDb.ToList().Find(p => p.Username == AuthUser.Username);
                if (rides.Count != 0)
                {
                    rides.ForEach(ride =>
                    {
                        ride.Location = DB.LocationDb.ToList().Find(p => p.ID == ride.Location.ID);
                        ride.Destination = DB.LocationDb.ToList().Find(p => p.ID == ride.Destination.ID);
                    });
                    return Ok(rides.Where(p => p.AdminID == user.ID));
                }
                return NotFound();
            }
        }

        [HttpGet, Route("api/Dispacher/getComment{id:int}")]
        public IHttpActionResult getComment(int id)
        {
            lock (DB)
            {
                CommentBase comment = DB.CommentDb.ToList().Find(p => p.RideID == id);
                return Ok(comment ?? null);
            }
        }

        [HttpGet,Route("api/Dispacher/getFreeDrivers/{id:int}")]
        public IHttpActionResult getFreeDrivers(int id)
        {
            lock (DB)
            {
                List<Driver> freeDrivers = new List<Driver>();
                List<CarBase> cars = DB.CarDb.ToList();
                RideBase currentRide = DB.RideDb.ToList().Find(p=>p.ID == id);

                DB.DriverDb.ToList().ForEach(driver =>
                {
                    if(!DB.RideDb.ToList().Exists(ride=>ride.TaxiRiderID == driver.ID)
                    || DB.RideDb.ToList().Exists(ride=> ride.TaxiRiderID == driver.ID
                    && (ride.Status == RideStatus.Successful || ride.Status == RideStatus.Unsuccessful)))
                    {
                        if (cars.Find(p=>p.ID == driver.ID).CarType == currentRide.CarType || currentRide.CarType == "Not_Defined")
                            {
                                freeDrivers.Add(driver);
                            }
                    }
    
                });
                return Ok(freeDrivers);
            }
        }
    }
}
