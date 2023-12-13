namespace HandcraftedGames.Inventory
{
    using UnityEngine;
    using HandcraftedGames.Common;
    using HandcraftedGames.Common.GlobalDI;

    public class EffectScheduler: MonoBehaviour
    {

        void Awake()
        {
            GlobalDependencyInjection.Shared.Add(this);
        }

        void OnDestroy()
        {
            GlobalDependencyInjection.Shared.Remove(this);
        }

        public EffectPlayerBehaviour Schedule(EffectTemplate effect, GameObject owner, string effectRuntimeID, bool autoplay = true)
        {
            var go = new GameObject("Effect - " + effectRuntimeID);
            go.transform.SetParent(transform, false);
            var behaviour = go.AddComponent<EffectPlayerBehaviour>();
            behaviour.ApplyEffect(effect, owner);
            behaviour.effectRuntimeId = effectRuntimeID;
            if(autoplay)
                behaviour.PlayEffect();

            return behaviour;
        }

        public EffectPlayerBehaviour FindPlayer(string effectRuntimeID)
        {
            var players = FindObjectsOfType<EffectPlayerBehaviour>();
            foreach(var p in players)
            {
                if(p.effectRuntimeId == effectRuntimeID)
                    return p;
            }
            return null;
        }
    }
}