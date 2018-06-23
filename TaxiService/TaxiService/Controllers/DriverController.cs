using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TaxiService.Models;
using TaxiService.Models.Security;

namespace TaxiService.Controllers
{
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

        [HttpPost,Route("api/Driver/CompleteRide{id:int}")]
        public IHttpActionResult CompleteRide(int id)
        {
            lock (DB)
            {
                DB.RideDb.ToList().Find(p => p.ID == id).Status = RideStatus.Successful;
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
            RideBase ride = DB.RideDb.ToList().Find(p => p.ID == data.ID);
            List<CommentBase> comment = DB.CommentDb.ToList();
            if (!comment.Exists(p => p.RideID == data.ID))
            {
                DB.CommentDb.Add(new CommentBase()
                {
                    CommentDate = DateTime.Now.ToString(),
                    Stars = data.Stars,
                    Summary = data.Summary,
                    ClientID = DB.ClientDb.ToList().Find(p => p.ID == ride.RideClient),
                    RideID = ride.ID
                });
                ride.CommentID = data.ID;
                DB.SaveChanges();

                return Ok(DB.CommentDb.ToList().Find(p => p.RideID == data.ID));
            }
            return NotFound();

        }

        [HttpGet,Route("api/Driver/getNewRides")]
        public IHttpActionResult getNewRides()
        {
            lock (DB)
            {
                List<RideBase> result = DB.RideDb.ToList().Where(p => p.Status == RideStatus.Created).ToList();
                result.ForEach(ride =>
                {
                    ride.Location = DB.LocationDb.ToList().Find(l => l.ID == ride.Location.ID);
                    ride.Destination = DB.LocationDb.ToList().Find(d => d.ID == ride.Destination.ID);
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
                List<RideBase> result = DB.RideDb.ToList().Where(p=>p.TaxiRiderID == AuthUser.ID).ToList();
                result.ForEach(ride =>
                {
                    ride.Location = DB.LocationDb.ToList().Find(l => l.ID == ride.Location.ID);
                    ride.Destination = DB.LocationDb.ToList().Find(d => d.ID == ride.Destination.ID);
                });
                result = result.OrderBy(p => p.Status == RideStatus.Accepted).ToList();
                return Ok(result);
            }
        }

        [HttpPost,Route("api/Driver/AcceptRide{rideID:int}")]
        public IHttpActionResult AcceptRide(int rideID)
        {
            lock (DB)
            {
                if (!DB.RideDb.ToList().Exists(p=> p.TaxiRiderID == AuthUser.ID && p.Status == RideStatus.Accepted))
                {
                    DB.RideDb.ToList().Find(p => p.ID == rideID).Status = RideStatus.Accepted;
                    DB.RideDb.ToList().Find(p => p.ID == rideID).TaxiRiderID = AuthUser.ID;
                    DB.SaveChanges();
                    return Ok();
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Cannot accept another ride.");
                }
            }
        }
    }
}
