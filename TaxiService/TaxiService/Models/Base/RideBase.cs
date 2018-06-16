using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class RideBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime RiderOrderDate { get; set; }

        public LocationBase Location { get; set; }

        public CarRole CarType { get; set; }

        public virtual Client RideClient { get; set; }

        public LocationBase Destination { get; set; }

        public virtual Dispacher AllocatedDispacher { get; set; }

        public virtual Driver TaxiDriver { get; set; }

        public double RidePrice { get; set; }

        public virtual CommentBase Comment { get; set; }

        public RideStatus Status { get; set; }
    }
}