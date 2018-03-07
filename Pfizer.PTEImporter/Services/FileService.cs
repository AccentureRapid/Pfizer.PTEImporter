using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter.Services
{
    public class FileService : IFileService
    {
        public async Task<List<string>> ReadTextFile(string fullPath)
        {
            FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string line = string.Empty;
            List<string> result = new List<string>();
            while ((line = await reader.ReadLineAsync()) != null)
            {
                result.Add(line);
            }

            return result;
        }

        public async Task<string> ReadAllText(string fullPath)
        {
            FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);

            var result = await reader.ReadToEndAsync();
            return result;
        }

        public async Task Copy(string source, string destination)
        {
            await Task.Run(() => {
                File.Copy(source, destination, true);
            });
        }

        public async Task Delete(string path)
        {
            await Task.Run(() => {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            });
        }
        public async Task MakeSureDirectoryExist(string path)
        {
            await Task.Run(() => {
                DirectoryInfo directory = new DirectoryInfo(path);
                if (!directory.Exists)
                {
                    directory.Create();
                }
            });
        }
    }
}
