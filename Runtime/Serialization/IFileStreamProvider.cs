namespace HandcraftedGames.Common.Serialization
{
    public interface IFileStreamProvider
    {
        IReusableStream CreateStreamWithFilenameSalt(string filenameSalt);
    }
}