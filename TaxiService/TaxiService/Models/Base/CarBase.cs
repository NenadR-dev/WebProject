using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class CarBase
    {
        [Key,ForeignKey("Owner")]
        public int ID { get; set; }

        public Driver Owner { get; set; }

        public ushort CarAge { get; set; }

        public String CarRegistration { get; set; }

        public ushort TaxiCarID { get; set; }

        public String CarType { get; set; }
    }
}