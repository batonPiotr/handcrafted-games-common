namespace HandcraftedGames.Common
{
    using UnityEngine;
    public interface ITriggerEventsProvider
    {
        event System.Action<Collider> OnTriggerEnterEvent;
        event System.Action<Collider> OnTriggerStayEvent;
        event System.Action<Collider> OnTriggerExitEvent;
    }
}