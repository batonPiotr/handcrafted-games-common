/* Just an example

namespace HandcraftedGames
{
    using UnityEngine;
    using System;

    [CreateAssetMenu(fileName = "BurningEffect", menuName = "Handcrafted Games/Effects/Burning")]
    [Serializable]
    public class BurningEffectTemplate: EffectTemplate
    {
        [Serializable]
        public class Runtime: EffectRuntime
        {
            public GameObject spawnedObject;
        }
        public GameObject fireParticles;

        public override EffectRuntime CreateRuntime()
        {
            return new Runtime();
        }

        protected override void OnStateChange(EffectRuntime runtime, EffectRuntime.State oldState)
        {
            var casted = runtime as Runtime;
            if(runtime.state == EffectRuntime.State.Playing)
            {
                var newGo = GameObject.Instantiate(fireParticles);
                newGo.transform.SetParent(casted.owner.transform);
                newGo.transform.localPosition = Vector3.zero;
                casted.spawnedObject = newGo;
            }
            else if(runtime.state == EffectRuntime.State.Idle || runtime.state == EffectRuntime.State.Finished)
            {
                if(casted.spawnedObject == null)
                {
                    return;
                }
                var particles = casted.spawnedObject.GetComponentsInChildren<ParticleSystem>();
                foreach(var p in particles)
                {
                    p.Stop();
                }
                GameObject.Destroy(casted.spawnedObject, 10.0f);
            }
        }
    }
}
*/