namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reactive;
    using System.Reactive.Linq;
    using UnityEngine;
    using System.Linq;
    using HandcraftedGames.Common.Async;
    using Newtonsoft;

    public class StreamDataRepo<T> : IDataRepo<T> where T : IIDentifiable
    {
        private IReusableStream reusableStream;
        private IEnumerable<T> cache = null;
        private IDispatchQueue taskQueue;
        private IDispatchQueue cacheDispatchQueue;
        public StreamDataRepo(IReusableStream reusableStream)
        {
            this.reusableStream = reusableStream;
            taskQueue = new ExtendedSerialTaskQueue();
            cacheDispatchQueue = new ExtendedSerialTaskQueue();
        }

        private IObservable<IEnumerable<T>> LoadFile()
        {
            return Observable.Create<IEnumerable<T>>(async (observer, token) => {
                await taskQueue.EnqueueAsync(async () => {
                    try
                    {
                        IEnumerable<T> retVal = new List<T>();
                        using(Stream stream = reusableStream.OpenStream())
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            var bytes = new byte[stream.Length];
                            await stream.ReadAsync(bytes, 0, bytes.Length);
                            var json = System.Text.Encoding.UTF8.GetString(bytes);
                            if(!(json == null || json == "" || json == "[]"))
                                retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json);
                        }
                        await cacheDispatchQueue.EnqueueAsync(() => { this.cache = retVal; });
                        observer.OnNext(retVal);
                        observer.OnCompleted();
                    }
                    catch(Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });
                return System.Reactive.Disposables.Disposable.Create(() => {});
            })
            ;
        }

        private IObservable<IEnumerable<T>> GetData()
        {
            return LoadCache().SwitchIfEmpty(LoadFile());
        }

        private IObservable<IEnumerable<T>> LoadCache()
        {
            return Observable.Create<IEnumerable<T>>(observer =>
            {
                cacheDispatchQueue.Enqueue(() =>
                {
                    if(this.cache != null)
                        observer.OnNext(this.cache);
                    observer.OnCompleted();
                });
                return System.Reactive.Disposables.Disposable.Create(() => {});
            });
        }

        private IObservable<Unit> SaveToFile(IEnumerable<T> data)
        {
            return Observable.FromAsync(async () => {
                await cacheDispatchQueue.EnqueueAsync(() => { this.cache = data; });
                await taskQueue.EnqueueAsync(async () => {
                    try
                    {
                        using(Stream stream = reusableStream.OpenStream())
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.SetLength(0);
                            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                            await stream.WriteAsync(bytes, 0, bytes.Length);
                            // await JsonSerializer.SerializeAsync(stream, data);
                            await stream.FlushAsync();
                        }
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                });
            });
        }

        public IObservable<IEnumerable<T>> Load(Func<T, bool> predicate)
        {
            return GetData().Select(i => i.Where(predicate));
        }

        public IObservable<IEnumerable<T>> LoadAll()
        {
            return GetData();
        }

        public IObservable<Unit> Remove(IEnumerable<T> objects)
        {
            return GetData()
            .Select(objects => objects.Except(objects, new IdentifiableEqualityComparer()))
            .SelectMany(filtered => SaveToFile(filtered));
        }

        public IObservable<Unit> RemoveAll()
        {
            return SaveToFile(new T[] {});
        }

        public IObservable<Unit> Save(IEnumerable<T> items)
        {
            return GetData()
            .Select(loaded => {
                var highestId = 0;
                if(loaded.Any())
                    highestId = loaded.OrderBy(i => i.Id).LastOrDefault()?.Id ?? 1;
                var highestIdObject = (object)highestId;
                var updatedItems = items.Select(i => {
                    if(i.Id == null)
                    {
                        var unwrappedHighestId = (int)highestIdObject;
                        unwrappedHighestId += 1;
                        i.Id = unwrappedHighestId;
                        highestIdObject = (object)unwrappedHighestId;
                    }
                    return i;
                });
                return loaded
                    .Except(updatedItems, new IdentifiableEqualityComparer())
                    .Concat(updatedItems);
            })
            .SelectMany(concatenated => SaveToFile(concatenated));
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