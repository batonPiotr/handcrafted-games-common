using System.Collections.Generic;
using HandcraftedGames.Common;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace HandcraftedGames.Utils.Tests
{
    public class TakeLastTests
    {
        [Test]
        public void TestNLargerThanSize()
        {
            var collection = new List<double>() { 5, 6, 7, 8, 9 };
            var expected = new List<double>() { 5, 6, 7, 8, 9 };
            Assert.AreEqual(expected, collection.TakeLast(15));
        }

        [Test]
        public void TestNSmallerThanSize()
        {
            var collection = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var expected = new List<double>() { 5, 6, 7, 8, 9 };
            Assert.AreEqual(expected, collection.TakeLast(5));
        }

        [Test, Performance]
        public void TestPerformance()
        {
            var collection = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Measure.Method(() =>
            {
                collection.TakeLast(5);
            })
            .WarmupCount(10)
            .MeasurementCount(50)
            .IterationsPerMeasurement(5000)
            .GC()
            .Run();
        }
    }
}