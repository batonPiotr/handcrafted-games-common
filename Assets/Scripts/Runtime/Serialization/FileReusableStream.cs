namespace HandcraftedGames.Common.Serialization
{
    using System.IO;

    public class GenericReusableStream: IReusableStream
    {
        private System.Func<Stream> OpenStream;

        public GenericReusableStream(System.Func<Stream> opener)
        {
            OpenStream = opener;
        }

        Stream IReusableStream.OpenStream()
        {
            return OpenStream();
        }
    }
}