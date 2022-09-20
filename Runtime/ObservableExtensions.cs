namespace HandcraftedGames.Common
{
    // using System;
    // using System.Reactive;
    // using System.Reactive.Linq;
    using UniRx;

    public static class ObservableExtension
    {
        public static System.IObservable<T> SwitchIfEmpty<T>(this System.IObservable<T> observable, System.IObservable<T> newIfPreviousEmpty)
        {
            return observable
                .ToArray()
                .SelectMany(i =>
                {
                    if(i.Length == 0)
                        return newIfPreviousEmpty;
                    return Observable.Return(i).SelectMany(u => u);
                });
        }

        public static System.IObservable<T> Debug<T>(this System.IObservable<T> observable, string name = "")
        {
            return observable
            .Do((value) => observable.Log("<color=magenta>" + name + "</color> -> OnNext: " + value))
            .DoOnCancel(() => observable.Log("<color=magenta>" + name + "</color> -> OnCancel"))
            .DoOnCompleted(() => observable.Log("<color=magenta>" + name + "</color> -> OnCompleted"))
            .DoOnError((error) =>
            {
                observable.LogError("<color=magenta>" + name + "</color> -> OnError: " + error);
                UnityEngine.Debug.LogException(error);
            })
            .DoOnSubscribe(() => observable.Log("<color=magenta>" + name + "</color> -> OnSubscribe"))
            .DoOnTerminate(() => observable.Log("<color=magenta>" + name + "</color> -> OnTerminate"))
            .Finally(() => observable.Log("<color=magenta>" + name + "</color> -> Finally"))
            ;
        }

        /// <summary>
        /// Will release values every passed interval
        /// </summary>
        /// <param name="releaseInterval"></param>
        public static System.IObservable<T> Debounce<T>(this System.IObservable<T> observable, System.TimeSpan releaseInterval)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            var lastTimestamp = 0.0;
            return observable.Where(i =>
            {
                var timestamp = (double)System.Diagnostics.Stopwatch.GetTimestamp() / (double)System.Diagnostics.Stopwatch.Frequency;
                if(timestamp >= lastTimestamp + releaseInterval.TotalSeconds)
                {
                    lastTimestamp = timestamp;
                    return true;
                }
                return false;
            });
        }

        public static System.IObservable<U> ToggleIf<T, U>(this System.IObservable<T> observable, System.Func<T, bool> predicate, System.IObservable<U> switchTo, System.IObservable<U> switchToIfNot = null)
        {
            return observable.Select(i =>
            {
                if(predicate(i))
                    return switchTo;
                else if(switchToIfNot != null)
                    return switchToIfNot;
                else
                    return UniRx.Observable.Empty<U>();
            })
            .Switch();
        }

        public static System.IObservable<U> ToggleIf<T, U>(this System.IObservable<T> observable, System.Func<T, bool> predicate, System.Func<T, System.IObservable<U>> switchTo, System.Func<T, System.IObservable<U>> switchToIfNot = null)
        {
            return observable.Select(i =>
            {
                if(predicate(i))
                    return switchTo(i);
                else if(switchToIfNot != null)
                    return switchToIfNot(i);
                else
                    return UniRx.Observable.Empty<U>();
            })
            .Switch();
        }

        public static System.IObservable<U> ToggleIfDeferred<T, U>(this System.IObservable<T> observable, System.Func<T, System.IObservable<bool>> predicate, System.Func<T, System.IObservable<U>> switchTo, System.Func<T, System.IObservable<U>> switchToIfNot = null)
        {
            return observable.Select(i =>
            {
                return predicate(i)
                .Select(shouldSwitch =>
                {
                    if(shouldSwitch)
                        return switchTo(i);
                    else if(switchToIfNot != null)
                        return switchToIfNot(i);
                    else
                        return UniRx.Observable.Empty<U>();
                })
                .Switch()
                ;
            })
            .Switch()
            ;
        }
    }
}