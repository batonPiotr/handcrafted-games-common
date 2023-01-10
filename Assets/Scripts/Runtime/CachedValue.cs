namespace HandcraftedGames.Common
{
    public class CachedValue<T>
    {
        public T Value
        {
            get
            {
                if(cache == null)
                    cache = accessor();
                return cache;
            }
        }
        private T cache;
        private System.Func<T> accessor;
        public CachedValue(System.Func<T> accessor)
        {
            this.accessor = accessor;
        }
    }
}