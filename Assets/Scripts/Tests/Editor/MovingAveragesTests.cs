using System.Collections.Generic;
using HandcraftedGames.Common;
using NUnit.Framework;
using Unity.PerformanceTesting;
using System.Linq;

namespace HandcraftedGames.Common.Tests
{
    public class MATests
    {
        [Test]
        public void TestShiftAndAppend()
        {
            var collection = new int[] { 0, 1, 2, 3, 4 };
            collection.ShiftAndAppend(5);
            Assert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, collection);
        }

        [Test]
        public void TestMANegativePeriod()
        {
            var collection = new List<double>();
            Assert.Throws<System.Exception>(() => collection.MovingAverage(-1));
        }

        [Test]
        public void TestMA()
        {
            var collection = new List<double>() { 5, 6, 7, 8, 9 };
            Assert.AreEqual(7.0, collection.MovingAverage(5));
        }

        [Test]
        public void TestMAWithSmallerPeriod()
        {
            var collection = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Assert.AreEqual(7.0, collection.MovingAverage(5));
        }

        [Test]
        public void TestMAWithOneElement()
        {
            var collection = new List<double>() { 5, 6, 7, 8, 9 };
            Assert.AreEqual(9.0, collection.MovingAverage(1));
        }

        [Test]
        public void TestMAWithLargerPeriodThanSize()
        {
            var collection = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Assert.AreEqual(4.5, collection.MovingAverage(20));
        }

        [Test, Performance]
        public void TestPerformance()
        {
            var collection = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Measure.Method(() =>
            {
                collection.MovingAverage(10);
            })
            .WarmupCount(10)
            .MeasurementCount(50)
            .IterationsPerMeasurement(5000)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void TestPerformanceShift()
        {
            var collection = new int[] { 0, 1, 2, 3, 4 };

            Measure.Method(() =>
            {
                collection.ShiftAndAppend(5);
            })
            .WarmupCount(10)
            .MeasurementCount(50)
            .IterationsPerMeasurement(5000)
            .GC()
            .Run();
        }
    }
}