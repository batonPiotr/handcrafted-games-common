namespace HandcraftedGames.Common.Async
{
    using System.Threading.Tasks;

    public interface IDispatchQueue
    {
        Task EnqueueAsync(System.Action action);
        Task EnqueueAsync(System.Func<Task> action);
        Task<T> EnqueueAsync<T>(System.Func<T> action);
        Task<T> EnqueueAsync<T>(System.Func<Task<T>> action);
    }

    public static class IDispatchQueueExtension
    {
        /// <summary>
        /// Enqueues an action. It does not await. Fire and forget.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="action"></param>
        public static void Enqueue(this IDispatchQueue queue, System.Action action)
        {
            queue.EnqueueAsync(action);
        }
        /// <summary>
        /// Enqueues an action. It does not await. Fire and forget.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="action"></param>
        public static void  Enqueue(this IDispatchQueue queue, System.Func<Task> action)
        {
            queue.EnqueueAsync(action);
        }
        /// <summary>
        /// Enqueues an action. It does not await. Fire and forget.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="action"></param>
        public static void  Enqueue<T>(this IDispatchQueue queue, System.Func<T> action)
        {
            queue.EnqueueAsync(action);
        }
        /// <summary>
        /// Enqueues an action. It does not await. Fire and forget.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="action"></param>
        public static void  Enqueue<T>(this IDispatchQueue queue, System.Func<Task<T>> action)
        {
            queue.EnqueueAsync(action);
        }
    }
}