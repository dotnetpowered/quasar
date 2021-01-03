using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models
{
    public class DeploymentRequest
    {
        public string PackageId { get; set; }
        public string EnvironmentName { get; set; }
    }
}
