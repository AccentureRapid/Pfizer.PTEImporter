using Abp.Dependency;
using Pfizer.PTEImporter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter.Services
{
    public interface IImporterService : ITransientDependency
    {
        Task<List<EpayRawDataLandingModel>> ReadDataSource(string fullPath);
    }
}
