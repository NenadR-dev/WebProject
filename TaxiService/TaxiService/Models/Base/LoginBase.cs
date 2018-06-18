using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models.Base
{
    public class LoginBase
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public String Role { get; set; }
    }
}