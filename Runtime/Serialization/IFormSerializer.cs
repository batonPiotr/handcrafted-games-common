namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;

    public interface IFormSerializer<T> where T: class
    {
        IObservable<Unit> Update(System.Action<T> writeAction);
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