namespace HandcraftedGames.Common
{
    using System;
    public static class QuickHash
    {
        public static UInt64 CalculateQuickHash(this string read)
        {
            UInt64 hashedValue = 3074457345618258791ul;
            // if(read != null)
            for (int i = 0; i < read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }
    }
}