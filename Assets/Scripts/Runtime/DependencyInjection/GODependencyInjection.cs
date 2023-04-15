namespace HandcraftedGames.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using System.Reflection;

    public class UnableToResolveException: System.Exception
    {
        public UnableToResolveException(string Requester, System.Type invalidType): base("[" + Requester + "] Couldn't resolve [" + invalidType + "].")
        {

        }
    }

    [System.Flags]
    public enum ResolveSource
    {
        Self                    = 1 << 0,
        ExplicitDependencies    = 1 << 1,
        Children                = 1 << 2,
        Parent                  = 1 << 3,
        Parents                 = 1 << 4,
        Scene                   = 1 << 5,
        Global                  = 1 << 6,
        Default                 = ResolveSource.Self | ResolveSource.Scene | ResolveSource.ExplicitDependencies | ResolveSource.Global
    }

    [ScriptOrder(10000)]
    public class GODependencyInjection : MonoBehaviour
    {
        public const string SceneDITag = "SceneDIContainer";
        public const string GlobalDITag = "GlobalDIContainer";

        public bool Verbose = false;

        public interface IGOResolver
        {
            T Resolve<T>(ResolveSource strategy = ResolveSource.Default, bool Optional = false) where T: class;
            IEnumerable<T> ResolveAll<T>(ResolveSource strategy = ResolveSource.Default) where T: class;
        }
        private class GOResolver: IGOResolver
        {
            public GODependencyInjection source;
            public T Resolve<T>(ResolveSource strategy = ResolveSource.Default, bool Optional = false) where T: class
            {
                return source.Resolve<T>(source, strategy, Optional);
            }
            public IEnumerable<T> ResolveAll<T>(ResolveSource strategy = ResolveSource.Default) where T: class
            {
                return source.ResolveAll<T>(strategy);
            }
        }

        private List<System.Object> registeredClasses = new List<System.Object>();

        [ReadOnly, SerializeReference]
        private List<string> registeredClassNames = new List<string>();
        [ReadOnly, SerializeField]
        private List<GODependencyInjection> dependencies = new List<GODependencyInjection>();
        private GODependencyInjection sceneDI;
        private GODependencyInjection globalDI;
        private List<System.Action<IGOResolver>> dependencyRequests = new List<System.Action<IGOResolver>>();

        public bool IsInitialized { get; private set;}
        public bool IsInitializationFailed { get; private set;}

        public event System.Action<GODependencyInjection> OnDidInitialize;

        private void Awake()
        {
            IsInitialized = false;
            IsInitializationFailed = false;
            if(Verbose) this.Log("Awake");
            if(!gameObject.CompareTag(GlobalDITag))
            {
                globalDI = gameObject.GetGlobalDependencyInjection();

                if(!gameObject.CompareTag(SceneDITag))
                {
                    sceneDI = gameObject.scene.GetGODependencyInjection(true);
                }
            }
        }

        private void Start()
        {
            if(Verbose) this.Log("Start");
            if(!IsInitialized)
                TryToInitialize();
        }

        public void AddDependencyRequest(System.Action<IGOResolver> request)
        {
            if(IsInitializationFailed)
                throw new System.Exception(gameObject.name + " Has failed to initialize because of unresolved dependencies. Request couldn't be satisfied. If some dependency doesn't have to be resolved, consider using `Optional` flag.");
            else if(IsInitialized)
            {
                var resolver = new GOResolver();
                resolver.source = this;
                try { request(resolver); }
                catch(System.Exception exception) { Debug.LogException(exception); return; }
                // this.LogError("This dependency resolver has already been resolved. Cannot add more dependency requests");
                // return;
            }
            else
                dependencyRequests.Add(request);
        }

        public void AddDependency(GameObject dependency)
        {
            if(IsInitialized)
            {
                this.LogError("This dependency resolver has already already resolved. Cannot add more dependencies");
                return;
            }
            if(dependency == gameObject)
                return;
            var di = dependency.GetGODependencyInjection();
            if(di.dependencies.Contains(this))
            {
                this.LogError("Requested dependency: " + dependency.name + " has in it dependency on this instance. This would create a deadlock");
                return;
            }
            if(dependencies.Contains(di))
            {
                // this.LogWarning("This dependency: [" + dependency + "] is already added.");
                return;
            }
            if(Verbose) this.Log("Added dependency: " + dependency.name);
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
            if(dependencies.FindAll(i => !i.IsInitialized).Count == 0)
            {
                TryToInitialize();
            }
        }

        private void TryToInitialize()
        {
            if(IsInitialized)
            {
                // Debug.LogError("This dependency resolver [" + name + "] has been already resolved. Cannot initialize again.");
                return;
            }
            var resolver = new GOResolver();
            resolver.source = this;
            foreach(var request in dependencyRequests)
            {
                try { request(resolver); }
                catch(System.Exception exception)
                {
                    Debug.LogException(exception);
                    IsInitializationFailed = true;
                    return;
                }
            }
            IsInitialized = true;
            foreach(var dependency in dependencies)
                dependency.OnDidInitialize -= OnDependencyDidInitialize;
            // dependencies.Clear();
            if(Verbose) this.Log("Did initialize!");
            OnDidInitialize?.Invoke(this);
        }

        private T Resolve<T>(Object requester, ResolveSource strategy = ResolveSource.Default, bool Optional = false) where T: class
        {
            if(this is T)
                return this as T;

            if(strategy.HasFlag(ResolveSource.Self))
                foreach(var obj in registeredClasses)
                {
                    if(obj is T)
                        return obj as T;
                }

            var selectedDependencies = new List<GODependencyInjection>();
            if(strategy.HasFlag(ResolveSource.ExplicitDependencies))
                selectedDependencies.AddRange(dependencies);
            if(strategy.HasFlag(ResolveSource.Global) && globalDI != null)
                selectedDependencies.Add(globalDI);
            if(strategy.HasFlag(ResolveSource.Scene) && sceneDI != null)
                selectedDependencies.Add(sceneDI);
            if(strategy.HasFlag(ResolveSource.Children))
                selectedDependencies.AddRange(gameObject.GetComponentsInChildren<GODependencyInjection>());
            if(strategy.HasFlag(ResolveSource.Parent))
                selectedDependencies.AddRange(gameObject.GetComponentsInParent<GODependencyInjection>());
            else if(strategy.HasFlag(ResolveSource.Parents))
                selectedDependencies.AddRange(gameObject.GetComponentsInAllParents<GODependencyInjection>());

            foreach(var d in selectedDependencies.Reverse<GODependencyInjection>())
            {
                try
                {
                    var retVal = d.Resolve<T>(requester, ResolveSource.Self);
                    if(retVal != null)
                        return retVal;
                }
                catch(UnableToResolveException exception)
                {
                    // Log this only in very verbose log mode
                    // this.LogWarning("[" + d.name + "] " + exception.Message);
                }
            }
            if(!Optional)
                throw new UnableToResolveException(requester.name, typeof(T));
            return null;
        }

        private IEnumerable<T> ResolveAll<T>(ResolveSource strategy = ResolveSource.Default) where T: class
        {
            var retVal = new List<T>();

            if(strategy.HasFlag(ResolveSource.Self))
                foreach(var obj in registeredClasses)
                {
                    if(obj is T)
                        retVal.Add((T)obj);
                }

            var selectedDependencies = new List<GODependencyInjection>();
            if(strategy.HasFlag(ResolveSource.ExplicitDependencies))
                selectedDependencies.AddRange(dependencies);
            if(strategy.HasFlag(ResolveSource.Global) && globalDI != null)
                selectedDependencies.Add(globalDI);
            if(strategy.HasFlag(ResolveSource.Scene) && sceneDI != null)
                selectedDependencies.Add(sceneDI);
            if(strategy.HasFlag(ResolveSource.Children))
                selectedDependencies.AddRange(gameObject.GetComponentsInChildren<GODependencyInjection>());
            if(strategy.HasFlag(ResolveSource.Parent))
                selectedDependencies.AddRange(gameObject.GetComponentsInParent<GODependencyInjection>());
            else if(strategy.HasFlag(ResolveSource.Parents))
                selectedDependencies.AddRange(gameObject.GetComponentsInAllParents<GODependencyInjection>());

            foreach(var d in selectedDependencies)
            {
                retVal.AddRange(d.ResolveAll<T>(ResolveSource.Self));
            }
            return retVal.AsEnumerable();
        }

        public void Register<T>(T obj)
        {
            registeredClasses.Add(obj);
            registeredClassNames.Add(typeof(T).ToString());
            ResolveDependencies(obj);
            if(Verbose) this.Log("Registered: " + obj);
        }

        public void ResolveDependencies<T>(T obj)
        {
            ResolveInjectAttributes(obj);
            ResolveInjectAllAttributes(obj);
            ResolveOnInjectMethodAttribute(obj);
            if(Verbose) this.Log("Resolved dependencies for: " + obj);
        }

        private void OnDestroy()
        {
            foreach(var registeredClass in registeredClasses)
                (registeredClass as IOnParentDestroyEventHandler)?.OnParentDestroy(gameObject);
            registeredClasses.Clear();
            foreach(var dependency in dependencies)
                dependency.OnDidInitialize -= OnDependencyDidInitialize;
            dependencies.Clear();
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

        private void ResolveInjectAttributes<T>(T obj)
        {
            System.Type currentType = typeof(T);
            while(currentType != null)
            {
                var fields = currentType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(i => InjectAttribute.IsDefined(i, typeof(InjectAttribute)));

                var method = typeof(IGOResolver).GetMethod("Resolve");

                foreach(var p in fields)
                {
                    var attribute = p.GetCustomAttribute<InjectAttribute>();
                    var genericMethod = method.MakeGenericMethod(p.FieldType);
                    AddDependencyRequest(resolver => {
                        p.SetValue(obj, genericMethod.Invoke(resolver, new object[] { attribute.strategy, attribute.Optional }));
                    });
                }
                currentType = currentType.BaseType;
            }
        }

        private void ResolveInjectAllAttributes<T>(T obj)
        {
            System.Type currentType = typeof(T);
            while(currentType != null)
            {
                var fields = currentType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(i => InjectAttribute.IsDefined(i, typeof(InjectAllAttribute)));

                var method = typeof(IGOResolver).GetMethod("ResolveAll");

                foreach(var p in fields)
                {
                    var attribute = p.GetCustomAttribute<InjectAllAttribute>();
                    var genericMethod = method.MakeGenericMethod(p.FieldType.GetGenericArguments()[0]);
                    AddDependencyRequest(resolver => {
                        p.SetValue(obj, genericMethod.Invoke(resolver, new object[] { attribute.strategy }));
                    });
                }
                currentType = currentType.BaseType;
            }
        }

        private void ResolveOnInjectMethodAttribute<T>(T obj)
        {
            var methods = typeof(T).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(i => OnDependenciesDidResolve.IsDefined(i, typeof(OnDependenciesDidResolve)));

            foreach(var m in methods)
            {
                System.Action<GODependencyInjection> onDidInitializeCallback = null;
                onDidInitializeCallback = (di) => {
                    m.Invoke(obj, null);
                    this.OnDidInitialize -= onDidInitializeCallback;
                };
                if(IsInitialized)
                    onDidInitializeCallback(this);
                else
                    this.OnDidInitialize += onDidInitializeCallback;
            }
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property | System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class InjectAttribute : System.Attribute
    {
        public InjectAttribute(ResolveSource strategy = ResolveSource.Default)
        {
            this.strategy = strategy;
        }

        public ResolveSource strategy = ResolveSource.Default;
        public bool Optional = false;
    }

    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property | System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class InjectAllAttribute : System.Attribute
    {
        public InjectAllAttribute(ResolveSource strategy = ResolveSource.Default)
        {
            this.strategy = strategy;
        }

        public ResolveSource strategy = ResolveSource.Default;
    }
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class OnDependenciesDidResolve : System.Attribute
    {
    }

    public static class GODependencyInjectionExtension
    {
        /// <summary>
        /// Calls given action after dependencies are resolved or now if it's already done.
        /// </summary>
        public static void NowOrAfterResolve(this GODependencyInjection source, System.Action action)
        {
            if(source.IsInitialized)
                action();
            else
            {
                System.Action<GODependencyInjection> callback = null;
                callback = (DI) => {
                    action();
                    source.OnDidInitialize -= callback;
                };
                source.OnDidInitialize += callback;
            }
        }
    }
}