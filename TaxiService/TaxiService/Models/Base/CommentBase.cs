using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    
    public class CommentBase
    {
        [Key,ForeignKey("CommentRide")]
        public int ID { get; set; }

        public String Summary { get; set; }

        public DateTime CommentDate { get; set; }

        public virtual Client CommentClient { get; set; }

        public virtual RideBase CommentRide { get; set; }

        [Range(0,5)]
        public ushort Stars { get; set; }
    }
}