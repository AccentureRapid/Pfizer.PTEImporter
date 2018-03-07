using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter.Services
{
    public interface IFileService : ITransientDependency
    {
        Task<List<string>> ReadTextFile(string fullPath);
        Task<string> ReadAllText(string fullPath);
        Task Copy(string source, string destination);
        Task Delete(string path);
        Task MakeSureDirectoryExist(string path);
    }
}
