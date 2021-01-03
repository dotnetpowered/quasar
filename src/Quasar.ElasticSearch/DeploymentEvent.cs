using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Quasar.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quasar.ElasticSearch
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventType { Start, Complete }

    public class DeploymentEvent
    {
        public Guid id { get; set; }
        [JsonProperty("@timestamp")]
        public DateTime Timestamp { get; set; }
        public Guid DeploymentRunId { get; set; }
        public DeploymentRun DeploymentRun { get; set; }
        public string Description { get; set; }
        public EventType EventType { get; set; }
    }
}
