namespace HandcraftedGames.Common
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class PoolFactory
    {
        public abstract void ReleaseInstance(IReusable instance);
    }

    public class PoolFactory<T>: PoolFactory, IFactory<T>, IOnParentDestroyEventHandler where T: IReusable
    {
        int PoolSize;

        // List<T> InstancesInUse;
        List<T> FreeInstances;
        List<T> AllInstances;

        private System.Func<T> Factory;

        public PoolFactory(System.Func<T> Factory, int PoolSize = 10)
        {
            // InstancesInUse = new List<T>(PoolSize);
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
            // if(InstancesInUse.Count == 0)
            //     return UseInstance(AllInstances[0]);
            if(FreeInstances.Count == 0)
                Extend();
            return UseInstance();
        }

        public override void ReleaseInstance(IReusable instance)
        {
            if(instance is T typedInstance)
            {
                // if(!InstancesInUse.Contains(typedInstance))
                //     throw new System.CannotUnloadAppDomainException("Cannot release instance since it's not being used");
                instance.FactorySource = null;
                instance.CleanAfterUse();
                // InstancesInUse.Remove(typedInstance);
                FreeInstances.Add(typedInstance);
            }
        }

        private T UseInstance()
        {
            var instance = FreeInstances[0];
            FreeInstances.RemoveAt(0);
            instance.FactorySource = this;
            instance.PrepareBeforeUse();
            // InstancesInUse.Add(instance);
            return instance;
        }

        public void OnParentDestroy(GameObject gameObject)
        {
            // InstancesInUse.Clear();
            FreeInstances.Clear();
            foreach(var instance in AllInstances)
                instance.FactorySource = null;
            AllInstances.Clear();
        }
    }
}