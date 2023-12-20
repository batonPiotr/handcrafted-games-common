namespace HandcraftedGames.Common
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PoolFactory<T>: IPoolFactory<T>, IOnParentDestroyEventHandler where T: IReusable
    {
        int PoolSize;

        List<T> FreeInstances;
        List<T> AllInstances;

        private System.Func<T> Factory;

        public PoolFactory(System.Func<T> Factory, int PoolSize = 10)
        {
            FreeInstances = new List<T>(PoolSize);
            AllInstances = new List<T>(PoolSize);
            this.Factory = Factory;
            this.PoolSize = PoolSize;

            for(int i = 0; i < PoolSize; i++)
            {
                AllInstances.Add(Factory());
            }
            FreeInstances.AddRange(AllInstances);
        }

        private void Extend()
        {
            this.Log("Extending from " + PoolSize);
            for(int i = 0; i < PoolSize; i++)
            {
                var instance = Factory();
                AllInstances.Add(instance);
                FreeInstances.Add(instance);
            }
            PoolSize += PoolSize;
        }

        public T Create()
        {
            if(FreeInstances.Count == 0)
                Extend();
            return UseInstance();
        }

        public void Release(T instance)
        {
            instance.FactorySource = null;
            instance.CleanAfterUse();
            FreeInstances.Add(instance);
        }

        private T UseInstance()
        {
            var instance = FreeInstances[0];
            FreeInstances.RemoveAt(0);
            instance.FactorySource = this as IPoolFactory<IReusable>;
            instance.PrepareBeforeUse();
            return instance;
        }

        public void OnParentDestroy(GameObject gameObject)
        {
            FreeInstances.Clear();
            foreach(var instance in AllInstances)
                instance.FactorySource = null;
            AllInstances.Clear();
        }
    }
}