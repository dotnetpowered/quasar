using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.Core
{
    public interface IDeploymentPublisher
    {
        void BeginDeploymentRun(DeploymentRun deploymentRun);
        void DeploymentRunInfo(DeploymentRun deploymentRun, string info);
        void DeploymentRunError(DeploymentRun deploymentRun, string error);
        void EndDeploymentRun(DeploymentRun deploymentRun);
    }
}
