namespace HandcraftedGames.Inventory
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class EffectPlayer
    {
        [field: SerializeReference]
        public EffectRuntime runtime { get; private set; }

        [field: SerializeField]
        public EffectTemplate template { get; private set; }

        public GameObject owner;

        public EffectPlayer(EffectTemplate template)
        {
            this.template = template;
        }

        public void Start()
        {
            if(runtime == null)
                runtime = template.CreateRuntime();
            runtime.owner = owner;
            template.OnStart(runtime);
        }

        public void Stop()
        {
            template?.OnStop(runtime);
        }

        public void Pause()
        {
            template.OnPause(runtime);
        }

        public void Resume()
        {
            template.OnResume(runtime);
        }

        public void Update(float deltaTime)
        {
            template.OnUpdate(runtime, deltaTime);
        }
    }
}