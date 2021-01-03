using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Quasar.Core;
using Quasar.Web.Models;

namespace Quasar.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/deploy")]
    public class DeployController : Controller
    {
        DeploymentRunner _deploymentRunner;
        IQuasarConfiguration _config;
        PackageManager _packageManager;

        public DeployController(DeploymentRunner deploymentRunner, IQuasarConfiguration quasarConfiguration, PackageManager packageManager)
        {
            this._packageManager = packageManager;
            this._deploymentRunner = deploymentRunner;
            this._config = quasarConfiguration;
        }

        // POST api/deploy
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Package))]
        [ProducesResponseType(404)]
        public IActionResult Post([FromBody]DeploymentRequest request)
        {
            var environment = _config.Environments.Where(e => e.Name == request.EnvironmentName).FirstOrDefault();
            if (environment == null)
                return NotFound();
            var package = _packageManager.LoadPackage(request.PackageId);
            _deploymentRunner.Deploy(package, environment);
            return Ok();
        }
    }
}