using System;
using UnityEngine;

namespace HandcraftedGames.Inventory
{
    [Serializable]
    public class ItemPropertyDroppable: IItemProperty
    {
        public string PropertyId => "droppable";
        public string Name => "Droppable";

        [field: SerializeField]
        public GameObject PrefabToDrop { get; private set; }

        public IItemProperty Clone()
        {
            return new ItemPropertyDroppable
            {

            };
        }
    }
}