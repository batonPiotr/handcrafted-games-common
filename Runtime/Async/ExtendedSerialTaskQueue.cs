namespace HandcraftedGames.Common.Async
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HandcraftedGames.Common.Async;
    using HandcraftedGames.Common.Async.Runners;

    /// <summary>
    /// Represents a queue of tasks to be started and executed serially.
    /// </summary>
    public class ExtendedSerialTaskQueue: IDispatchQueue
    {
        /// <summary>The ordered queue of tasks to be executed. Also serves as a lock protecting all shared state.</summary>
        private readonly Queue<IRunner> _tasks = new Queue<IRunner>();
        /// <summary>
        /// The scheduler tasks are executed on
        /// </summary>
        private readonly TaskScheduler scheduler;

        public bool PrintExceptions = true;

        public ExtendedSerialTaskQueue(TaskScheduler taskScheduler = null)
        {
            scheduler = taskScheduler;
        }

        /// <summary>The task currently executing, or null if there is none.</summary>
        private Task _taskInFlight;

        // /// <summary>Enqueues the task to be processed serially and in order.</summary>
        // /// <param name="taskGenerator">The function that generates a non-started task.</param>
        // public void Enqueue(Func<Task> taskGenerator)
        // {
        //     EnqueueInternal(taskGenerator);
        // }

        // /// <summary>Enqueues the non-started task to be processed serially and in order.</summary>
        // /// <param name="task">The task.</param>
        // public Task Enqueue(Task task)
        // {
        //     EnqueueInternal(task);
        //     return task;
        // }

        // /// <summary>Gets a Task that represents the completion of all previously queued tasks.</summary>
        // public Task Completed()
        // {
        //     return Enqueue(new Task(() => { }));
        // }

        /// <summary>Enqueues the task to be processed serially and in order.</summary>
        /// <param name="taskOrFunction">The task or functino that generates a task.</param>
        /// <remarks>The task must not be started and must only be started by this instance.</remarks>
        private void EnqueueInternal(IRunner runner)
        {
            // Validate the task
            if (runner == null) throw new ArgumentNullException(nameof(runner));
            if(runner.Tasks == null)
                throw new System.ArgumentNullException();
            if(runner.Tasks.Count() == 0)
                throw new IndexOutOfRangeException();

            lock (_tasks)
            {
                // If there is currently no task in flight, we'll start this one
                if (_taskInFlight == null) StartTask_CallUnderLock(runner);
                // Otherwise, just queue the task to be started later
                else _tasks.Enqueue(runner);
            }
        }

        /// <summary>Called when a Task completes to potentially start the next in the queue.</summary>
        /// <param name="ignored">The task that completed.</param>
        private void OnTaskCompletion(Task ignored)
        {
            lock (_tasks)
            {
                // The task completed, so nothing is currently in flight.
                // If there are any tasks in the queue, start the next one.
                _taskInFlight = null;
                if (_tasks.Count > 0)
                    StartTask_CallUnderLock(_tasks.Dequeue());
            }
        }

        /// <summary>Starts the provided task (or function that returns a task).</summary>
        /// <param name="nextItem">The next task or function that returns a task.</param>
        private void StartTask_CallUnderLock(IRunner nextItem)
        {
            var next = nextItem.Tasks.First();
            _taskInFlight = nextItem.Tasks.Last();
            _taskInFlight.ContinueWith(OnTaskCompletion);

            if (next.Status == TaskStatus.Created)
            {
                if(scheduler != null)
                    next.Start(scheduler);
                else
                    next.Start();
            }
            //TODO: Make exception handling more general
            if(PrintExceptions)
            foreach(var t in nextItem.Tasks)
                t.ContinueWith(task => {
                    UnityEngine.Debug.LogError("ExtendedSerialTaskQueue task failure: " + task.Exception);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
        private async Task Enqueue(IRunner runner)
        {
            EnqueueInternal(runner);
            if(System.Threading.Tasks.TaskScheduler.Current == this.scheduler && this.scheduler != null)
                UnityEngine.Debug.LogWarning("Creating a task on the same scheduler as Current. It may produce Deadlocks if this task is nested awaiting.");
            var summedTask = Task.WhenAll(runner.Tasks);
            await summedTask;
        }

        public async Task EnqueueAsync(Action action)
        {
            await Enqueue(new SimpleRunner(action));
        }

        public async Task EnqueueAsync(Task task)
        {
            await Enqueue(new TaskedRunner(task));
        }

        public async Task EnqueueAsync(Func<Task> action)
        {
            await Enqueue(new DeferredRunner(action));
        }

        public async Task<T> EnqueueAsync<T>(Func<T> action)
        {
            var runner = new SimpleRunner<T>(action);
            await Enqueue(runner);
            return (T)runner.Result;
        }

        public async Task<T> EnqueueAsync<T>(Func<Task<T>> action)
        {
            var runner = new DeferredRunner<T>(action);
            await Enqueue(runner);
            return (T)runner.Result;
        }

        public async Task<T> EnqueueAsync<T>(Task<T> task)
        {
            var runner = new TaskedRunner<T>(task);
            await Enqueue(runner);
            return (T)runner.Result;
        }
    }
}

// //
// // Example Usage:
// // var serialTaskQueue = new SerialTaskQueue();
// // for (int i = 1; i <= 10; i++)
// // {
// //     var threadId = i;
// //     log.Info("Starting thread: " + threadId);
// //     serialTaskQueue.Enqueue(Task.Factory.StartNew(() => LogMessages(threadId)));
// // }
// // serialTaskQueue.Completed().Wait();

// // private static void LogMessages(int threadId)
// // {
// //     var log = new Logger("Thread_" + threadId); // Create logger from string.
// //     log.Info("This is log message asfasf");
// // }