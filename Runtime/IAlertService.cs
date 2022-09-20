namespace HandcraftedGames.Common
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;

    public interface IAlertService
    {
        void Show(string text, System.Action onOk);
    }

    public static class AlertServiceExtension
    {
        public static IObservable<Unit> Show(this IAlertService service, string text)
        {
            return Observable.Create<Unit>((observer) =>
            {
                service.Show(text, () => {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });
                return System.Reactive.Disposables.Disposable.Create(() => {});
            });
        }
    }
}
