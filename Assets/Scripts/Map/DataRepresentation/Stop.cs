using Map.Data;

namespace Map.DataRepresentation {
    public class Stop : MapElement {
        public Coordinates coordinates;
        public StopGroup group;

        public SubwayLine GetSubwayLine() {
            return GetComponentInParent<SubwayLine>();
        }

        public void SetGroup(StopGroup newGroup) {
            group = newGroup;
        }
    }
}
