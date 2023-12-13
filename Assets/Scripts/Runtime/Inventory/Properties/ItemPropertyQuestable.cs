using System;
using UnityEngine;

namespace HandcraftedGames.Inventory
{
    [Serializable]
    public class ItemPropertyQuestable: IItemProperty
    {
        public string PropertyId => "questable";
        public string Name => "Questable";

        public IItemProperty Clone()
        {
            return new ItemPropertyQuestable
            {

            };
        }
    }
}