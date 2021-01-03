using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Quasar.Core
{
    public class ProcessRunner : IExternalProcessRunner
    {
        private readonly ILogger _logger;
        private readonly IDeploymentPublisher _publisher;

        public ProcessRunner(ILogger<ProcessRunner> logger, IDeploymentPublisher publisher)
        {
            this._logger = logger;
            this._publisher = publisher;
        }

        public virtual int ExecuteProcess(DeploymentRun deploymentRun, string workingDirectory, string filename, string args)
        {
            var process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = args;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += (sender, data) =>
            {
                if (data.Data != null)
                {
                    _logger.LogInformation(data.Data);
                    _publisher.DeploymentRunInfo(deploymentRun, data.Data);
                }
            };
            process.StartInfo.RedirectStandardError = true;
            process.ErrorDataReceived += (sender, data) =>
            {
                if (data.Data != null)
                {
                    _logger.LogError(data.Data);
                    _publisher.DeploymentRunError(deploymentRun, data.Data);
                }
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
