using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using System.Web.Security;
using TaxiService.Models;
using TaxiService.Models.Base;
using TaxiService.Models.Security;

namespace TaxiService.Controllers
{
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
            var User = DB.UserDb.ToList().Find(p => p.Username == data.Username && p.Password == data.Password);
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
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName,encTicket);
            HttpContext.Current.Response.Cookies.Add(faCookie);
            return Ok();
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
                return DB.UserDb.ToList().Find(p => p.Username == AuthUser.Username);
            
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
