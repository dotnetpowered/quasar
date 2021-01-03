namespace Quasar.Core
{
    public interface IExternalProcessRunner
    {
        int ExecuteProcess(DeploymentRun deploymentRun, string workingDirectory, string filename, string args);
    }
}