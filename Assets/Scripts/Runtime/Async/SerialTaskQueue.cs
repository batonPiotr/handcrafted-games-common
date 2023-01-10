//--------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  File: SerialTaskQueue.cs
//
//--------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Represents a queue of tasks to be started and executed serially.
    /// </summary>
    public class SerialTaskQueue
    {
        /// <summary>The ordered queue of tasks to be executed. Also serves as a lock protecting all shared state.</summary>
        private readonly Queue<object> _tasks = new Queue<object>();
        /// <summary>
        /// The scheduler tasks are executed on
        /// </summary>
        private readonly TaskScheduler scheduler;

        public SerialTaskQueue(TaskScheduler taskScheduler = null)
        {
            scheduler = taskScheduler;
        }

        /// <summary>The task currently executing, or null if there is none.</summary>
        private Task _taskInFlight;

        /// <summary>Enqueues the task to be processed serially and in order.</summary>
        /// <param name="taskGenerator">The function that generates a non-started task.</param>
        public void Enqueue(Func<Task> taskGenerator)
        {
            EnqueueInternal(taskGenerator);
        }

        /// <summary>Enqueues the non-started task to be processed serially and in order.</summary>
        /// <param name="task">The task.</param>
        public Task Enqueue(Task task)
        {
            EnqueueInternal(task);
            return task;
        }

        /// <summary>Gets a Task that represents the completion of all previously queued tasks.</summary>
        public Task Completed()
        {
            return Enqueue(new Task(() => { }));
        }

        /// <summary>Enqueues the task to be processed serially and in order.</summary>
        /// <param name="taskOrFunction">The task or functino that generates a task.</param>
        /// <remarks>The task must not be started and must only be started by this instance.</remarks>
        private void EnqueueInternal(object taskOrFunction)
        {
            // Validate the task
            if (taskOrFunction == null) throw new ArgumentNullException(nameof(taskOrFunction));
            lock (_tasks)
            {
                // If there is currently no task in flight, we'll start this one
                if (_taskInFlight == null) StartTask_CallUnderLock(taskOrFunction);
                // Otherwise, just queue the task to be started later
                else _tasks.Enqueue(taskOrFunction);
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
                if (_tasks.Count > 0) StartTask_CallUnderLock(_tasks.Dequeue());
            }
        }

        /// <summary>Starts the provided task (or function that returns a task).</summary>
        /// <param name="nextItem">The next task or function that returns a task.</param>
        private void StartTask_CallUnderLock(object nextItem)
        {
            var next = nextItem as Task ?? ((Func<Task>) nextItem)();

            if (next.Status == TaskStatus.Created)
            {
                if(scheduler != null)
                    next.Start(scheduler);
                else
                    next.Start();
            }
            _taskInFlight = next;
            next.ContinueWith(OnTaskCompletion);

            //TODO: Make exception handling more general
            next.ContinueWith(task => {
                UnityEngine.Debug.LogError("SerialTaskQueue task failure: " + task.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}

//
// Example Usage:
// var serialTaskQueue = new SerialTaskQueue();
// for (int i = 1; i <= 10; i++)
// {
//     var threadId = i;
//     log.Info("Starting thread: " + threadId);
//     serialTaskQueue.Enqueue(Task.Factory.StartNew(() => LogMessages(threadId)));
// }
// serialTaskQueue.Completed().Wait();

// private static void LogMessages(int threadId)
// {
//     var log = new Logger("Thread_" + threadId); // Create logger from string.
//     log.Info("This is log message asfasf");
// }