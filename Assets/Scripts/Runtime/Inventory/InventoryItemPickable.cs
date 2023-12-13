namespace HandcraftedGames.Inventory
{
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine;
    using HandcraftedGames.Common;

    public class InventoryItemPickable : MonoBehaviour
    {
        enum Removal
        {
            DeleteGameObject,
            DisableGameObject
        }

        [SerializeField] private Item itemToAdd;
        [SerializeField] private GameObject gameObjectToRemove;

        [SerializeField] private Removal afterPick;

        private GameObject GameObjectToRemoveOrThis
        {
            get
            {
                if (gameObjectToRemove == null)
                    return gameObject;
                return gameObjectToRemove;
            }
        }

        public UnityEngine.Events.UnityEvent OnPick;

        /// <summary>
        /// You need to connect it to your `usable` interface
        /// </summary>
        /// <param name="transform"></param> <summary>
        ///
        /// </summary>
        /// <param name="transform"></param>
        void OnUse(Transform transform)
        {
            var inventory = transform.GetComponentInChildren<IInventory>();
            if (inventory != null)
            {
                if (inventory.AddItem(itemToAdd))
                {
                    if (afterPick == Removal.DeleteGameObject)
                        GameObject.Destroy(GameObjectToRemoveOrThis);
                    else
                        GameObjectToRemoveOrThis.SetActive(false);
                }
                else
                {
                    this.LogError("Your inventory is full");
                }
            }

        }
    }
}
