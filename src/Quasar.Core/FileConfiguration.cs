using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Quasar.Core
{
    public class FileConfiguration : IQuasarConfiguration
    {

        public ServerType[] Servers { get; set; }
        public Product[] Products { get; set; }
        public ProductEnvironment[] Environments { get; set; }

        public void LoadConfiguration(string confPath)
        {
            var js = JsonSerializer.Create();
            using (var reader = File.OpenText(Path.Combine(confPath, "servers.json")))
            {
                Servers = js.Deserialize<ServerType[]>(new JsonTextReader(reader));
            }
            using (var reader = File.OpenText(Path.Combine(confPath, "products.json")))
            {
                Products = js.Deserialize<Product[]>(new JsonTextReader(reader));
            }
            using (var reader = File.OpenText(Path.Combine(confPath, "envs.json")))
            {
                Environments = js.Deserialize<ProductEnvironment[]>(new JsonTextReader(reader));
            }
        }
    }
}
