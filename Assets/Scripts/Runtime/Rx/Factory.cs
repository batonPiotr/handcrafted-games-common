namespace HandcraftedGames.Common.Rx
{
    using System;
    using UniRx;
    using System.Collections;
    using System.Collections.Generic;

    public class ObservableFactory
    {
        public static IObservable<T> FromEnumerable<T>(IEnumerable<T> enumerable)
        {
            return UniRx.Observable.Create<T>(observer => {
                foreach(var i in enumerable)
                    observer.OnNext(i);
                observer.OnCompleted();
                return UniRx.Disposable.Create(() => {});
            });
        }
    }
}