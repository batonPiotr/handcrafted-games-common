namespace HandcraftedGames.Inventory
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class EffectRuntime
    {
        [Serializable]
        public enum State
        {
            Idle,
            Playing,
            Paused,
            Finished
        }

        public float currentTime = 0.0f;

        [SerializeField]
        public State state = State.Idle;
        public GameObject owner;
    }

    [Serializable]
    public class EffectTemplate: ScriptableObject
    {
        public string effectID;
        public bool infinite = false;

        [NaughtyAttributes.HideIf("infinite")]
        public float duration;

        public virtual EffectRuntime CreateRuntime() { return new EffectRuntime(); }

        protected virtual void OnStateChange(EffectRuntime runtime, EffectRuntime.State oldState)
        {
        }

        protected void ChangeState(EffectRuntime runtime, EffectRuntime.State newState)
        {
            if(runtime.state == newState)
                return;
            var oldState = runtime.state;
            runtime.state = newState;
            OnStateChange(runtime, oldState);
        }

        public virtual bool OnStart(EffectRuntime runtime)
        {
            if(runtime.state != EffectRuntime.State.Idle)
                return false;
            runtime.currentTime = 0;
            ChangeState(runtime, EffectRuntime.State.Playing);
            return true;
        }

        public virtual bool OnStop(EffectRuntime runtime)
        {
            if(runtime.state != EffectRuntime.State.Playing && runtime.state != EffectRuntime.State.Finished)
                return false;
            runtime.currentTime = 0;
            ChangeState(runtime, EffectRuntime.State.Idle);
            return true;
        }

        public virtual bool OnPause(EffectRuntime runtime)
        {
            if(runtime.state != EffectRuntime.State.Playing)
                return false;
            ChangeState(runtime, EffectRuntime.State.Paused);
            return true;
        }

        public virtual bool OnResume(EffectRuntime runtime)
        {
            if(runtime.state != EffectRuntime.State.Paused)
                return false;
            ChangeState(runtime, EffectRuntime.State.Playing);
            return true;
        }

        public virtual bool OnUpdate(EffectRuntime runtime, float deltaTime)
        {
            if(runtime.state != EffectRuntime.State.Playing)
                return false;
            runtime.currentTime += deltaTime;
            if(duration > 0.0f && runtime.currentTime >= duration)
                ChangeState(runtime, EffectRuntime.State.Finished);
            return true;
        }

    }
}