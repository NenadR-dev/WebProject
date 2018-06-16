using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class Driver : UserBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int ID { get; set; }

        public LocationBase Location { get; set; }

        public virtual CarBase Car { get; set; }

        public Driver() : base()
        {
            Car = new CarBase();
            Location = new LocationBase();
        }

    }
}