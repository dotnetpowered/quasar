using Quasar.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.ElasticSearch
{
    public class DeploymentInstance
    {
        public string id { get; set; }
        public Product product { get; set; }
        public string environment { get; set; }
        public DateTime latestDeployment { get; set; }
        public Guid deploymentRunId { get; set; }
    }
}
