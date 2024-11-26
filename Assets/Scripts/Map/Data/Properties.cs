using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Map.Data {
    [Serializable]
    public class Properties {
        [JsonProperty("@id")]
        public string ID { get; set; }
        [JsonProperty("departures_board")]

        public string DeparturesBoard { get; set; }
        public string Name { get; set; }
        public string Network { get; set; }
        [JsonProperty("network:wikidata")]
        public string NetworkWikidata { get; set; }
        [JsonProperty("operator")]

        public string Operator { get; set; }
        [JsonProperty("operator:wikidata")]

        public string OperatorWkidata { get; set; }
        public string Platforms { get; set; }
        [JsonProperty("public_transport")]
        public string PublicTransport { get; set; }
        public string Railway { get; set; }
        [JsonProperty("railway:position")]
        public string RailwayPosition { get; set; }
        [JsonProperty("railway:position:exact")]
        public string RailwayPositionExact { get; set; }
        [JsonProperty("railway:ref")]
        public string RailwayRef { get; set; }
        public string Station { get; set; }
        public string Subway { get; set; }
        [JsonProperty("uic_name")]
        public string IicName { get; set; }
        public string Wheelchair { get; set; }
        public string Wikidata { get; set; }
        public string Wikipedia { get; set; }
    }
}