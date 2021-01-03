using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Models
{
    public class PackageStoreRequest
    {
        [Required]
        public string ProductName { get; set; }
        public string PackageFileReference { get; set; }
    }
}
