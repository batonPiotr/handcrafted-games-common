namespace HandcraftedGames.Inventory
{
    using System;
    using UnityEngine;


    [CreateAssetMenu(fileName = "Items Database", menuName = "Handcrafted Games/Inventory/ItemDefinition Database", order = 0)]
	[Serializable]
    public class ItemDefinitionDatabase: ScriptableObject
    {
        public ItemDefinition[] items;
        public ItemDefinition this[string key]
        {
            get
            {
                foreach(var item in items)
                {
                    if(item.itemId == key)
                        return item;
                }
                return null;
            }
        }
    }
}