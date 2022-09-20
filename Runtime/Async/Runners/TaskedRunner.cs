using System.Threading.Tasks;

namespace HandcraftedGames.Common.Async.Runners
{
    public class TaskedRunner : BaseRunner
    {
        public TaskedRunner(Task task)
        {
            this.tasks.Add(task);
        }
    }

    public class TaskedRunner<T>: BaseRunner
    {
        public TaskedRunner(Task<T> task1)
        {
            // this.task = new Task(() => {
            //     if(task1.Status == TaskStatus.Created)
            //         task1.Start(MainThreadUnityDispatcher.scheduler);
            //     task1.Wait();
            //     result = task1.Result;
            // });

            type = typeof(T);
            tasks.Add(task1);
            tasks.Add(task1.ContinueWith(antecedent =>
            {
                if(antecedent.Status == TaskStatus.Faulted)
                {
                    UnityEngine.Debug.LogError("TaskedRunner fault: " + antecedent.Exception.GetBaseException());
                }
                else
                {
                    this.result = antecedent.Result;
                }
            }));
        }
    }
}