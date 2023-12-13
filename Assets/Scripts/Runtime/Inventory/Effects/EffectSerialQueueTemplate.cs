namespace HandcraftedGames.Inventory
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SerialQueue", menuName = "Handcrafted Games/Effects/Serial Queue")]
    [Serializable]
    public class EffectSerialQueueTemplate: EffectTemplate
    {
        [Serializable]
        public class Runtime: EffectRuntime
        {
            public int currentIndex;

            [SerializeReference]
            public EffectRuntime[] runtimes;
        }

        [SerializeReference, NaughtyAttributes.Expandable]
        public EffectTemplate[] steps;

        public override EffectRuntime CreateRuntime()
        {
            return new Runtime
            {
                currentIndex = 0,
                runtimes = steps.Select(i => i.CreateRuntime()).ToArray()
            };
        }

        private EffectRuntime CurrentRuntime(Runtime runtime)
        {
            var r = runtime.runtimes[runtime.currentIndex];
            r.owner = runtime.owner;
            return r;
        }

        private EffectTemplate CurrentStep(Runtime runtime)
        {
            return steps[runtime.currentIndex];
        }

        public override bool OnStart(EffectRuntime runtime)
        {
            if(!base.OnStart(runtime))
                return false;
            var runtimeCasted = runtime as Runtime;
            runtimeCasted.currentIndex = 0;
            CurrentStep(runtimeCasted).OnStart(CurrentRuntime(runtimeCasted));
            return true;
        }
        public override bool OnStop(EffectRuntime runtime)
        {
            if(!base.OnStop(runtime))
                return false;
            var runtimeCasted = runtime as Runtime;
            for(int i = 0; i < steps.Length; i++)
            {
                steps[i].OnStop(runtimeCasted.runtimes[i]);
            }
            // CurrentStep(runtimeCasted).OnStop(CurrentRuntime(runtimeCasted));
            runtimeCasted.currentIndex = 0;
            return true;
        }
        public override bool OnPause(EffectRuntime runtime)
        {
            if(!base.OnPause(runtime))
                return false;
            var runtimeCasted = runtime as Runtime;
            CurrentStep(runtimeCasted).OnPause(CurrentRuntime(runtimeCasted));
            return true;
        }
        public override bool OnResume(EffectRuntime runtime)
        {
            if(!base.OnResume(runtime))
                return false;
            var runtimeCasted = runtime as Runtime;
            CurrentStep(runtimeCasted).OnResume(CurrentRuntime(runtimeCasted));
            return true;
        }
        public override bool OnUpdate(EffectRuntime runtime, float deltaTime)
        {
            if(!base.OnUpdate(runtime, deltaTime))
                return false;
            var runtimeCasted = runtime as Runtime;
            var currentRuntime = CurrentRuntime(runtimeCasted);
            var currentStep = CurrentStep(runtimeCasted);
            currentStep.OnUpdate(currentRuntime, deltaTime);
            if(currentRuntime.state == EffectRuntime.State.Finished)
            {
                runtimeCasted.currentIndex += 1;
                if(runtimeCasted.currentIndex >= steps.Length)
                {
                    ChangeState(runtime, EffectRuntime.State.Finished);
                    return true;
                }
                return CurrentStep(runtimeCasted).OnStart(CurrentRuntime(runtimeCasted));
            }
            return true;
        }
    }
}