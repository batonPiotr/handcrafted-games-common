using System;
using UnityEngine;

namespace HandcraftedGames.Inventory
{
    [Serializable]
    public class ItemPropertyTradeable: IItemProperty
    {
        public string PropertyId => "tradeable";
        public string Name => "Tradeable";

        [SerializeField]
        public int value;

        public IItemProperty Clone()
        {
            return new ItemPropertyTradeable
            {
                value = this.value
            };
        }
    }
}