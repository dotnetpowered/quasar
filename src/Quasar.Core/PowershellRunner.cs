using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Quasar.Core
{
    public class PowershellRunner : ProcessRunner
    {
        public PowershellRunner(ILogger<PowershellRunner> logger, IDeploymentPublisher publisher) : base(logger, publisher)
        {
        }

        public override int ExecuteProcess(DeploymentRun deploymentRun, string workingDirectory, string script, string runFilename)
        {
            var filename = "powershell.exe";
            var args = "&'" + script + "' " + runFilename;
            return base.ExecuteProcess(deploymentRun, workingDirectory, filename, args);
        }
    }
}
