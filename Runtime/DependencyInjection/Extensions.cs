namespace HandcraftedGames.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class GameObjectDependencyInjectionExtension
    {
        public static T GODIRegister<T>(this GameObject gameObject, T obj)
        {
            gameObject.GetGODependencyInjection().Register(obj);
            return obj;
        }
        public static T GODIResolve<T>(this GameObject gameObject, T obj)
        {
            gameObject.GetGODependencyInjection().ResolveDependencies(obj);
            return obj;
        }
        public static GODependencyInjection GetGODependencyInjection(this GameObject gameObject)
        {
            var godi = gameObject.GetComponent<GODependencyInjection>();
            return godi == null ? gameObject.AddComponent<GODependencyInjection>() : godi;
        }
        public static System.IObservable<T> ResolveDependencyAsObservable<T>(this GameObject gameObject, bool Optional = false) where T: class
        {
            return UniRx.Observable.Create<T>(observer =>
            {
                gameObject.GetGODependencyInjection().AddDependencyRequest(resolver =>
                {
                    try
                    {
                        var resolved = resolver.Resolve<T>(Optional: Optional);
                        observer.OnNext(resolved);
                        observer.OnCompleted();
                    }
                    catch (System.Exception exception)
                    {
                        observer.OnError(exception);
                    }
                });
                return UniRx.Disposable.Create(() => {});
            });
        }
        public static GODependencyInjection GetGlobalDependencyInjection(this GameObject gameObject)
        {
            var go = GameObject.FindGameObjectWithTag(GODependencyInjection.GlobalDITag);
            if(go == null)
            {
                go = new GameObject("__ Global DI Container");
                GameObject.DontDestroyOnLoad(go);
                go.tag = GODependencyInjection.GlobalDITag;
                return go.AddComponent<GODependencyInjection>();
            }
            return go.GetComponent<GODependencyInjection>();
        }
        public static IEnumerable<T> GetComponentsInAllParents<T>(this GameObject gameObject)
        {
            var parent = gameObject.transform.parent;
            var retVal = new List<T>();
            while(parent != null)
            {
                retVal.AddRange(parent.GetComponents<T>());
                parent = parent.transform.parent;
            }
            return retVal;
        }
    }

    public static class SceneDI
    {
        public static GODependencyInjection GetGODependencyInjection(this Scene scene, bool CreateWhenNotPresent = false)
        {
            var gos = scene.GetRootGameObjects();
            foreach(var go in gos)
            {
                if(go.CompareTag(GODependencyInjection.SceneDITag))
                {
                    var di = go.GetComponent<GODependencyInjection>();
                    if(di != null)
                        return di;
                }
            }
            if(CreateWhenNotPresent)
            {
                var go = new GameObject("__ Scene DI Container");
                go.tag = GODependencyInjection.SceneDITag;
                SceneManager.MoveGameObjectToScene(go, scene);
                return go.AddComponent<GODependencyInjection>();
            }
            return null;
        }
    }
}