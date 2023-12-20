namespace HandcraftedGames.Common
{
    /// <summary>
    /// Interface for objects used by pooled object sources
    /// </summary>
    public interface IReusable
    {
        IPoolFactory<IReusable> FactorySource { get; set; }
        void CleanAfterUse();
        void PrepareBeforeUse();
    }

    public static class ReusableExtension
    {
        /// <summary>
        /// Releases this `IReusable` instance to the source pool
        /// </summary>
        public static void Release(this IReusable self)
        {
            self.FactorySource.Release(self);
        }
    }
}