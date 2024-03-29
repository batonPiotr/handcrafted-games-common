using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HandcraftedGames.Common.Async;
using HandcraftedGames.Common.Async.Runners;
// using Dispatch; //borland implementation of dispatch queue - have a look
using UnityEngine;

namespace HandcraftedGames.Common
{
    public class MainThreadUnityDispatcher : MonoBehaviour, IDispatchQueue
    {
        public static MainThreadUnityDispatcher Main;
        public static System.Threading.Tasks.TaskScheduler MainScheduler;
        public static int MainThreadId;

        private Queue<IRunner> queue = new Queue<IRunner>();
        private IRunner current = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            Main = Create("MainThreadUnityDispatcher");
        }

        public static MainThreadUnityDispatcher Create(string name, bool dontDestroyOnLoad = true)
        {
            var go = new GameObject(name);
            var retVal = go.AddComponent<MainThreadUnityDispatcher>();
            MainScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
            if (dontDestroyOnLoad) DontDestroyOnLoad(go);
            return retVal;
        }

        private async Task Enqueue(IRunner runner)
        {
            if (runner.Tasks == null)
                throw new System.ArgumentNullException();
            if (runner.Tasks.Count() == 0)
                throw new IndexOutOfRangeException();
            if (MainThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                if (runner.Tasks.First().Status == TaskStatus.Created)
                    runner.Tasks.First().Start(MainScheduler);
                try { runner.Tasks.Last().Wait(); }
                catch (System.Exception e) { Handle(e); }
                return;
            }
            lock (queue)
            {
                queue.Enqueue(runner);
            }
            await runner.Tasks.Last();
        }

        public async Task EnqueueAsync(System.Action action)
        {
            var runner = new SimpleRunner(action);
            await Enqueue(runner);
        }

        public async Task EnqueueAsync(Task task)
        {
            var runner = new TaskedRunner(task);
            await Enqueue(runner);
        }

        public async Task EnqueueAsync(System.Func<Task> action)
        {
            var runner = new DeferredRunner(action);
            await Enqueue(runner);
        }

        public async Task<T> EnqueueAsync<T>(System.Func<T> action)
        {
            var runner = new SimpleRunner<T>(action);
            await Enqueue(runner);
            return (T)runner.Result;
        }

        public async Task<T> EnqueueAsync<T>(Task<T> task)
        {
            var runner = new TaskedRunner<T>(task);
            await Enqueue(runner);
            return (T)runner.Result;
        }

        public async Task<T> EnqueueAsync<T>(System.Func<Task<T>> action)
        {
            var runner = new DeferredRunner<T>(action);
            await Enqueue(runner);
            return (T)runner.Result;
        }
        public async Task Completed()
        {
            await EnqueueAsync(() => { });
        }

        private async void Update()
        {
            if (current == null)
            {
                lock (queue)
                {
                    if (queue.Count == 0)
                    {
                        return;
                    }
                    current = queue.Dequeue();
                }
                if (current != null)
                {
                    // using(var A = AsyncBridge.AsyncHelper.Wait)
                    // {

                    // }
                    // var lol = current.Task.ConfigureAwait(true);
                    if (current.Tasks.First().Status == TaskStatus.Created)
                        // current.Task.Start(scheduler);
                        current.Tasks.First().RunSynchronously(MainScheduler);
                    try { await current.Tasks.Last(); }
                    catch (System.Exception e) { Handle(e); }
                    current = null;
                }
            }
        }

        private void Handle(System.Exception exception)
        {
            // Debug.Log("WHAT HAPPENED current: " + current);
            // Debug.Log("WHAT HAPPENED queue.count: " + queue.Count);
            Debug.LogError(exception);
            // Debug.Log("WHAT HAPPENED 1 current: " + current);
            // Debug.Log("WHAT HAPPENED 1 queue.count: " + queue.Count);
        }
    }
}