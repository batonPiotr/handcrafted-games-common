namespace HandcraftedGames.Common.Rx
{
    using UniRx;
    using System;

    public class Never
    {
        private Never() {}
    }

    public static class ObservableNeverExtension
    {
        /// <summary>
        /// Subscribes to the next stream after this completable completes.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObservable<T> AndThen<T>(this IObservable<Never> source, System.Func<IObservable<T>> selector)
        {
            return source.Materialize()
                .Where(e => e.Kind == NotificationKind.OnCompleted)
                .SelectMany(notification => {
                    return selector();
                });
        }

        public static System.IObservable<T> AndThen<T>(this System.IObservable<Never> source, System.IObservable<T> continuation)
        {
            return source
                .Materialize()
                .Where(i => i.Kind == NotificationKind.OnCompleted)
                .SelectMany(i => { return continuation; })
                ;
        }

        /// <summary>
        /// Ignores all elements and casts this stream to a completable.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObservable<Never> IgnoreElementsAsCompletable<T>(this IObservable<T> source)
        {
            return source.Where(e => false).Select(i => i as Never);
        }

        /// <summary>
        /// Ignores all elements.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObservable<T> IgnoreElements<T>(this IObservable<T> source)
        {
            return source.Where(e => false);
        }
    }
}