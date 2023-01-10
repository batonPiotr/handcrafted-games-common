namespace HandcraftedGames.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class MovingAveragesExtension
    {

        /// <summary>
        /// Computes SMA for the data series and returns only last value
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="period"></param>
        /// <returns>Last value of SMA data series</returns>
        public static double MovingAverage(this IEnumerable<double> collection, int period = -1)
        {
            if(period < 0)
                period = collection.Count();
            if(period < 1)
                throw new System.Exception("Period should be equal or larger than 1");

            var sum = 0.0;
            var actualPeriod = 0;
            for(int i = collection.Count() - 1; i >= 0 && actualPeriod < period; i--)
            {
                sum += collection.ElementAt(i);
                actualPeriod++;
            }
            return sum / actualPeriod;
        }

        /// <summary>
        /// Computes EMA for the data series and returns only last value
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="period"></param>
        /// <returns>Last value of EMA data series</returns>
        public static double ExponentialMovingAverage(this IEnumerable<double> collection, int period = -1)
        {
            if(period < 0)
                period = collection.Count();
            if(period < 1)
                throw new System.Exception("Period should be equal or larger than 1");

            var alpha = 2.0 / ((double)period + 1);

            var count = collection.Count();
            var lastEMA = collection.ElementAt(0);
            for(int i = 0; i < count; i++)
            {
                var value = collection.ElementAt(i);
                lastEMA = (value * alpha) + ((1.0 - alpha) * lastEMA);
            }
            return lastEMA;
        }

        /// <summary>
        /// Shifts all values by one place to the beginning and append the new value. First value will be discarded.
        /// </summary>
        public static void ShiftAndAppend<T>(this T[] collection, T element)
        {
            var count = collection.Count();
            for(var i = 0; i < count - 1; i++)
            {
                collection[i] = collection[i+1];
            }
            collection[count - 1] = element;
        }
    }
}