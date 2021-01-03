using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.Core
{
    public class ProductEnvironment
    {
        List<Server> _Servers;
        List<DeploymentVariable> _Variables;

        public ProductEnvironment()
        {
            _Servers = new List<Server>();
            _Variables = new List<DeploymentVariable>();
        }

        public string Name { get; set;  }
        public IList<Server> Servers { get { return _Servers; } }
        public IList<DeploymentVariable> Variables { get { return _Variables; } }
    }
}
