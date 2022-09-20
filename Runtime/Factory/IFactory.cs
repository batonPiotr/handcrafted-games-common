namespace HandcraftedGames.Common
{
    public interface IFactory<T>
    {
        T Create();
    }
}