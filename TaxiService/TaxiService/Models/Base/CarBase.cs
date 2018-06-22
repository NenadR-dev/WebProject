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
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int OwnerID { get; set; }
        //public Driver Owner { get; set; }

        public int CarAge { get; set; }

        public String CarRegistration { get; set; }

        public int TaxiCarID { get; set; }

        public String CarType { get; set; }
    }
}