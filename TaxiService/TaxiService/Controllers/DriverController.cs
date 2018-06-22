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
        private static LocationBase CurrentLocation = null;
        private static LocationBase Destination = null;
        private static String CarType = CarRole.Not_Specified;
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

        [HttpPost,Route("api/Driver/AddDriver")]
        public IHttpActionResult AddDriver(Driver data)
        {
            DataAccess db = DataAccess.CreateDb();
            if (!db.DriverDb.ToList().Exists(p => p.Username == data.Username)
                && !db.ClientDb.ToList().Exists(p => p.Username == data.Username)
                && !db.DispacherDb.ToList().Exists(p => p.Username == data.Username))
            {
                data.RideList = new List<RideBase>();
                data.CarID = 0;
                data.Location = new LocationBase();
                data.ID = db.DriverDb.ToList().Count + 1;
                db.DriverDb.Add(data);
            }
            db.SaveChanges();
            return Ok();
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
    }
}
