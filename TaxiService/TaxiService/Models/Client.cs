using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class Client : UserBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int ID { get; set; }

        public Client() : base() { }
    }
}