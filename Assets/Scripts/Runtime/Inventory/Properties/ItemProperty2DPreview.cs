using System;
using UnityEngine;

namespace HandcraftedGames.Inventory
{
    [Serializable]
    public class ItemProperty2DPreview: IItemProperty
    {
        public string PropertyId => "droppable";
        public string Name => "Droppable";
        public Sprite icon;

        public IItemProperty Clone()
        {
            return new ItemProperty2DPreview
            {
                icon = icon
            };
        }
    }
}