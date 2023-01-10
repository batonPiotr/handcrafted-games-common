namespace HandcraftedGames.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UniRx;

    public class AlertErrorHandler: IGracefulErrorHandler
    {
        [Inject] private IAlertService alertService;

        public IObservable<T> HandleError<T>(System.Exception error)
        {
            var localized = error as LocalizedException;
            var message = "";
            if(localized != null)
                message = localized.LocalizedMessage;
            else
                message = error.Message;
            return alertService.Show(message).Select(i => default(T));
        }
    }
}