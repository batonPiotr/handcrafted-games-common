namespace HandcraftedGames.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UniRx;
    using UnityEngine.Events;

    public abstract class ExecutableAction : MonoBehaviour
    {
        private CompositeDisposable disposeBag = new CompositeDisposable();
        [SerializeField] private bool CancelTaskOnDisable = false;
        [SerializeField] private bool Verbose = false;
        [SerializeField] private string VerboseComment;

        [SerializeField] private float DelayBeforeExecution = 0.0f;

        public abstract System.IObservable<Unit> GenerateTask();

        [SerializeField] UnityEvent OnExecute;
        [SerializeField] UnityEvent OnComplete;
        [SerializeField] UnityEvent OnFailure;

        public void Execute()
        {
            var task =
            GenerateTask()
                .DoOnError((error) => OnFailure?.Invoke())
                .DoOnCompleted(() => OnComplete?.Invoke())
                .DoOnSubscribe(() => OnExecute?.Invoke());

            if(Verbose)
                task = task.Debug(VerboseComment);

            task.DelaySubscription(System.TimeSpan.FromSeconds(DelayBeforeExecution))
                .Subscribe()
                .AddTo(disposeBag);
        }

        private void OnDisable()
        {
            if(CancelTaskOnDisable)
                disposeBag.Clear();
        }

        private void OnDestroy()
        {
            disposeBag.Clear();
        }
    }
}