using System.Collections.Generic;
using System.Linq;
using Map.DataRepresentation;

namespace Simulation
{
    public class MetroPathfinder {
        public static List<Transfer> FindShortestPath(StopGroup start, StopGroup destination) {
            var visited = new HashSet<StopGroup>();
            var queue = new Queue<List<Transfer>>();

            queue.Enqueue(new List<Transfer> { new(start, null) });

            while (queue.Count > 0) {
                var path = queue.Dequeue();
                var current = path.Last().stop;

                if (current == destination){
                    var directions0 = path[0].stop.GetSubwayLines()
                        .SelectMany(line => line.directions).ToList();
                    var directions1 = path[1].stop.GetSubwayLines()
                        .SelectMany(line => line.directions).ToList();
                    foreach(var dir in directions0) {
                        var stops = dir.stops.Select(stop => stop.group).ToList();
                        int s0_index = stops.IndexOf(path[0].stop);
                        foreach(var dir1 in directions1) {
                            var stops1 = dir1.stops.Select(stop => stop.group).ToList();
                            if(stops1.IndexOf(path[0].stop) >= 0 && stops1.IndexOf(path[0].stop) == s0_index) {
                                path[0].direction = dir;
                            }
                        }
                    }
                    return new List<Transfer>(path);
                }
                if (visited.Contains(current)) continue;

                visited.Add(current);

                foreach (var direction in current.GetSubwayLines().SelectMany(line => line.directions)) {
                    foreach (var neighbor in GetNeighbors(current, direction)) {
                        if (!visited.Contains(neighbor)) {
                            var newPath = new List<Transfer>(path) { new(neighbor, direction) };
                            queue.Enqueue(newPath);
                        }
                    }
                }
            }
            return null;
        }

        private static IEnumerable<StopGroup> GetNeighbors(StopGroup current, LineDirection direction) {
            var stopGroups = direction.stops.Select(stop => stop.group).ToList();
            int index = stopGroups.IndexOf(current);
            if (index >= 0) {
                if (index < stopGroups.Count - 1) yield return stopGroups[index + 1];
            }
        }
    }
}