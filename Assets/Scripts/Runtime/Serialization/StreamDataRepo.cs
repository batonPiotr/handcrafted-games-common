namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UniRx;
    using UnityEngine;
    using System.Linq;
    using HandcraftedGames.Common.Async;
    using Newtonsoft;
    using HandcraftedGames.Common.Rx;
    using System.Threading.Tasks;
    using System.Threading;

    public class StreamDataRepo<T> : IDataRepo<T> where T : IIDentifiable
    {
        private IReusableStream reusableStream;
        private IEnumerable<T> cache = null;
        private IDispatchQueue taskQueue;
        private IDispatchQueue cacheDispatchQueue;
        private IDispatchQueue totalQueue;
        private IScheduler totalScheduler;
        public StreamDataRepo(IReusableStream reusableStream)
        {
            this.reusableStream = reusableStream;
            totalQueue = new ExtendedSerialTaskQueue();
            taskQueue = new ExtendedSerialTaskQueue();
            cacheDispatchQueue = new ExtendedSerialTaskQueue();
            totalScheduler = Scheduler.MainThread;
        }

        private async Task<IEnumerable<T>> LoadFileAsync()
        {
            return await taskQueue.EnqueueAsync<IEnumerable<T>>(async () =>
            {
                try
                {
                    IEnumerable<T> retVal = new List<T>();
                    using (Stream stream = reusableStream.OpenStream())
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var bytes = new byte[stream.Length];
                        await stream.ReadAsync(bytes, 0, bytes.Length);
                        var json = System.Text.Encoding.UTF8.GetString(bytes);
                        if (!(json == null || json == "" || json == "[]"))
                            retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json);
                    }
                    await cacheDispatchQueue.EnqueueAsync(() => { this.cache = retVal; });
                    return retVal;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        private async Task<IEnumerable<T>> GetDataAsync()
        {
            var cache = await LoadCacheAsync();
            if (cache == null)
                return await LoadFileAsync();
            return cache;
        }

        private async Task<IEnumerable<T>> LoadCacheAsync()
        {
            return await cacheDispatchQueue.EnqueueAsync(() =>
            {
                if (this.cache != null)
                    return cache;
                return null;
            });
        }

        private async Task SaveToFileAsync(IEnumerable<T> data)
        {
            await cacheDispatchQueue.EnqueueAsync(() => { this.cache = data; });
            await taskQueue.EnqueueAsync(async () =>
            {
                try
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                    using (Stream stream = reusableStream.OpenStream())
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.SetLength(0);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                        // await JsonSerializer.SerializeAsync(stream, data);
                        await stream.FlushAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    // observer.OnError(ex);
                }
            });
        }

        public IObservable<IEnumerable<T>> Load(Func<T, bool> predicate)
        {
            return LoadAll().Select(i => i.Where(predicate));
        }

        public IObservable<IEnumerable<T>> LoadAll()
        {
            return totalQueue.EnqueueAsync(async () => {
                return await GetDataAsync();
            })
            .ToObservable();
        }

        public IObservable<Never> Remove(IEnumerable<T> objects)
        {
            return totalQueue.EnqueueAsync(async () => {
                var objects = await GetDataAsync();
                await SaveToFileAsync(objects.Except(objects, new IdentifiableEqualityComparer()));
            })
            .ToObservable()
            .IgnoreElementsAsCompletable();
        }

        public IObservable<Never> RemoveAll()
        {
            return totalQueue.EnqueueAsync(async () => {
                await SaveToFileAsync(new T[] { });
            })
            .ToObservable()
            .IgnoreElementsAsCompletable();
        }

        public IObservable<Never> Save(IEnumerable<T> items)
        {
            return totalQueue.EnqueueAsync(async () =>
            {
                var loaded = await GetDataAsync();

                var highestId = 0;
                if (loaded.Any())
                    highestId = loaded.OrderBy(i => i.Id).LastOrDefault()?.Id ?? 1;
                var highestIdObject = (object)highestId;
                var updatedItems = items.Select(i =>
                {
                    if (i.Id == null)
                    {
                        var unwrappedHighestId = (int)highestIdObject;
                        unwrappedHighestId += 1;
                        i.Id = unwrappedHighestId;
                        highestIdObject = (object)unwrappedHighestId;
                    }
                    return i;
                });
                var concatenated = loaded
                    .Except(updatedItems, new IdentifiableEqualityComparer())
                    .Concat(updatedItems);
                await SaveToFileAsync(concatenated);
            })
            .ToObservable()
            .IgnoreElementsAsCompletable();
        }

        internal class IdentifiableEqualityComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(T obj)
            {
                return obj.Id ?? -1;
            }
        }
    }
}