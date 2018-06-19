using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class RideStatus
    {
        public static string Created = "Created";
        public static string Formated = "Formated";
        public static string Processed = "Processed";
        public static string Accepted = "Accepted";
        public static string Canceled = "Canceled";
        public static string Successful = "Successful";
        public static string Unsuccessful = "Unsuccessful";
    }
}