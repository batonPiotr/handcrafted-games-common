namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using HandcraftedGames.Common.Rx;

    public interface IDataRepo<T> where T: IIdentifiable
    {
        IObservable<Never> Save(IEnumerable<T> items);
        IObservable<IEnumerable<T>> Load(System.Func<T, bool> predicate);
        IObservable<IEnumerable<T>> LoadAll();
        IObservable<Never> RemoveAll();
        IObservable<Never> Remove(IEnumerable<T> objects);
    }

    public static class IDataRepoExtensions
    {
        public static IObservable<Never> Remove<T>(this IDataRepo<T> repo, T obj) where T: IIdentifiable
        {
            return repo.Remove(new T[] { obj });
        }
        public static IObservable<Never> Save<T>(this IDataRepo<T> repo, T obj) where T: IIdentifiable
        {
            return repo.Save(new T[] { obj });
        }
    }
}