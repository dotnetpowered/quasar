using System;
using System.Collections.Generic;
using System.Text;
using Quasar.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;

namespace QuasarCli
{
    public class App
    {
        private readonly PackageManager _packman;
        private readonly DeploymentRunner _runner;
        private readonly ILogger<App> _logger;

        public App(PackageManager packman, DeploymentRunner runner, ILogger<App> logger)
        {
            this._packman = packman;
            this._runner = runner;
            this._logger = logger;
        }

        public void Run()
        {
            var confPath = Path.GetFullPath(string.Format("..{0}..{0}conf", Path.DirectorySeparatorChar));

            ServerType[] servers;
            Product[] products;
            ProductEnvironment[] environments;

            var js = JsonSerializer.Create();
            using (var reader = File.OpenText(Path.Combine(confPath, "servers.json")))
            {
                servers = js.Deserialize<ServerType[]>(new JsonTextReader(reader));
            }
            using (var reader = File.OpenText(Path.Combine(confPath, "products.json")))
            {
                products = js.Deserialize<Product[]>(new JsonTextReader(reader));
            }
            using (var reader = File.OpenText(Path.Combine(confPath, "envs.json")))
            {
                environments = js.Deserialize<ProductEnvironment[]>(new JsonTextReader(reader));
            }

            _logger.LogInformation("Staging Deployment Package");
            var package = _packman.StorePackage(products[0], @"c:\temp\ea.zip");
            _logger.LogInformation("Package stored: {0}", package.PackageID);

            _runner.Deploy(package, environments[0]);
        }


        private void WriteSampleConfig()
        {
            var appServer = new ServerType() { Name = "App", OperatingSystem = OS.Windows };
            var product = new Product() { Name = "SearchApi", ServerType = appServer, DeployScript = "echo.ps1" };
            var env = new ProductEnvironment() { Name = "QA" };
            env.Servers.Add(new Server() { Name = "fx2-dev-app", ServerType = appServer });
            env.Variables.Add(new DeploymentVariable { Name = "dbserver", Value = "fx2-dev-sql" });

            var servers = new ServerType[] { appServer };
            var products = new Product[] { product };
            var envs = new ProductEnvironment[] { env };

            var js = JsonSerializer.Create();
            using (var runFile = File.CreateText("servers.json"))
            {
                js.Serialize(runFile, servers);
            }
            using (var runFile = File.CreateText("products.json"))
            {
                js.Serialize(runFile, products);
            }
            using (var runFile = File.CreateText("envs.json"))
            {
                js.Serialize(runFile, envs);
            }
        }
    }
}
