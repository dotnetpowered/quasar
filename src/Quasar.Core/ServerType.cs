using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OS { Linux, Windows, Mac }

    public class ServerType
    {
        public string Name { get; set; }
        public OS OperatingSystem { get; set; }
    }
}
