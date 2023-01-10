namespace HandcraftedGames.Common.Events
{
    public class DefaultEventBus: IEventBus
    {
        public event System.Action<IEvent> onEvent;
        public void Emit(IEvent emittedEvent)
        {
            onEvent?.Invoke(emittedEvent);
        }
    }
}