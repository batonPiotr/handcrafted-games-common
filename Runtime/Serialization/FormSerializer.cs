namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;

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

        public IObservable<Unit> Update(Action<T> writeAction)
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