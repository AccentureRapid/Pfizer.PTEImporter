using System.Data.Entity;
using Abp.EntityFramework;
using Pfizer.PTEImporter.Core.Entities;

namespace Pfizer.PTEImporter
{
    //EF DbContext class.
    public class PTEImporterDbContext : AbpDbContext
    {
       
        public virtual IDbSet<EpayRawDataLanding> EpayRawDataLanding { get; set; }
        public virtual IDbSet<Employee> Employee { get; set; }
        public PTEImporterDbContext()
            : base("Default")
        {
            this.Database.CommandTimeout = 0;
        }

        public PTEImporterDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.Database.CommandTimeout = 0;
        }
    }
}