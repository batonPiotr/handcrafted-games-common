using System;
using System.Collections.Generic;
using UnityEngine;

namespace HandcraftedGames.Inventory
{
    [CreateAssetMenu(fileName = "BaseItem", menuName = "Handcrafted Games/Inventory/ItemDefinition", order = 0)]
	[Serializable]
    public class ItemDefinition: ScriptableObject, ISerializationCallbackReceiver
    {
        public string itemId;
        public string itemName;
        public string shortDescription;
        public string fullDescription;
        public int maxStackSize;

        [SerializeReference, SubclassSelector]
        public IItemProperty[] properties = new IItemProperty[0];

        [SerializeField]
        public ItemEffect[] effects;

        public T GetProperty<T>() where T: IItemProperty
        {
            foreach(var p in properties)
            {
                if(p is T foundProperty)
                    return foundProperty;
            }
            return default(T);
        }

        public void OnAfterDeserialize()
        {
            // Debug.Log("ItemDefinition " + itemName + " on after deserialize");
            var dict = new Dictionary<Type, object>();
            foreach(var p in properties)
            {
                if(p == null)
                    dict[typeof(IItemProperty)] = null;
                else
                {
                    var t = p.GetType();
                    if(!dict.ContainsKey(t))
                        dict[t] = p;
                }
            }
            if(dict.Count != properties.Length)
            {
                properties = new IItemProperty[dict.Count];
                dict.Values.CopyTo(properties, 0);
            }
        }

        public void OnBeforeSerialize()
        {
            // Debug.Log("ItemDefinition " + itemName + " on before serialize");
        }
    }
}