using System;
using Map.DataRepresentation;

namespace Simulation {
    [Serializable]
    public class Transfer {
        public StopGroup stop;
        public LineDirection direction;

        public Transfer(StopGroup stop, LineDirection direction) {
            this.stop = stop;
            this.direction = direction;
        }
    }
}
