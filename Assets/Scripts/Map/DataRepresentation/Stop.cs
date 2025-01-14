using Map.Data;

namespace Map.DataRepresentation {
    public class Stop : MapElement {
        public Coordinates coordinates;
        public StopGroup group;
        public SubwayLine line;

        public void SetGroup(StopGroup newGroup) {
            group = newGroup;
        }
    }
}
