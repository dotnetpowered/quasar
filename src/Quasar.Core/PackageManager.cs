using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Quasar.Core
{
    public class PackageManager
    {
        private readonly string PackageRepoPath = Path.GetFullPath(string.Format("..{0}..{0}packages", Path.DirectorySeparatorChar));
        ILogger<PackageManager> _logger;

        public PackageManager(ILogger<PackageManager> logger)
        {
            this._logger = logger;
        }

        public Package LoadPackage(string PackageId)
        {
            string PackagePath = Path.Combine(PackageRepoPath, PackageId.Replace('/', Path.DirectorySeparatorChar));
            var js = JsonSerializer.Create();
            using (var reader = File.OpenText(Path.Combine(PackagePath, "package.json")))
            {
                return js.Deserialize<Package>(new JsonTextReader(reader));
            }
        }

        public Package StorePackage(Product product, string PackageSourcePath)
        {
            Guid deploymentUUID = Guid.NewGuid();
            string yearMon = (DateTime.Now.Year * 100 + DateTime.Now.Month).ToString();
            string productName = product.Name.Replace(" ", "_");

            string ProductPath = Path.Combine(PackageRepoPath, productName);
            string YearMonPath = Path.Combine(ProductPath, yearMon);
            string PackagePath = Path.Combine(YearMonPath, deploymentUUID.ToString());
            string FileExt = Path.GetExtension(PackageSourcePath);

            _logger.LogInformation("Storing {0} package {1} to {2}",productName,deploymentUUID,PackagePath);

            Directory.CreateDirectory(PackagePath);
            File.Copy(PackageSourcePath, Path.Combine(PackagePath,"package"+FileExt));

            var package = new Package()
            {
                PackageID = productName + "/" + yearMon + "/" + deploymentUUID,
                Product = product
            };
            using (var packageFile = File.CreateText(Path.Combine(PackagePath, "package.json")))
            {
                JsonSerializer.Create().Serialize(packageFile, package);
            }
            return package;
        }
    }
}
