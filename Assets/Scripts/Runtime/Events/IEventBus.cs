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
}