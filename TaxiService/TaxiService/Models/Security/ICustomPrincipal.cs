using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TaxiService.Models.Security
{
    public interface ICustomPrincipal : IPrincipal
    {
        int ID { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string Role { get; set; }
    }
}
