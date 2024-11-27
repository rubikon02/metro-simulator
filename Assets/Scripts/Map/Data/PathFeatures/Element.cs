using System;
using System.Collections.Generic;

namespace Map.Data {
    [Serializable]
    public class Element {
        public string Type { get; set; }
        public int ID { get; set; }
        public Bounds Bounds { get; set; }
        public List<Member> Members { get; set; }
        public Tags Tags { get; set; }
    }
}
