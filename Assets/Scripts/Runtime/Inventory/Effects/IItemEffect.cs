namespace HandcraftedGames.Inventory
{
    using System;
    using HandcraftedGames.Common.GlobalDI;
    using UnityEngine;

    [Serializable]
    public class ItemEffect
    {
        [SerializeReference, SubclassSelector]
        public IInventoryEvent enableEvent;

        [SerializeReference, SubclassSelector]
        public IInventoryEvent disableEvent;

        [field: SerializeField]
        public EffectTemplate effectTemplate { get; private set; }

        private string GetRuntimeID(Item item)
        {
            return "item_" + item.itemId;
        }

        public void HandleEvent(IInventoryEvent e)
        {
            if(enableEvent != null && e.GetType() == enableEvent.GetType())
            {
                GlobalDependencyInjection.Shared.Resolve<EffectScheduler>().Schedule(effectTemplate, e.inventory.owner, GetRuntimeID(e.item));
            }
            else if(disableEvent != null && e.GetType() == disableEvent.GetType())
            {
                var effect = GlobalDependencyInjection.Shared.Resolve<EffectScheduler>().FindPlayer(GetRuntimeID(e.item));
                effect.StopEffect();
            }
        }
    }
}
