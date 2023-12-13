namespace HandcraftedGames.Inventory
{
    using UnityEngine;

    public class EffectPlayerBehaviour: MonoBehaviour
    {
        [SerializeField]
        public string effectRuntimeId;

        [field: SerializeField]
        public EffectPlayer currentPlayer { get; private set; }

        public EffectTemplate templateToUse;

        public void ApplyEffect(EffectTemplate template, GameObject owner = null)
        {
            if(currentPlayer != null)
                currentPlayer.Stop();
            currentPlayer = new EffectPlayer(template);
            currentPlayer.owner = owner == null ? gameObject : owner;
        }

        void OnDisable()
        {
            if(currentPlayer != null)
                currentPlayer.Stop();
        }

        [NaughtyAttributes.Button("Apply selected template")]
        public void Apply()
        {
            ApplyEffect(templateToUse);
        }

        [NaughtyAttributes.Button("Play")]
        public void PlayEffect()
        {
            currentPlayer.Start();
        }

        [NaughtyAttributes.Button("Stop")]
        public void StopEffect()
        {
            currentPlayer.Stop();
        }

        void Update()
        {
            if(currentPlayer != null && currentPlayer.runtime != null && currentPlayer.runtime.state == EffectRuntime.State.Playing)
                currentPlayer.Update(Time.deltaTime);
        }
    }
}