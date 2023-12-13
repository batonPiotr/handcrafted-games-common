namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Linq;
    using HandcraftedGames.Common;
    using HandcraftedGames.Common.Rx;
    using HandcraftedGames.Common.Dependencies.Newtonsoft;
    using UniRx;
    using HandcraftedGames.Common.Dependencies.Newtonsoft.Json;

    public class StreamKeyValueRepo<KeyType> : IKeyValueRepo<KeyType>
    {
        private struct KeyValuePair : IIdentifiable
        {
            public int? Id { get => (int)Key.CalculateQuickHash(); set {} }
            public string Key { get; set; }
            public string Value { get; set; }
        }
        private IDataRepo<KeyValuePair> dataRepo;
        public StreamKeyValueRepo(IReusableStream reusableStream)
        {
            dataRepo = new StreamDataRepo<KeyValuePair>(reusableStream);
        }

        public IObservable<T> Load<T>(KeyType key)
        {
            var keyString = key.ToString();
            return dataRepo.Load(i => i.Key == keyString)
                .Select(i => i.FirstOrDefault())
                .Select(i => i.Value)
                .Where(i => i != null)
                .Do((o) => this.Log("Next: " + o + ". Type is: " + o.GetType()))
                .Select(i => JsonConvert.DeserializeObject<T>(i))
                .Do((i) => {}, () => this.Log("On completed"));
                ;
        }

        public IObservable<Never> Remove(KeyType key)
        {
            var keyString = key.ToString();
            return dataRepo.Load(i => i.Key == keyString)
                .Select(i => i.FirstOrDefault())
                .SelectMany(i => dataRepo.Remove(i));
        }

        public IObservable<Never> Save<T>(KeyType key, T value)
        {
            return dataRepo.Save(new KeyValuePair { Key = key.ToString(), Value = JsonConvert.SerializeObject(value) });
        }
    }
}