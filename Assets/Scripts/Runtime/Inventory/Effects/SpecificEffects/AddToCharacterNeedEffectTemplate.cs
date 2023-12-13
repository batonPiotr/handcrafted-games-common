/* Just an example

namespace HandcraftedGames
{
    using UnityEngine;
    using System;
    using HandcraftedGames.TheDayThatShouldNotBe;

    [CreateAssetMenu(fileName = "ReplenishNeed", menuName = "Handcrafted Games/Effects/Replenish need")]
    [Serializable]
    public class AddToCharacterNeedEffectTemplate: EffectTemplate
    {
        public enum Need
        {
            Hunger,
            Stamina,
            Thirst,
            Sleep
        }
        public Need needToReplenish;
        public float healPercentage = 0.25f;
        protected override void OnStateChange(EffectRuntime runtime, EffectRuntime.State oldState)
        {
            if(runtime.state == EffectRuntime.State.Playing)
            {
                if(runtime.owner != null)
                {
                    CharacterNeed selectedNeed = null;
                    var needs = runtime.owner.GetComponentInChildren<CharacterNeeds>();
                    if(needs != null)
                    {
                        switch(needToReplenish)
                        {
                            case Need.Hunger:
                            selectedNeed = needs.Hunger;
                            break;
                            case Need.Stamina:
                            selectedNeed = needs.Stamina;
                            break;
                            case Need.Thirst:
                            selectedNeed = needs.Thirst;
                            break;
                            case Need.Sleep:
                            selectedNeed = needs.Sleep;
                            break;
                        }
                        selectedNeed.CurrentValue += selectedNeed.Value * healPercentage;
                    }
                }
                ChangeState(runtime, EffectRuntime.State.Finished);
            }
        }
    }
}
*/