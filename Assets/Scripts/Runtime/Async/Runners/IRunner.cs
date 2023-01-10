using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandcraftedGames.Common.Async.Runners
{
    public interface IRunner
    {
        object Result { get; }
        Type ResultType { get; }
        IEnumerable<Task> Tasks { get; }
    }
}