namespace HandcraftedGames.Common
{
    public interface IPoolFactory<T>: IFactory<T>
    {
        /// <summary>
        /// Resets the instance and brings to the default state
        /// </summary>
        /// <param name="instance"></param>
        void Release(T instance);
    }
}