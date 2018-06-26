using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class LocationBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public double XCoordinate { get; set; }
        
        public double YCoordinate { get; set; }

        public String StreetName { get; set; }

        public String Place { get; set; }

        public ushort ZipCode { get; set; }

        public LocationBase()
        {
            StreetName = "";
            Place = "";
            ZipCode = 0;
            XCoordinate = 0;
            YCoordinate = 0;
        }
    }
}