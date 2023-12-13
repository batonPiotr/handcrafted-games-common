/* Just an example

namespace HandcraftedGames
{
    using UnityEngine;
    using System;
    using HandcraftedGames.TheDayThatShouldNotBe;

    [CreateAssetMenu(fileName = "HealPlayer", menuName = "Handcrafted Games/Effects/Heal Player")]
    [Serializable]
    public class HealPlayerEffectTemplate: EffectTemplate
    {
        public float healPercentage = 0.25f;
        protected override void OnStateChange(EffectRuntime runtime, EffectRuntime.State oldState)
        {
            if(runtime.state == EffectRuntime.State.Playing)
            {
                if(runtime.owner != null)
                {
                    var needs = runtime.owner.GetComponentInChildren<CharacterNeeds>();
                    if(needs != null)
                        needs.Hunger.CurrentValue += needs.Hunger.Value * healPercentage;
                }
                ChangeState(runtime, EffectRuntime.State.Finished);
            }
        }
    }
}
*/