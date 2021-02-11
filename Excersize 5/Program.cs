using System;
using System.Collections.Generic;
using System.Threading;

namespace Excersize_5
{
    class Program
    {
        static List<Thread> _allThreads = new List<Thread>();
        static void Main(string[] args)
        {
            SynchronizedRandomGenerator numGen = new SynchronizedRandomGenerator(0, 100);
            int numberOfCars = 0;
            do
            {
                Console.WriteLine($"Hello. Please write a number of cars to participate in the race: ");
            }
            while (!int.TryParse(Console.ReadLine(), out numberOfCars));

            List<Car> cars = new List<Car>();
            for (int i = 0; i < numberOfCars; i++)
            {
                cars.Add(new Car(i + 1, numGen, 1000));
            }
            foreach (Car car in cars)
            {
                Thread thread = new Thread(car.Race);
                _allThreads.Add(thread);
                thread.Start();
            }
            WaitUntilAllThreadsComplete();
            Console.WriteLine("\n************ All cars have finished! ************\n" +
                "Exiting in 5 seconds...");
            Thread.Sleep(5000);
            Console.ReadLine();

        }
        private static void WaitUntilAllThreadsComplete()
        {
            foreach (Thread thread in _allThreads)
            {
                thread.Join();
            }
        }
        // A class which can return Randomized numbers in a 
        // synchronized manner.
        public sealed class SynchronizedRandomGenerator
        {
            // Initialize the random number generator.
            // Instantiate a "System.Random" object internally to generate
            // random numbers.
            private Random _random;
            private int _minValue;
            private int _maxValue;
            private object baton = new object();
            public SynchronizedRandomGenerator(int minValue, int maxValue)
            {
                _random = new Random();
                _minValue = minValue;
                _maxValue = maxValue;
            }
            // Use the "Next(minValue, maxValue)" method on
            // a "System.Random" object to get a random number
            // within the requested range.
            // Note that the "System.Random" object itself is not thread
            // safe.
            public int Next()
            {
                lock (baton)
                {
                    return _random.Next(_minValue, _maxValue);                    
                }
            }
        }
        public sealed class Car
        {
            // Initialize the car with a random generator
            // and destination number of kilomteres required
            // to finish the race.
            private SynchronizedRandomGenerator _numGen;
            private int _destKm;
            public int _carId { get; private set; }
            private int _totalKm;
            public Car(int carId, SynchronizedRandomGenerator randomGenerator, int destKm)
            {
                _carId = carId;
                _numGen = randomGenerator;
                _destKm = destKm;
                _totalKm = 0;
            }
            // A car should (in a loop):
            //   1. Generate a random number of kilometers
            //      past by using the random generator.
            //   2. Sum the number of kilometers past, 
            //      and write an update to the console.
            //   3. Sleep for 10 milliseconds (Thread.Sleep(10)).
            //   4. Once the destination number of kilometers is 
            //      achieved - write the name of the car to the 
            //      console stating that it finished.
            // Note that this method should be executed on a dedicated 
            // thread.
            public void Race(object obj)
            {
                while (_destKm > _totalKm)
                {
                    _totalKm += _numGen.Next();
                    Console.WriteLine($"Car {_carId} has passed {(_totalKm > 1000 ? 1000 : _totalKm)} kilometers");
                    Thread.Sleep(100);
                }
                Console.WriteLine($"Car {_carId} has finished the race!");
            }
        }

    }
}

