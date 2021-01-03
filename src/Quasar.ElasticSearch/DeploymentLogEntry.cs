using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.ElasticSearch
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntryType { Error, Info }

    public class DeploymentLogEntry
    {
        public Guid id { get; set; }
        [JsonProperty("@timestamp")]
        public DateTime Timestamp { get; set; }
        public Guid DeploymentRunId { get; set; }
        public string Message { get; set; }
        public EntryType MessageType { get; set; }
    }
}
