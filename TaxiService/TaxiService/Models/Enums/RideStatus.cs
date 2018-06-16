using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public enum RideStatus
    {
        Created,
        Formated,
        Processed,
        Accepted,
        Canceled,
        Successful,
        Unsuccessful
    }
}