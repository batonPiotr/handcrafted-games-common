namespace HandcraftedGames.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class TakeLastExtension
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> collection, int n)
        {
            var retVal = new List<T>(n);
            var count = collection.Count();
            var begin = count - n;
            if(begin < 0)
                begin = 0;
            for(int i = 0; i < n && i + begin < count; i++)
            {
                retVal.Add(collection.ElementAt(i + begin));
            }
            return retVal;
            // return collection.Reverse().Take(n).Reverse();
        }
    }
}