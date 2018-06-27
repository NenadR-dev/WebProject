using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiService.Models.Base
{
    public class FilterBase
    {
        public String FilterStatus { get; set; }

        public String SortDate { get; set; }
        public String SortGrade { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int FromGrade { get; set; }
        public int ToGrade { get; set; }
        public int FromPrice { get; set; }
        public int ToPrice { get; set; }
        public String FirstnameDriver { get; set; }
        public String LastnameDriver { get; set; }
        public String FirstnameClient { get; set; }
        public String LastnameClient { get; set; }
    }
}