namespace HandcraftedGames.Common
{
    using System;
    using System.Reactive.Linq;

    public interface LocalizedException
    {
        string LocalizedMessage { get; }
    }

    public interface TitledException
    {
        string LocalizedTitle { get; }
    }

    public interface IGracefulErrorHandler
    {
        IObservable<T> HandleError<T>(System.Exception exception);
    }

    public static class GracefulErrorHandlerExtension
    {
        /// <summary>
        /// It handles the error - shows an alert with seleted error handler. It doesn't suppress the error, it will be rethrown after user dismisses the alert.
        /// </summary>
        public static IObservable<T> HandleError<T>(this IObservable<T> stream, IGracefulErrorHandler errorHandler)
        {
            return stream.Catch<T, System.Exception>((error) =>
            {
                return errorHandler
                    .HandleError<T>(error)
                    .Concat(Observable.Throw<T>(error));
            });
        }
    }
}