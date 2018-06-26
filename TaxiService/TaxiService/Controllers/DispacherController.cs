using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using TaxiService.Models;
using TaxiService.Models.Base;
using TaxiService.Models.Security;

namespace TaxiService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DispacherController : ApiController
    {
        private static LocationBase CurrentLocation = null;
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

        private static DataAccess DB = DataAccess.CreateDb();

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
        public IHttpActionResult AddDriver(DriverCreationBase data)
        {
            lock (DB)
            {
                if (!DB.DriverDb.ToList().Exists(p => p.Username == data.Driver.Username)
                        && !DB.ClientDb.ToList().Exists(p => p.Username == data.Driver.Username)
                        && !DB.DispacherDb.ToList().Exists(p => p.Username == data.Driver.Username))
                {
                    if (DB.CarDb.ToList().Exists(x => x.TaxiCarID == data.Car.TaxiCarID))
                        return BadRequest();

                    data.Driver.ID = DB.DriverDb.ToList().Count + 1;
                    data.Car.ID = DB.CarDb.ToList().Count + 1;
                    data.Car.OwnerID = data.Driver.ID;
                    data.Driver.CarID = data.Car.TaxiCarID;
                    data.Driver.RideList = new List<RideBase>();
                    data.Driver.Role = UserRole.DriverRole;

                    DB.UserDb.Add(new LoginBase()
                    {
                        Username = data.Driver.Username,
                        Password = data.Driver.Password,
                        Role = data.Driver.Role
                    });
                    DB.CarDb.Add(data.Car);
                    DB.DriverDb.Add(data.Driver);
                }
                DB.SaveChanges();
                return Ok();
            }
        }

        [HttpPost, Route("api/Dispacher/Update")]
        public IHttpActionResult Update(Dispacher data)
        {
            lock (DB)
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
        }

        [HttpPost, Route("api/Dispacher/LogOff")]
        public IHttpActionResult LogOff()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return Ok();
        }

        [HttpPost,Route("api/Dispacher/AssignDriver")]
        public IHttpActionResult AssignDriver(AssignBase assign)
        {
            lock (DB)
            {
                DB.RideDb.ToList().Find(p => p.ID == assign.RideID).TaxiRiderID = assign.DriverID;
                DB.RideDb.ToList().Find(p => p.ID == assign.RideID).Status = RideStatus.Processed;
                DB.RideDb.ToList().Find(p => p.ID == assign.RideID).AdminID = AuthUser.ID;
                DB.SaveChanges();
                return Ok(DB.DriverDb.ToList().Find(p => p.ID == assign.DriverID).Firstname);
            }
        }
        [HttpPost, Route("api/Dispacher/AddLocation")]
        public IHttpActionResult AddLocation(LocationBase data)
        {
            CurrentLocation = data;
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
            lock (DB)
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
                    Location = new LocationBase(){
                        Place = CurrentLocation.Place,
                        XCoordinate = CurrentLocation.XCoordinate,
                        YCoordinate = CurrentLocation.YCoordinate,
                        StreetName = CurrentLocation.StreetName,
                        ZipCode = CurrentLocation.ZipCode
                    },
                    Destination = new LocationBase(),
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
            try
            {
                lock (DB)
                {
                    RideBase ride = DB.RideDb.ToList().Find(x => x.ID == id);

                    List<Driver> drivers = new List<Driver>();
                    DB.DriverDb.ToList().ForEach(x =>
                    {
                        CarBase car = DB.CarDb.ToList().Find(y => y.TaxiCarID == x.CarID);
                        if (ride.CarType == car.CarType || ride.CarType == CarRole.Not_Specified)
                        {
                            if(!DB.RideDb.ToList().Exists(z=> z.Status == RideStatus.Accepted && z.TaxiRiderID == id))
                                drivers.Add(x);
                        }
                    });

                    LocationBase location = DB.LocationDb.ToList().Find(x => x.ID == ride.Location.ID);

                    List<Tuple<double, Driver>> freeDrivers = new List<Tuple<double, Driver>>();
                    drivers.ForEach(x =>
                    {
                        x.Location = DB.LocationDb.ToList().Find(y => y.ID == x.Location.ID);
                        double lentgh = Math.Abs
                        (Math.Sqrt(Math.Pow((location.XCoordinate - x.Location.XCoordinate), 2) + Math.Pow((location.YCoordinate - x.Location.YCoordinate), 2)));

                        freeDrivers.Add(new Tuple<double, Driver>(lentgh, x));
                    });
                    freeDrivers = freeDrivers.OrderByDescending(x => x.Item1).ToList();
                    int driverCount = freeDrivers.Count;
                    drivers = new List<Driver>();
                    for (int i = 0; i < 5; i++)
                    {
                        if (driverCount == 0)
                            break;

                        drivers.Add(freeDrivers[i].Item2);
                        driverCount--;
                    }

                    return Ok(drivers);
                }
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet,Route("api/Dispacher/GetAllClients")]
        public IHttpActionResult GetAllClients()
        {
            lock (DB)
            {
                List<Client> clients = DB.ClientDb.ToList();
                return Ok(clients);
            }
        }

        [HttpGet, Route("api/Dispacher/GetALlDrivers")]
        public IHttpActionResult GetALlDrivers()
        {
            lock (DB)
            {
                List<Driver> drivers = DB.DriverDb.ToList();
                return Ok(drivers);
            }
        }

        [HttpPost,Route("api/Dispacher/BanDriver{id:int}")]
        public IHttpActionResult BanDrive(int id)
        {
            lock (DB)
            {
                Driver target = DB.DriverDb.ToList().Find(x => x.ID == id);
                if (!DB.BlockDb.ToList().Exists(x => x.Username == target.Username))
                {
                    DB.BlockDb.Add(new BanBase()
                    {
                        Username = target.Username
                    });
                    DB.SaveChanges();
                }
                else
                {
                    return BadRequest("Driver: " + target.Username + "already banned.");
                }
                return Ok("Driver: "+target.Username+" banned.");
            }
        }
        [HttpPost, Route("api/Dispacher/BanClient{id:int}")]
        public IHttpActionResult BanClient(int id)
        {
            lock (DB)
            {
                Client target = DB.ClientDb.ToList().Find(x => x.ID == id);
                if (!DB.BlockDb.ToList().Exists(x => x.Username == target.Username))
                {
                    DB.BlockDb.Add(new BanBase()
                    {
                        Username = target.Username
                    });
                    DB.SaveChanges();
                }
                else
                {
                    return BadRequest("Client: " + target.Username + "already banned.");
                }
                return Ok("Client: " + target.Username + " banned.");
            }
        }

        [HttpPost, Route("api/Dispacher/UnbanDriver{id:int}")]
        public IHttpActionResult UnbanDrive(int id)
        {
            lock (DB)
            {
                Driver target = DB.DriverDb.ToList().Find(x => x.ID == id);
                if (DB.BlockDb.ToList().Exists(x => x.Username == target.Username))
                {
                    DB.BlockDb.Remove(DB.BlockDb.ToList().Find(x => x.Username == target.Username));
                    DB.SaveChanges();
                }
                else
                {
                    return BadRequest("Driver: " + target.Username + "isnt banned.");
                }
                return Ok("Driver: " + target.Username + " un-banned.");
            }
        }

        [HttpPost, Route("api/Dispacher/UnbanClient{id:int}")]
        public IHttpActionResult UnbanClient(int id)
        {
            lock (DB)
            {
                Client target = DB.ClientDb.ToList().Find(x => x.ID == id);
                if (DB.BlockDb.ToList().Exists(x => x.Username == target.Username))
                {
                    DB.BlockDb.Remove(DB.BlockDb.ToList().Find(x=> x.Username == target.Username));
                    DB.SaveChanges();
                }
                else
                {
                    return BadRequest("Client: " + target.Username + "isnt banned.");
                }
                return Ok("Client: " + target.Username + " un-banned.");
            }
        }
    }
}
