namespace HandcraftedGames.Common.Events
{
    public interface IEvent { }

    public class EventValue<T>: IEvent
    {
        public T Value;
    }

    public interface IEventBus
    {
        void Emit(IEvent emittedEvent);

        event System.Action<IEvent> onEvent;
    }

    public interface IEventBus<EventType> where EventType: IEvent
    {
        event System.Action<EventType> onEvent;

        void Emit(EventType emittedEvent);
    }

    public class EventBus<EventType>: IEventBus<EventType> where EventType: IEvent
    {
        public event System.Action<EventType> onEvent;

        public void Emit(EventType emittedEvent)
        {
            onEvent?.Invoke(emittedEvent);
        }
    }
}