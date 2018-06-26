using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using System.Web.Security;
using TaxiService.Models;
using TaxiService.Models.Base;
using TaxiService.Models.Security;

namespace TaxiService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DefaultController : ApiController
    {
        private static DataAccess DB = DataAccess.CreateDb();

        [HttpGet, Route("")]
        public RedirectResult Index()
        {
            var route = Request.RequestUri.AbsoluteUri;
            return Redirect(route + "Home");
        }

        [HttpPost,Route("api/Default/ValidateLogin")]
        public IHttpActionResult ValidateLogin(LoginBase data)
        {
            try
            {
                lock (DB)
                {
                    var User = DB.UserDb.ToList().Find(p => p.Username == data.Username && p.Password == data.Password);
                    if (User != null)
                    {
                        if(DB.BlockDb.ToList().Exists(x=> x.Username == User.Username))
                        {
                            return BadRequest("User banned");
                        }
                        CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                        serializeModel.ID = User.ID;
                        serializeModel.Username = User.Username;
                        serializeModel.Password = User.Password;
                        serializeModel.Role = User.Role;
                        JavaScriptSerializer serializer = new JavaScriptSerializer();

                        string userData = serializer.Serialize(serializeModel);
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1,
                            User.Username,
                            DateTime.Now,
                            DateTime.Now.AddMinutes(15),
                            false,
                            userData);

                        string encTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                        HttpContext.Current.Response.Cookies.Add(faCookie);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Invalid username or password");
                    }
                }
            }
            catch
            {
                return NotFound();
            }
        }

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

        [HttpGet, Route("api/Default/getUser")]
        public LoginBase getUser()
        {
            lock (DB)
            {
                return DB.UserDb.ToList().Find(p => p.Username == AuthUser.Username);
            }
        }
        [HttpGet, Route("api/Default/getCarTypes")]
        public List<string> getCarTypes()
        {
            List<String> items = new List<string>()
            {
                CarRole.Not_Specified,
                CarRole.Sedan,
                CarRole.Van
            };
            return items;
        }
    }
}
