using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Quasar.Web.Models;
using Quasar.Core;

namespace Quasar.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/package")]
    public class PackageController : Controller
    {
        PackageManager _packageManager;
        IQuasarConfiguration _config;

        public PackageController(PackageManager packageManager, IQuasarConfiguration quasarConfiguration)
        {
            this._packageManager = packageManager;
            this._config = quasarConfiguration;
        }

        // POST api/package
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Package))]
        [ProducesResponseType(404)]
        public IActionResult Post([FromBody]PackageStoreRequest request)
        {
            var product = _config.Products.Where(p => p.Name == request.ProductName).FirstOrDefault();
            if (product == null)
                return NotFound();
            var package = _packageManager.StorePackage(product, request.PackageFileReference);
            return Ok(package);
        }
    }
}