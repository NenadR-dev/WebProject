using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using TaxiService.Models;
using TaxiService.Models.Security;

namespace TaxiService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DriverController : ApiController
    {

        private static DataAccess DB = DataAccess.CreateDb();


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
        
        [HttpGet,Route("api/Driver/CheckIfBanned")]
        public IHttpActionResult CheckIfBanned()
        {
            lock (DB)
            {
                if(DB.BlockDb.ToList().Exists(x=> x.Username == AuthUser.Username))
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
        }

        [HttpPost,Route("api/Driver/SetLocation")]
        public IHttpActionResult SetLocation(LocationBase data)
        {
            lock (DB)
            {
                DB.DriverDb.ToList().Find(x => x.Username == AuthUser.Username).Location = new LocationBase()
                {
                    Place = data.Place,
                    StreetName = data.StreetName,
                    XCoordinate = data.XCoordinate,
                    YCoordinate = data.YCoordinate,
                    ZipCode = data.ZipCode
                };
                DB.SaveChanges();
                return Ok();
            }
        }

        [HttpPost,Route("api/Driver/CompleteRide")]
        public IHttpActionResult CompleteRide()
        {
            lock (DB)
            {
                int taxiDriverID = DB.DriverDb.ToList().Find(x => x.Username == AuthUser.Username).ID;
                DB.RideDb.ToList().Find(p => p.TaxiRiderID == taxiDriverID && p.Status == RideStatus.Accepted).Status = RideStatus.Successful;
                DB.SaveChanges();
                return Ok();
            }
        }

        [HttpPost,Route("api/Driver/CancelRide{id:int}")]
        public IHttpActionResult CancelRide(int id)
        {
            lock (DB)
            {
                DB.RideDb.ToList().Find(p => p.ID == id).Status = RideStatus.Unsuccessful;
                DB.SaveChanges();
                return Ok();
            }
        }

        [HttpPost,Route("api/Driver/addComment")]
        public IHttpActionResult addComment(CommentBase data)
        {
            lock (DB)
            {
                RideBase ride = DB.RideDb.ToList().Find(p => p.ID == data.ID);
                List<CommentBase> comment = DB.CommentDb.ToList();
                if (!comment.Exists(p => p.RideID == data.ID))
                {
                    DB.CommentDb.Add(new CommentBase()
                    {
                        CommentDate = DateTime.Now.ToString(),
                        Stars = data.Stars,
                        Summary = data.Summary,
                        OriginalPoster = AuthUser.Username,
                        RideID = ride.ID
                    });
                    ride.CommentID = data.ID;
                    DB.SaveChanges();

                    return Ok(DB.CommentDb.ToList().Find(p => p.RideID == data.ID));
                }
                return NotFound();
            }
        }

        [HttpGet,Route("api/Driver/getNewRides")]
        public IHttpActionResult getNewRides()
        {
            lock (DB)
            {
                Driver d = DB.DriverDb.ToList().Find(x => x.Username == AuthUser.Username);
                CarBase car = DB.CarDb.ToList().Find(x => x.TaxiCarID == d.CarID);

                List<RideBase> result = DB.RideDb.ToList().Where(p => p.Status == RideStatus.Created && (car.CarType == p.CarType || p.CarType == CarRole.Not_Specified)).ToList();
                result.ForEach(ride =>
                {
                    ride.Location = DB.LocationDb.ToList().Find(l => l.ID == ride.Location.ID);
                    ride.Destination = DB.LocationDb.ToList().Find(p => p.ID == ride.Destination.ID);
                });
                return Ok(result);
            }
        }
        [HttpGet, Route("api/Driver/getComment{id:int}")]
        public IHttpActionResult getComment(int id)
        {
            lock (DB)
            {
                CommentBase comment = DB.CommentDb.ToList().Find(p => p.RideID == id);
                return Ok(comment ?? new CommentBase());
            }
        }

        [HttpGet,Route("api/Driver/getDriverRides")]
        public IHttpActionResult getDriverRides()
        {
            lock (DB)
            {
                int taxiDriverID = DB.DriverDb.ToList().Find(x => x.Username == AuthUser.Username).ID;
                List<RideBase> result = DB.RideDb.ToList().Where(p=>p.TaxiRiderID == taxiDriverID).ToList();
                result.ForEach(ride =>
                {
                    ride.Location = DB.LocationDb.ToList().Find(l => l.ID == ride.Location.ID);
                    ride.Destination = DB.LocationDb.ToList().Find(d => d.ID == ride.Destination.ID);
                    if(ride.Status == RideStatus.Processed)
                    {
                        ride.Status = RideStatus.Accepted;
                    }
                });
                result = result.OrderByDescending(p => p.Status == RideStatus.Accepted).ToList();
                return Ok(result);
            }
        }

        [HttpPost,Route("api/Driver/AcceptRide{rideID:int}")]
        public IHttpActionResult AcceptRide(int rideID)
        {
            lock (DB)
            {
                int taxiDriverID = DB.DriverDb.ToList().Find(x => x.Username == AuthUser.Username).ID;
                if (!DB.RideDb.ToList().Exists(p=> p.TaxiRiderID == taxiDriverID && p.Status == RideStatus.Accepted))
                {
                    DB.RideDb.ToList().Find(p => p.ID == rideID).Status = RideStatus.Accepted;
                    DB.RideDb.ToList().Find(p => p.ID == rideID).TaxiRiderID = taxiDriverID;
                    DB.SaveChanges();
                    return Ok();
                }
                else
                { 
                    return Content(HttpStatusCode.BadRequest, "Cannot accept another ride.");
                }
            }
        }

        [HttpPost,Route("api/Driver/AddDestination")]
        public IHttpActionResult AddDestination(LocationBase data)
        {
            lock (DB)
            {
                int taxiDriverID = DB.DriverDb.ToList().Find(x => x.Username == AuthUser.Username).ID;
                RideBase currentRide = DB.RideDb.ToList().Find(x => x.TaxiRiderID == taxiDriverID && x.Status == "Accepted");
                LocationBase l = DB.LocationDb.ToList().Find(x => x.ID == currentRide.Destination.ID);
                l.Place = data.Place;
                l.StreetName = data.StreetName;
                l.ZipCode = data.ZipCode;
                l.XCoordinate = data.XCoordinate;
                l.YCoordinate = data.YCoordinate;
                DB.SaveChanges();
                return Ok();
            }
        }

        [HttpPost,Route("api/Driver/SetPrice{price:int}")]
        public IHttpActionResult SetPrice(int price)
        {
            lock (DB)
            {
                int taxiDriverID = DB.DriverDb.ToList().Find(x => x.Username == AuthUser.Username).ID;
                DB.RideDb.ToList().Find(x => x.TaxiRiderID == taxiDriverID && x.Status == RideStatus.Accepted).RidePrice = price;
                return Ok();
            }
        }

        [HttpPost, Route("api/Driver/LogOff")]
        public IHttpActionResult LogOff()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return Ok();
        }
    }
}
