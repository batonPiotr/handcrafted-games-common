namespace HandcraftedGames.Common.Serialization
{
    using System.IO;
    public interface IReusableStream
    {
        Stream OpenStream();
    }
}