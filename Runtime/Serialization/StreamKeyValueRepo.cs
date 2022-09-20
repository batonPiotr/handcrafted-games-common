namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Linq;
    using HandcraftedGames.Common;
    using Newtonsoft.Json;

    public class StreamKeyValueRepo<KeyType> : IKeyValueRepo<KeyType>
    {
        private struct KeyValuePair : IIDentifiable
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

        public IObservable<Unit> Remove(KeyType key)
        {
            var keyString = key.ToString();
            return dataRepo.Load(i => i.Key == keyString)
                .Select(i => i.FirstOrDefault())
                .SelectMany(i => dataRepo.Remove(i));
        }

        public IObservable<Unit> Save<T>(KeyType key, T value)
        {
            return dataRepo.Save(new KeyValuePair { Key = key.ToString(), Value = JsonConvert.SerializeObject(value) });
        }
    }
}