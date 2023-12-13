namespace HandcraftedGames.Inventory
{
    using UnityEngine;

    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Shared { get; private set; } = null;

        public ItemDefinitionDatabase ItemDefinitionDatabase;

        void Awake()
        {
            if (Shared != null)
            {
                Destroy(gameObject);
                return;
            }
            Shared = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
