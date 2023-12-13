namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class DataList<T> : List<T>
    {
        protected DataList()
        { }
    }

    [Serializable]
    public class DataNumberList : DataList<double> { }

    [Serializable]
    public class DataStringList : DataList<string> { }

    [Serializable]
    public class DataBoolList : DataList<bool> { }

    [Serializable]
    public class DataContainerList : DataList<DataContainer> { }

    [Serializable]
    public class DataContainer
    {
        private Dictionary<string, DataContainer> data = new();

        private object value;

        public object Build()
        {
            if(value is double || value is string || value is bool)
                return value;
            if(value is DataStringList || value is DataNumberList || value is DataBoolList)
                return value;
            if(value is DataContainerList dataContainerList)
                return dataContainerList.Select(i => i.Build());
            var retVal = new Dictionary<string, object>();
            if(data == null)
                return retVal;
            foreach(var entry in data)
            {
                retVal[entry.Key] = entry.Value.Build();
            }
            return retVal;
        }

        public double Double
        {
            get
            {
                return (double) value;
            }
            set
            {
                this.value = value;
                data = null;
            }
        }
        public string String
        {
            get
            {
                return (string) value;
            }
            set
            {
                this.value = value;
                data = null;
            }
        }
        public bool Bool
        {
            get
            {
                return (bool) value;
            }
            set
            {
                this.value = value;
                data = null;
            }
        }
        public DataBoolList BoolList
        {
            get
            {
                return (DataBoolList) value;
            }
            set
            {
                this.value = value;
                data = null;
            }
        }
        public DataNumberList NumberList
        {
            get
            {
                return (DataNumberList) value;
            }
            set
            {
                this.value = value;
                data = null;
            }
        }
        public DataStringList StringList
        {
            get
            {
                return (DataStringList) value;
            }
            set
            {
                this.value = value;
                data = null;
            }
        }
        public DataContainerList ContainerList
        {
            get
            {
                return (DataContainerList) value;
            }
            set
            {
                this.value = value;
                data = null;
            }
        }

        public DataContainer this [string key]
        {
            get
            {
                if (data == null)
                    data = new();
                DataContainer o;
                if (data.TryGetValue(key, out o))
                    return o;
                o = new();
                data[key] = o;
                return o;
            }
            set
            {
                this.value = null;
                if (data == null)
                    data = new();
                data[key] = value;
            }
        }
    }
}