namespace HandcraftedGames.Common.Serialization
{
    using System;
    using HandcraftedGames.Common.Rx;

    public interface IKeyValueRepo<KeyType>
    {
        IObservable<Never> Save<T>(KeyType key, T value);
        IObservable<Never> Remove(KeyType key);
        IObservable<T> Load<T>(KeyType key);
    }
}