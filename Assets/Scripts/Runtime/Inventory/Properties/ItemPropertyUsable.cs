using System;
using HandcraftedGames.Common;
using HandcraftedGames.Common.Dependencies.SimpleJSON;
using HandcraftedGames.Common.Events;
using HandcraftedGames.Common.GlobalDI;
using HandcraftedGames.Inventory.Events;
using UnityEngine;

namespace HandcraftedGames.Inventory
{
    [Serializable]
    public class ItemPropertyUsable: IItemPropertyWithRuntimeData
    {
        public string PropertyId => "usable";
        public string Name => "Usable";

        public IItemProperty Clone()
        {
            return new ItemPropertyTradeable
            {
            };
        }

        public JSONNode Serialize()
        {
            return new JSONObject();
        }

        public void Deserialize(JSONNode data)
        {
        }

        public void Use(Item item)
        {
            GlobalDependencyInjection.Shared.Resolve<IEventBus<IInventoryEvent>>().Emit(
                new OnConsumeEvent
                {
                    item = item,
                    inventory = item.owner.owner
                }
            );
            item.owner.owner.RemoveItem(item);
        }
    }
}