namespace HandcraftedGames.Inventory
{
    using HandcraftedGames.Common.Events;

    public interface IInventoryEvent: IEvent
    {
        IInventory inventory { get; }
        Item item { get; }
    }
    public class InventoryEventBus: EventBus<IInventoryEvent>
    {
    }
}