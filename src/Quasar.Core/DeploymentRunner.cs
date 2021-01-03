using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Quasar.Core
{
    public class DeploymentRunner
    {
        string ScriptPath = Path.GetFullPath(string.Format("..{0}..{0}..{0}scripts", Path.DirectorySeparatorChar));
        string PackageRepoPath = Path.GetFullPath(string.Format("..{0}..{0}..{0}packages", Path.DirectorySeparatorChar));
        ILogger<DeploymentRunner> _logger;
        IServiceProvider _serviceProvider;
        IDeploymentPublisher _publisher;

        public DeploymentRunner(ILogger<DeploymentRunner> logger, IDeploymentPublisher publisher, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this._publisher = publisher;
            this._serviceProvider = serviceProvider;
        }

        public void Deploy(Package package, ProductEnvironment env)
        {
            foreach (var server in env.Servers.Where(s=>s.ServerType.Name==package.Product.ServerType.Name))
            {
                var PackagePath = Path.Combine(PackageRepoPath, package.PackageID.Replace("/", Path.DirectorySeparatorChar.ToString()));
                var runFilename = Path.Combine(PackagePath, string.Format("run.{0}.{1}.{2:yyyyMMdd.HHmmss}.json", env.Name, server.Name, DateTime.Now));
                var logFilename = runFilename.Replace(".json", ".log");
                var deploymentRun = new DeploymentRun()
                {
                    id = Guid.NewGuid(),
                    Server = server,
                    Variables = env.Variables,
                    Product = package.Product,
                    EnvironmentName = env.Name,
                    DeploymentStatus = DeploymentStatus.Pending
                };
                NLog.MappedDiagnosticsContext.Set("Deploy.LogPath", logFilename);

                _logger.LogInformation("Deploying {0} to {1}", package.PackageID, server.Name);
                var script = Path.Combine(ScriptPath, package.Product.DeployScript);
                _logger.LogInformation("Execute: {0}", script);
                foreach (var variable in env.Variables)
                {
                    _logger.LogInformation("Variable: {0}={1}", variable.Name, variable.Value);
                }

                // Output the deployment parameters to a json run file
                using (var runFile = File.CreateText(runFilename))
                {
                    JsonSerializer.Create().Serialize(runFile, deploymentRun);
                }

                ExecuteDeploymentProcess(runFilename, deploymentRun, script);
            }
        }

        private void ExecuteDeploymentProcess(string runFilename, DeploymentRun deploymentRun, string scriptPath)
        {
            // Construct deployment runner based on script type
            IExternalProcessRunner processRunner;
            if (scriptPath.EndsWith("ps1"))
                processRunner = _serviceProvider.GetRequiredService<PowershellRunner>();
            else
                processRunner = _serviceProvider.GetRequiredService<ProcessRunner>();

            // Perform deployment and publish results
            _publisher.BeginDeploymentRun(deploymentRun);
            // Run process to perform deployment using the configuration from the run file
            var exitCode = processRunner.ExecuteProcess(deploymentRun, ScriptPath, scriptPath, runFilename);
            if (exitCode == 0)
                deploymentRun.DeploymentStatus = DeploymentStatus.Success;
            else
                deploymentRun.DeploymentStatus = DeploymentStatus.Failure;
            _logger.LogInformation("Deployment Status: {0}, exit code={1}", deploymentRun.DeploymentStatus, exitCode);
            _publisher.EndDeploymentRun(deploymentRun);
        }
    }
}
