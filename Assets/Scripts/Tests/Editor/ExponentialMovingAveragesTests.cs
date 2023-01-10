using System.Collections.Generic;
using HandcraftedGames.Common;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace HandcraftedGames.Utils.Tests
{
    public class EMATests
    {
        [Test]
        public void TestEMANegativePeriod()
        {
            var collection = new List<double>();
            Assert.Throws<System.Exception>(() => collection.ExponentialMovingAverage(-1));
        }

        [Test]
        public void TestEMA()
        {
            var collection = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Assert.AreEqual(8.001953125, collection.ExponentialMovingAverage(3));
        }

        [Test]
        public void TestEMASameValues()
        {
            var collection = new List<double>() { 1, 1, 1, 1, 1 };
            Assert.AreEqual(1, collection.ExponentialMovingAverage(5));
        }

        [Test]
        public void TestEMAWithSmallerPeriod()
        {
            var collection = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Assert.AreEqual(7.0520245897474991, collection.ExponentialMovingAverage(5));
        }

        [Test]
        public void TestEMAWithOneElement()
        {
            var collection = new List<double>() { 5, 6, 7, 8, 9 };
            Assert.AreEqual(9.0, collection.ExponentialMovingAverage(1));
        }

        [Test]
        public void TestEMAWithLargerPeriodThanSize()
        {
            var collection = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Assert.AreEqual(3.3595116950201258, collection.ExponentialMovingAverage(20));
        }

        [Test, Performance]
        public void TestPerformance()
        {
            var collection = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Measure.Method(() =>
            {
                collection.ExponentialMovingAverage(10);
            })
            .WarmupCount(10)
            .MeasurementCount(50)
            .IterationsPerMeasurement(5000)
            .GC()
            .Run();
        }

        [Test, Performance]
        public void TestPerformanceWithQueue()
        {
            var collection = new Queue<double>(new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Measure.Method(() =>
            {
                collection.ExponentialMovingAverage(10);
            })
            .WarmupCount(10)
            .MeasurementCount(50)
            .IterationsPerMeasurement(5000)
            .GC()
            .Run();
        }
    }
}