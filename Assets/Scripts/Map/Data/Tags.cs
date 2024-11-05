using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Map.Data {
    [Serializable]
    public class Tags {
        [JsonProperty("by_night")]
        public string ByNight { get; set; }
        [JsonProperty("by_night:weekend")]
        public string ByNightWeekend { get; set; }
        public string Colour { get; set; }
        public string From { get; set; }
        public string Interval { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Network { get; set; }
        [JsonProperty("network:wikidata")]
        public string NetworkWikidata { get; set; }
        [JsonProperty("opening_hours")]
        public string OpeningHours { get; set; }
        public string Operator { get; set; }
        [JsonProperty("operator:wikidata")]
        public string OperatorWikidata { get; set; }
        [JsonProperty("public_transport:version")]
        public string PublicTransportVersion { get; set; }
        [JsonProperty("railway:lzb")]
        public string RailwayLzb { get; set; }
        public string Ref { get; set; }
        public string Route { get; set; }
        public string To { get; set; }
        public string Type { get; set; }
        public string Wikidata { get; set; }
        public string Wikipedia { get; set; }
        [JsonExtensionData]
        public Dictionary<string, JToken> DynamicFields { get; set; } = new();
    }
}