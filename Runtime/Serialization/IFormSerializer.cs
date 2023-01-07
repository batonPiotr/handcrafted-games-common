namespace HandcraftedGames.Common.Serialization
{
    using System;
    using UniRx;
    using HandcraftedGames.Common.Rx;

    public interface IFormSerializer<T> where T: class
    {
        IObservable<Never> Update(System.Action<T> writeAction);
        IObservable<T> Load();
    }

    public static class FormSerializerExtension
    {
        public static IObservable<V> LoadProperty<T, V>(this IFormSerializer<T> serializer, System.Func<T, V> selector) where T: class
        {
            return serializer.Load().Select(form => selector(form));
        }
    }
}