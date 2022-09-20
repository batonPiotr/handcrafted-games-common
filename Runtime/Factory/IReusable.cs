namespace HandcraftedGames.Common
{
    /// <summary>
    /// Interface for objects used by pooled object sources
    /// </summary>
    public interface IReusable
    {
        System.Object FactorySource { get; set; }
        void CleanAfterUse();
        void PrepareBeforeUse();
    }

    public static class ReusableExtension
    {
        public static void PutBack(this IReusable self)
        {
            var factory = self.FactorySource as PoolFactory;
            factory.ReleaseInstance(self);
        }
    }
}