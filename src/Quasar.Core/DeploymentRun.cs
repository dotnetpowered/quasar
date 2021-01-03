using System;
using System.Collections.Generic;

namespace Quasar.Core
{
    public enum DeploymentStatus
    {
        Pending,
        Success,
        Failure
    }

    public class DeploymentRun
    {
        public Guid id { get; set; }
        public Product Product { get; set; }
        public Server Server { get; set; }
        public string EnvironmentName { get; set; }
        public IList<DeploymentVariable> Variables { get; set; }
        public DeploymentStatus DeploymentStatus { get; set; }
    }
}
