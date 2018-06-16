using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using TaxiService.Models;

namespace TaxiService.App_Start
{
    public class DatabaseConfig: DbMigrationsConfiguration<DataAccess>
    {
        public DatabaseConfig()
        {
            AutomaticMigrationDataLossAllowed = true;
            AutomaticMigrationsEnabled = true;
            ContextKey = "TaxiServiceDB";
        }
    }
}