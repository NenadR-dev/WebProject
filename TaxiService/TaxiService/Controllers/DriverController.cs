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
    public class DriverController : ApiController
    {

        private static DataAccess DB = DataAccess.CreateDb();
        private static FilterBase filter = null;
        private static int currentRideID = 0;

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

        [HttpPost, Route("api/Driver/ApplyFilter")]
        public IHttpActionResult ApplyFilter(FilterBase filterBase)
        {
            filter = filterBase;
            if (filter.ToDate.ToString() == "01-Jan-01 12:00:00 AM")
            {
                filter.ToDate = DateTime.Now;
            }
            if (filter.ToPrice == 0)
            {
                filter.ToPrice = 90000;
            }
            if (filter.ToGrade == 0)
            {
                filter.ToGrade = 5;
            }
            filter.ToDate = filterBase.ToDate.AddDays(1);
            return Ok();
        }

        [HttpPost,Route("api/Driver/CompleteRide")]
        public IHttpActionResult CompleteRide()
        {
            lock (DB)
            {
                DB.RideDb.ToList().Find(p => p.ID == currentRideID).Status = RideStatus.Successful;

                DB.SaveChanges();
                return Ok();
            }
        }

        [HttpPost,Route("api/Driver/CancelRide{id:int}")]
        public IHttpActionResult CancelRide(int id)
        {
            lock (DB)
            {
                try
                {
                    DB.RideDb.ToList().Find(p => p.ID == id).Status = RideStatus.Unsuccessful;
                    DB.SaveChanges();
                    return Ok();
                }
                catch
                {
                    return BadRequest();
                }
            }
        }

        [HttpPost,Route("api/Driver/AddComment")]
        public IHttpActionResult AddComment(CommentBase data)
        {
            lock (DB)
            {
                RideBase ride = DB.RideDb.ToList().Find(x => x.TaxiRiderID == DB.DriverDb.ToList().Find(y => y.Username == AuthUser.Username).ID && x.ID == currentRideID);
                List<CommentBase> comment = DB.CommentDb.ToList();
                if (!comment.Exists(p => p.RideID == data.ID))
                {
                    DB.CommentDb.Add(new CommentBase()
                    {
                        CommentDate = DateTime.Now.ToString(),
                        Stars = data.Stars,
                        Summary = data.Summary,
                        OriginalPoster = AuthUser.Username,
                        RideID = currentRideID
                    });
                    ride.CommentID = data.ID;
                    DB.SaveChanges();

                    return Ok();
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
                    try
                    {
                        ride.CommentID = DB.CommentDb.ToList().Find(x => x.RideID == currentRideID).RideID;
                    }
                    catch
                    {

                    }
                });

                if(filter != null)
                {
                    result = SortRides(result);
                }

                return Ok(result);
            }
        }

        private static List<RideBase> SortRides(List<RideBase> sortedList)
        {
            //SearchByDate
            sortedList = sortedList.Where(x => DateTime.Parse(x.RiderOrderDate) >= DateTime.Parse(filter.FromDate.ToShortDateString()) && DateTime.Parse(x.RiderOrderDate) <= DateTime.Parse(filter.ToDate.ToShortDateString())).ToList();

            //SearchByPrice
            sortedList = sortedList.Where(x => x.RidePrice >= filter.FromPrice && x.RidePrice <= filter.ToPrice).ToList();
            //SearchByGrade
            if (filter.FromGrade > 0)
            {
                List<RideBase> currentSort = new List<RideBase>();
                sortedList.ForEach(x => currentSort.Add(x));
                currentSort.RemoveAll(x => x.CommentID == 0);
                List<RideBase> newSorted = new List<RideBase>();
                currentSort.ForEach(x =>
                {
                    if (DB.CommentDb.ToList().Find(y => y.RideID == x.CommentID).Stars >= filter.FromGrade && DB.CommentDb.ToList().Find(y => y.RideID == x.CommentID).Stars <= filter.ToGrade)
                    {
                        newSorted.Add(x);
                    }
                });
                sortedList = newSorted;
            }

            //filter options
            if (!string.IsNullOrEmpty(filter.FilterStatus) && filter.FilterStatus != "None")
            {
                sortedList = sortedList.Where(x => string.Equals(filter.FilterStatus, x.Status)).ToList();
            }
            //Sort options
            if (filter.SortDate == "Newest")
            {
                sortedList = sortedList.OrderByDescending(x => x.RiderOrderDate).ThenBy(x => x.RiderOrderDate).ToList();
            }
            if (filter.SortDate == "Oldest")
            {
                sortedList = sortedList.OrderBy(x => x.RiderOrderDate).ThenBy(x => x.RiderOrderDate).ToList();
            }
            if (filter.SortGrade == "Highest")
            {
                List<RideBase> order = new List<RideBase>();
                sortedList.ForEach(x => order.Add(x));
                order.RemoveAll(x => x.CommentID == 0);
                order = order.OrderByDescending(x => DB.CommentDb.ToList().Find(y => y.RideID == x.CommentID).Stars).ToList();
                List<RideBase> newList = new List<RideBase>();
                order.ForEach(o =>
                {
                    newList.Add(o);
                });
                sortedList.ForEach(item =>
                {
                    if (!newList.Exists(p => p.ID == item.ID))
                    {
                        newList.Add(item);
                    }
                });
                sortedList = newList;
            }
            if (filter.SortGrade == "Lowest")
            {
                List<RideBase> order = sortedList;
                order.RemoveAll(x => x.CommentID == 0);
                order = order.OrderBy(x => DB.CommentDb.ToList().Find(y => y.RideID == x.CommentID).Stars).ToList();
                List<RideBase> newList = new List<RideBase>();
                order.ForEach(o =>
                {
                    newList.Add(o);
                });
                sortedList.ForEach(item =>
                {
                    if (!newList.Exists(p => p.ID == item.ID))
                    {
                        newList.Add(item);
                    }
                });
                sortedList = newList;
            }
            //end of sort
            //
            return sortedList;
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
                RideBase currentRide = DB.RideDb.ToList().Find(x => x.TaxiRiderID == taxiDriverID && x.ID == currentRideID);
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

        [HttpPost,Route("api/Driver/SetRideID{id:int}")]

        public IHttpActionResult SetRideID(int id)
        {
            currentRideID = id;
            return Ok();
        }
        [HttpPost,Route("api/Driver/SetPrice{price:int}")]

        public IHttpActionResult SetPrice(int price)
        {
            lock (DB)
            {
                int taxiDriverID = DB.DriverDb.ToList().Find(x => x.Username == AuthUser.Username).ID;
                DB.RideDb.ToList().Find(x => x.TaxiRiderID == taxiDriverID && x.ID == currentRideID).RidePrice = price;
                DB.SaveChanges();
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
