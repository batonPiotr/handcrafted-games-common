namespace HandcraftedGames.Common.Serialization
{
    using System;
    using HandcraftedGames.Common.Rx;
    using UniRx;

    public class FormSerializer<T> : IFormSerializer<T> where T : class, new()
    {
        private IKeyValueRepo<string> keyValueRepo;

        public FormSerializer(IReusableStream reusableStream)
        {
            keyValueRepo = new StreamKeyValueRepo<string>(reusableStream);
        }

        public IObservable<T> Load()
        {
            return keyValueRepo.Load<T>("data");
        }

        public IObservable<Never> Update(Action<T> writeAction)
        {
            return Load()
                .DefaultIfEmpty(new T())
                .Select(i =>
                {
                    writeAction(i);
                    return i;
                })
                .SelectMany(i => keyValueRepo.Save("data", i));
        }
    }
}