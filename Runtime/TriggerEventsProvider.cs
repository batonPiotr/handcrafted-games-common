namespace HandcraftedGames.Common
{
    using UnityEngine;
    using UnityEngine.Events;

    public class TriggerEventsProvider : MonoBehaviour, ITriggerEventsProvider
    {
        public UnityEvent<Collider> OnTriggerEnterUEvent;
        public UnityEvent<Collider> OnTriggerStayUEvent;
        public UnityEvent<Collider> OnTriggerExitUEvent;

        public event System.Action<Collider> OnTriggerEnterEvent;
        public event System.Action<Collider> OnTriggerStayEvent;
        public event System.Action<Collider> OnTriggerExitEvent;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayEvent?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitEvent?.Invoke(other);
        }
    }
}