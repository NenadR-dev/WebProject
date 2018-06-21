using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models.Security
{
    public class CustomPrincipalSerializeModel
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}