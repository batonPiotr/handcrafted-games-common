namespace HandcraftedGames.Common
{
    using System;
    using HandcraftedGames.Common.Rx;
    using UniRx;

    public interface IAlertService
    {
        void Show(string text, System.Action onOk);
    }

    public static class AlertServiceExtension
    {
        public static IObservable<Never> Show(this IAlertService service, string text)
        {
            return Observable.Create<Never>((observer) =>
            {
                service.Show(text, () => {
                    observer.OnCompleted();
                });
                return UniRx.Disposable.Create(() => {});
            });
        }
    }
}
