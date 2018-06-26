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
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public String Summary { get; set; }

        public String CommentDate { get; set; }

        public String OriginalPoster { get; set; }
        //public virtual Client ClientID { get; set; }

        public int RideID { get; set; }
        //public virtual RideBase CommentRide { get; set; }

        public int Stars { get; set; }
    }
}