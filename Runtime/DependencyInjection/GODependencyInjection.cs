namespace HandcraftedGames.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [ScriptOrder(10000)]
    public class GODependencyInjection : MonoBehaviour
    {
        public interface IGOResolver
        {
            T Resolve<T>() where T: class;
            T[] ResolveAll<T>() where T: class;
        }
        private class GOResolver: IGOResolver
        {
            public GODependencyInjection source;
            public T Resolve<T>() where T: class
            {
                return source.Resolve<T>();
            }
            public T[] ResolveAll<T>() where T: class
            {
                return source.ResolveAll<T>();
            }
        }
        private List<System.Object> registeredClasses = new List<System.Object>();
        private List<GODependencyInjection> dependencies = new List<GODependencyInjection>();
        private List<System.Func<IGOResolver, bool>> dependencyRequests = new List<System.Func<IGOResolver, bool>>();

        private bool isInitialized = false;

        public bool IsInitialized => isInitialized;
        public event System.Action<GODependencyInjection> OnDidInitialize;

        private void Start()
        {
            if(!isInitialized)
                TryToInitialize();
        }

        public void AddDependencyRequest(System.Func<IGOResolver, bool> request)
        {
            if(isInitialized)
            {
                Debug.LogError("This dependency resolver [" + name + "] has been already resolved. Cannot add more dependency requests");
                return;
            }
            dependencyRequests.Add(request);
        }

        public void AddDependency(GameObject dependency)
        {
            if(isInitialized)
            {
                Debug.LogError("This dependency resolver [" + name + "] has been already resolved. Cannot add more dependencies");
                return;
            }
            if(dependency == gameObject)
                return;
            var di = dependency.GetGODependencyInjection();
            if(di.dependencies.Contains(this))
            {
                Debug.LogError("Requested dependency: " + dependency.name + " has in it dependency on this instance. This would create a deadlock");
                return;
            }
            dependencies.Add(di);
            di.OnDidInitialize += OnDependencyDidInitialize;
        }

        public void AddDependencies(IEnumerable<GameObject> dependenciesRange)
        {
            foreach(var d in dependenciesRange)
                AddDependency(d);
        }

        private void OnDependencyDidInitialize(GODependencyInjection dependency)
        {
            if(dependencies.FindAll(i => !i.isInitialized).Count == 0)
            {
                TryToInitialize();
            }
        }

        private void TryToInitialize()
        {
            if(isInitialized)
            {
                Debug.LogError("This dependency resolver [" + name + "] has been already resolved. Cannot initialize again.");
                return;
            }
            var resolver = new GOResolver();
            resolver.source = this;
            foreach(var request in dependencyRequests)
            {
                if(!request(resolver))
                {
                    Debug.LogError("Couldn't satisfy dependencies on " + name + ". Further resolve is not possible.");
                    return;
                }
            }
            isInitialized = true;
            OnDidInitialize?.Invoke(this);
            foreach(var dependency in dependencies)
                dependency.OnDidInitialize -= OnDependencyDidInitialize;
            dependencies.Clear();
        }

        private T Resolve<T>() where T: class
        {
            foreach(var obj in registeredClasses)
            {
                if(obj is T)
                    return obj as T;
            }
            foreach(var d in dependencies)
            {
                var retVal = d.Resolve<T>();
                if(retVal != null)
                    return retVal;
            }
            return null;
        }

        private T[] ResolveAll<T>() where T: class
        {
            var retVal = new List<T>();
            foreach(var obj in registeredClasses)
            {
                if(obj is T)
                    retVal.Add(obj as T);
            }
            foreach(var d in dependencies)
            {
                retVal.AddRange(d.ResolveAll<T>());
            }
            return retVal.ToArray();
        }

        public void Register<T>(T obj)
        {
            registeredClasses.Add(obj);
        }

        private void OnDestroy()
        {
            foreach(var registeredClass in registeredClasses)
                (registeredClass as IOnParentDestroyEventHandler)?.OnParentDestroy(gameObject);
            registeredClasses.Clear();
            foreach(var dependency in dependencies)
                dependency.OnDidInitialize -= OnDependencyDidInitialize;
        }

        public List<GODependencyInjection> DeepDependencies()
        {
            var retVal = new List<GODependencyInjection>(dependencies);
            foreach(var d in retVal)
            {
                retVal.AddRange(d.dependencies);
            }
            return retVal;
        }
    }

    public static class GameObjectDependencyInjectionExtension
    {
        public static GODependencyInjection GetGODependencyInjection(this GameObject gameObject)
        {
            return gameObject.GetComponent<GODependencyInjection>() ?? gameObject.AddComponent<GODependencyInjection>();
        }
    }
}