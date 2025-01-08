using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using Map.DataRepresentation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Simulation {
    public class PassengerSpawner : MonoBehaviour {
        [SerializeField] private Passenger passengerPrefab;
        [SerializeField] private float spawnInterval = 2f;
        private bool from = false;

        private void Start() {
            InvokeRepeating(nameof(GeneratePassenger), 0, spawnInterval);
        }

        private void GeneratePassenger() {
            if(from) return;
            var startStop = GetRandomStop();

            // StopGroup GetRandomDestinationStopStrategy() => GetRandomStop();
            StopGroup GetRandomDestinationStopStrategy() => GetRandomStopOnTheSameLine(startStop);

            var destinationStop = GetRandomDestinationStopStrategy();
            while (startStop == destinationStop) {
                destinationStop = GetRandomDestinationStopStrategy();
            }

            // var startStop = SubwayLineGenerator.I.stopGroups[0];
            // var destinationStop = SubwayLineGenerator.I.stopGroups[1];
            Debug.Log("From: " + startStop.name);
            Debug.Log("To: " + destinationStop.name);

            var passenger = Instantiate(passengerPrefab, startStop.transform.position, Quaternion.identity);
            startStop.AddPassenger(passenger);
            passenger.SetDestination(destinationStop);
            passenger.SetColor(Color.magenta);
            from = true;
        }

        private StopGroup GetRandomStop() {
            var stops = SubwayLineGenerator.I.stopGroups;
            return stops[Random.Range(0, stops.Count)];
        }

        private StopGroup GetRandomStopOnTheSameLine(StopGroup stopGroup) {
            var stopsOnSameLine = stopGroup
                .GetSubwayLines()
                .SelectMany(line => line.directions)
                .SelectMany(direction => direction.stops)
                .Select(stop => stop.group)
                .ToList();

            return stopsOnSameLine[Random.Range(0, stopsOnSameLine.Count)];
        }

        
        // private void GeneratePassenger() {
        //     var line = GetRandomLine();
        //     var direction = GetRandomDirection(line);
        //     var stops = GetRandomStop(direction);
        //     var startStop = stops[0];
        //     var destinationStop = stops[1];
        //     if(from) {
        //         Debug.Log("Line: " + line.name);
        //         Debug.Log("Direction: " + direction.name);
        //         Debug.Log("From: " + startStop.name);
        //         Debug.Log("To: " + destinationStop.name);
        //         from = false;
        //     }
        //     // // var startStop = SubwayLineGenerator.I.stopGroups[1];

        //     // // StopGroup GetRandomDestinationStopStrategy() => GetRandomStop();
        //     // StopGroup GetRandomDestinationStopStrategy() => GetRandomStopOnTheSameLine(startStop);

        //     // var destinationStop = GetRandomDestinationStopStrategy();
        //     // while (startStop == destinationStop) {
        //     //     destinationStop = GetRandomDestinationStopStrategy();
        //     // }
        //     var passenger = Instantiate(passengerPrefab, startStop.transform.position, Quaternion.identity);
        //     startStop.AddPassenger(passenger);
        //     passenger.SetDestination(destinationStop);
        // }

        // private SubwayLine GetRandomLine() {
        //     var lines = SubwayLineGenerator.I.subwayLines;
        //     return lines[Random.Range(0, lines.Count)];
        // }

        // private LineDirection GetRandomDirection(SubwayLine subwayLine) {
        //     var directions = subwayLine.directions;
        //     return directions[Random.Range(0, directions.Count)];
        // }

        // private List<StopGroup> GetRandomStop(LineDirection direction) {
        //     var stops = direction.stops;

        //     System.Random random = new System.Random();
        //     int number1 = random.Next(0, stops.Count);
        //     int number2;
        //     do {
        //         number2 = random.Next(0, stops.Count);
        //     }
        //     while (number1 == number2);

        //     if(number1 > number2) {
        //         int temp = number1;
        //         number1 = number2;
        //         number2 = temp;
        //     }

        //     return new List<StopGroup>() {stops[number1], stops[number2]};
        // }
    }

}
