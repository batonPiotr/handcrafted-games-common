namespace HandcraftedGames.Inventory
{
    using HandcraftedGames.Common.Events;

    public interface IInventory
    {
        UnityEngine.GameObject owner { get; }
        string InventoryId { get; }
        /// <summary>
        /// Checks if inventory contains any item with the itemId
        /// </summary>
        bool HasItem(string itemId);
        /// <summary>
        /// Checks if inventory contains this specific instance
        /// </summary>
        bool HasItem(Item item);
        /// <summary>
        /// Returns amount of items with given `itemId`
        /// </summary>
        int GetItemCount(string itemId);
        /// <summary>
        /// Adds item
        /// </summary>
        /// <param name="item">Instance that will be added</param>
        /// <returns>Returns true if operation succeeded.</returns>
        bool AddItem(Item item);
        /// <summary>
        /// Removes item in given quantity
        /// </summary>
        Item[] RemoveItem(string itemId, int quantity);
        Item RemoveItem(Item item);
        /// <summary>
        /// Drops one item with given ID
        /// </summary>
        bool DropItem(string itemId);
        /// <summary>
        /// Drops this specific instance.
        /// </summary>
        bool DropItem(Item item);
    }
}