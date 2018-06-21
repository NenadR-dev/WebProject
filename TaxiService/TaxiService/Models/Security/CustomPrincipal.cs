using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace TaxiService.Models.Security
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public CustomPrincipal(string username)
        {
            this.Identity = new GenericIdentity(username);
        }


    }
}