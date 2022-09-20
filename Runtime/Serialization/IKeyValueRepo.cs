namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Reactive;

    public interface IKeyValueRepo<KeyType>
    {
        IObservable<Unit> Save<T>(KeyType key, T value);
        IObservable<Unit> Remove(KeyType key);
        IObservable<T> Load<T>(KeyType key);
    }
}