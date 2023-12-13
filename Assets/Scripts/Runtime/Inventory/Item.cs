namespace HandcraftedGames.Inventory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HandcraftedGames.Common;
    using HandcraftedGames.Common.Dependencies.SimpleJSON;
    using HandcraftedGames.Common.GlobalDI;
    using HandcraftedGames.Common.Serialization;
    using UnityEngine;

    [Serializable]
    public class Item: ISerializationCallbackReceiver, ISerializable
    {
        [NonSerialized]
        GlobalDI<InventoryEventBus> eventBus = new ();

        public System.Guid itemId = System.Guid.NewGuid();

        [field: NonSerialized]
        public Slot owner { get; private set; }

        [field: SerializeField]
        public ItemDefinition ItemDefinition { get; private set; }

        // [field: SerializeField]
        // public string RuntimeData { get; private set; }

        [field: SerializeReference]
        private IItemPropertyWithRuntimeData[] itemProperties;

        public Item()
        {
        }

        public Item(ItemDefinition itemDefinition)
        {
            this.ItemDefinition = itemDefinition;
            CopyProperties();
        }

        public void OnAddToInventorySlot(Slot slot)
        {
            owner = slot;
            eventBus.Resolved.onEvent += OnEvent;
        }

        public void OnRemoveFromInventory()
        {
            owner = null;
            eventBus.Resolved.onEvent -= OnEvent;
        }

        public T GetProperty<T>() where T: IItemProperty
        {
            if(!typeof(IItemPropertyWithRuntimeData).IsAssignableFrom(typeof(T)))
                return ItemDefinition.GetProperty<T>();
            foreach(var p in itemProperties)
            {
                if(p is T foundProperty)
                    return foundProperty;
            }
            return default(T);
        }

        private void CopyProperties()
        {
            var list = new List<IItemPropertyWithRuntimeData>();
            for(int i = 0; i < ItemDefinition.properties.Length; i++)
            {
                var p = ItemDefinition.properties[i];
                if(p is IItemPropertyWithRuntimeData pr)
                    list.Add(pr);
            }
            itemProperties = list.ToArray();
        }

        public void OnAfterDeserialize()
        {
            // Debug.Log("LOLTEST: ITEM.After deserialize");
            if(ItemDefinition == null)
                return;
            var runtimeProperties = 0;
            foreach(var p in ItemDefinition.properties)
            {
                if(p is IHasRuntimeData)
                    runtimeProperties += 1;
            }
            if(ItemDefinition == null)
                return;
            if(itemProperties == null || itemProperties.Length != runtimeProperties)
            {
                CopyProperties();
            }
        }

        public void OnBeforeSerialize()
        {
        }

        private void OnEvent(IInventoryEvent e)
        {
            if(e.inventory != owner.owner || e.item != this)
                return;
            foreach(var effect in ItemDefinition.effects)
            {
                effect.HandleEvent(e);
            }
        }

        private class SaveDataKeys
        {
            public const string itemDefinitionId = "itemDefinitionId";
            public const string runtimeProperties = "runtimeProperties";
            public const string propertyId = "propertyId";
            public const string propertyData = "data";
        }

        public JSONNode Serialize()
        {
            var json = new JSONObject();
            json[SaveDataKeys.itemDefinitionId] = ItemDefinition.itemId;
            var propertiesArray = json[SaveDataKeys.runtimeProperties].AsArray;
            foreach(var p in itemProperties)
            {
                var jsonobj = new JSONObject();
                jsonobj[SaveDataKeys.propertyId] = p.PropertyId;
                jsonobj[SaveDataKeys.propertyData] = p.Serialize();
                propertiesArray.Add(jsonobj);
            }
            return json;
        }

        public void Deserialize(JSONNode data)
        {
            this.ItemDefinition = InventorySystem.Shared.ItemDefinitionDatabase[data[SaveDataKeys.itemDefinitionId].Value];
            CopyProperties();
            var array = data[SaveDataKeys.runtimeProperties].AsArray;
            foreach(var p in array.Values)
            {
                var property = this.itemProperties.FirstOrDefault(i => i.PropertyId == p[SaveDataKeys.propertyId].Value);
                property.Deserialize(p[SaveDataKeys.propertyData]);
            }
        }
    }
}