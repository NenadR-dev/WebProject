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

        public virtual DbSet<Client> ClientDb { get; set; }
        public virtual DbSet<Driver> DriverDb { get; set; }
        public virtual DbSet<Dispacher> DispacherDb { get; set; }
        public virtual DbSet<RideBase> RideDb { get; set; }

    }
}