namespace HandcraftedGames.Common.GlobalDI
{
    using System.Collections.Generic;

    public class GlobalDI<T> where T: class
    {
        private T _resolved;
        public T Resolved
        {
            get
            {
                if(_resolved == null)
                {
                    _resolved = GlobalDependencyInjection.Shared.Resolve<T>();
                }
                return _resolved;
            }
        }
    }

    public class GlobalDependencyInjection
    {
        public static GlobalDependencyInjection Shared = new GlobalDependencyInjection();

        private GlobalDependencyInjection(){}

        List<object> objects = new();

        public void Add<T>(T dependency) where T: class
        {
            objects.Add(dependency);
        }

        public void Remove<T>(T dependency) where T: class
        {
            objects.Remove(dependency);
        }

        public T Resolve<T>() where T: class
        {
            foreach(var d in objects)
            {
                if(d is T found)
                    return found;
            }
            foreach(var d in objects)
            {
                if(!d.GetType().IsAssignableFrom(typeof(T)))
                    return (T)d;
            }
            return null;
        }
    }
}