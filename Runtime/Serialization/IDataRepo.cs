namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;

    public interface IDataRepo<T> where T: IIDentifiable
    {
        IObservable<Unit> Save(IEnumerable<T> items);
        IObservable<IEnumerable<T>> Load(System.Func<T, bool> predicate);
        IObservable<IEnumerable<T>> LoadAll();
        IObservable<Unit> RemoveAll();
        IObservable<Unit> Remove(IEnumerable<T> objects);
    }

    public static class IDataRepoExtensions
    {
        public static IObservable<Unit> Remove<T>(this IDataRepo<T> repo, T obj) where T: IIDentifiable
        {
            return repo.Remove(new T[] { obj });
        }
        public static IObservable<Unit> Save<T>(this IDataRepo<T> repo, T obj) where T: IIDentifiable
        {
            return repo.Save(new T[] { obj });
        }
    }
}