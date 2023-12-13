namespace HandcraftedGames.Inventory
{
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using HandcraftedGames.Common;
    using HandcraftedGames.Common.GlobalDI;
    using HandcraftedGames.Common.Serialization;
    using HandcraftedGames.Common.Dependencies.SimpleJSON;

    [Serializable]
    public class Slot: List<Item>, ISerializable
    {
        [field: NonSerialized]
        public IInventory owner { get; set; }

        public new void Add(Item item)
        {
            base.Add(item);
            item.OnAddToInventorySlot(this);
        }

        public new void AddRange(IEnumerable<Item> items)
        {
            base.AddRange(items);
            foreach(var i in items)
            {
                i.OnAddToInventorySlot(this);
            }
        }

        public new bool Remove(Item item)
        {
            if(base.Remove(item))
            {
                item.OnRemoveFromInventory();
                return true;
            }
            return false;
        }

        public new void Clear()
        {
            foreach(var item in this)
            {
                item.OnRemoveFromInventory();
            }
            base.Clear();
        }

        public ItemDefinition ItemDefinition
        {
            get
            {
                var count = this.Count;
                if(count > 0)
                {
                    var def = this[0].ItemDefinition;
                    return def;
                }
                return null;
            }
        }

        public void Deserialize(JSONNode data)
        {
            var array = data["items"].AsArray;
            this.Clear();
            foreach(var item in array)
            {
                var i = new Item();
                i.Deserialize(item);
                this.Add(i);
            }
        }

        public JSONNode Serialize()
        {
            var data = new JSONObject();
            var array = data["items"].AsArray;
            foreach(var item in this)
            {
                array.Add(item.Serialize());
            }
            return data;
        }
    }

    [Serializable]
    public class SlotInventory : InjectableBehaviour, IInventory, ISerializable
    {
        GlobalDI<InventoryEventBus> inventoryEventBus = new();

        [field: SerializeField]
        public GameObject owner { get; private set; }

        [SerializeReference]
        // It's public only temporarily!
        public Slot[] slots;

        [field: SerializeField]
        public string InventoryId { get; private set; }

        [SerializeField]
        private Transform DropLocation;

        protected override void Awake()
        {
            base.Awake();
            for(int i = 0; i < slots.Length; i++)
            {
                if(slots[i] == null)
                    slots[i] = new Slot { owner = this };
            }
        }

        public bool AddItem(Item item)
        {
            var slot = FindSlot(item.ItemDefinition.itemId, false);
            if(slot == null)
                // Inventory is full, no more space
                return false;
            slot.Add(item);
            inventoryEventBus.Resolved.Emit(new Events.InventoryChangedEvent { inventory = this });
            return true;
        }

        public bool DropItem(string itemId)
        {
            var slot = FindSlot(itemId, true);
            if(slot == null)
                return false;
            var droppable = slot.ItemDefinition.GetProperty<ItemPropertyDroppable>();
            if(droppable == null)
                return false;

            var removed = RemoveItem(itemId, 1)[0];

            var go = Instantiate(droppable.PrefabToDrop);
            go.transform.position = DropLocation.position;
            return true;
        }

        public bool DropItem(Item item)
        {
            throw new System.NotImplementedException();
        }

        public int GetItemCount(string itemId)
        {
            var count = 0;
            foreach(var slot in slots)
            {
                if(slot != null && slot.ItemDefinition != null && slot.ItemDefinition.itemId == itemId)
                {
                    count += slot.Count;
                }
            }
            return count;
        }

        public bool HasItem(string itemId)
        {
            return FindSlot(itemId, true) != null;
        }

        public bool HasItem(Item item)
        {
            foreach(var slot in slots)
            {
                foreach(var i in slot)
                {
                    if(ReferenceEquals(item, i))
                        return true;
                }
            }
            return false;
        }

        public Item RemoveItem(Item item)
        {
            if(item == null)
                return null;

            if(item.owner.owner != this)
                return null;

            if(item.owner.Remove(item))
            {
                inventoryEventBus.Resolved.Emit(new Events.InventoryChangedEvent { inventory = this });
                return item;
            }
            return null;
        }

        public Item[] RemoveItem(string itemId, int quantity)
        {
            if(quantity < 1)
                return null;
            var removed = new List<Item>();
            while(removed.Count < quantity)
            {
                var slot = FindSlot(itemId, true);
                if(slot == null)
                    break;
                var toRemove = System.Math.Min(quantity - removed.Count, slot.Count);
                removed.AddRange(slot.GetRange(0, toRemove));
                slot.RemoveRange(0, toRemove);
                inventoryEventBus.Resolved.Emit(new Events.InventoryChangedEvent { inventory = this });
            }
            return removed.ToArray();
        }

        private Slot FindSlot(string itemId, bool ignoreEmpty)
        {
            Slot slot = null;
            foreach(var s in slots)
            {
                if(s != null && s.ItemDefinition != null && s.ItemDefinition.itemId == itemId)
                    return s;
            }
            if(slot == null && !ignoreEmpty)
            {
                for(int i = 0; i < slots.Length; i++)
                {
                    if(slots[i].Count == 0)
                        return slots[i];
                }
            }
            return null;
        }

        public JSONNode Serialize()
        {
            var data = new JSONObject();
            var array = data["slots"].AsArray;
            foreach(var slot in slots)
            {
                array.Add(slot.Serialize());
            }
            return data;
        }

        public void Deserialize(JSONNode data)
        {
            var array = data["slots"].AsArray;
            var slots = new List<Slot>();
            foreach(var slot in array)
            {
                var s = new Slot { owner = this };
                s.Deserialize(slot);
                slots.Add(s);
            }
            this.slots = slots.ToArray();
        }
    }
}