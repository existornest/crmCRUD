using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortalCRM.Models
{
    public class PortalModelContext: DbContext
    {
        public DbSet<PortalModel> dbContext { get; set; }
    }
}