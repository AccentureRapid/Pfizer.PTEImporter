using System.Data.Entity;
using Abp.EntityFramework;


namespace Pfizer.PTEImporter
{
    //EF DbContext class.
    public class PTEImporterDbContext : AbpDbContext
    {
       
        //public virtual IDbSet<SystemParameter> SystemParameter { get; set; }
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