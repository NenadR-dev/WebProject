using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public abstract class UserBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int ID { get; set; }

        public String Username { get; set; }
         
        public String Password { get; set; }

        public String Firstname { get; set; }

        public String Lastname { get; set; }

        public String Gender { get; set; }

        public String JMBG { get; set; }

        public int ContactPhone { get; set; }

        public String Email { get; set; }

        public String Role { get; set; }

        public ICollection<RideBase> RideList { get; set; }
    }
}