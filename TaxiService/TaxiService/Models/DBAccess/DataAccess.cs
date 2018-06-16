using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaxiService.Models
{
    public class DataAccess : DbContext
    {
        private static DataAccess data = null;

        public static  DataAccess CreateDb()
        {
            if(data == null)
            {
                data = new DataAccess();
            }
            return data;
        }
        private DataAccess() : base("TaxiServiceDB") { }

        public virtual DbSet<Client> clientDb { get; set; }
        public virtual DbSet<Driver> driverDb { get; set; }
        public virtual DbSet<Dispacher> DispacherDb { get; set; }
    }
}