using System;
using UnityEngine;
using HandcraftedGames.Common.Serialization;
using HandcraftedGames.Common.Dependencies.SimpleJSON;

namespace HandcraftedGames.Inventory
{

    [Serializable]
    public class ItemPropertyDamageable: IItemPropertyWithRuntimeData
    {
        public string PropertyId => "damageable";
        public string Name => "Damageable";

        [field: SerializeField]
        public int MaxHealth { get; private set; }

        [field: SerializeField]
        public int InitialHealth { get; private set; }

        public int CurrentHealth { get; set; }

        public IItemProperty Clone()
        {
            return new ItemPropertyDamageable
            {
                MaxHealth = this.MaxHealth,
                InitialHealth = this.InitialHealth,
                CurrentHealth = this.CurrentHealth
            };
        }

        private class Keys
        {
            public const string currentHealth = "currentHealth";
        }

        public void Deserialize(JSONNode data)
        {
            CurrentHealth = data[Keys.currentHealth];
        }

        public JSONNode Serialize()
        {
            var json = new JSONObject();
            json[Keys.currentHealth] = 2;
            return json;
        }
    }
}