namespace HandcraftedGames.Inventory.Events
{
    using System;

    [Serializable]
    public struct InventoryChangedEvent : IInventoryEvent
    {
        public IInventory inventory { get; set; }
        public Item item { get; set; }
    }

    [Serializable]
    public struct OnConsumeEvent : IInventoryEvent
    {
        public IInventory inventory { get; set; }
        public Item item { get; set; }
        public Slot slot { get; set; }
    }
}
