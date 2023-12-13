namespace HandcraftedGames.Inventory.UI
{
    using UnityEngine;
    using HandcraftedGames.Common;
    using UnityEngine.UI;

    public class SlotDetailView: MonoBehaviour
    {
        [SerializeField] private Image iconImage;

        [SerializeField] private Text itemTitle;
        [SerializeField] private Text itemShortDescription;
        [SerializeField] private Text itemLongDescription;

        [SerializeField] private Text count;

        private Slot _slot;
        public Slot Slot
        {
            get => _slot;
            set
            {
                _slot = value;
                UpdateData();
            }
        }

        private ItemDefinition _itemDefinition => _slot.Count > 0 ? _slot[0].ItemDefinition : null;

        private void UpdateData()
        {
            if(_itemDefinition == null)
                return;
            var iconProperty = _itemDefinition.GetProperty<ItemProperty2DPreview>();
            if(iconProperty != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = iconProperty.icon;
            }
            else
            {
                iconImage.enabled = false;
            }

            itemTitle.text = _itemDefinition.itemName;
            itemShortDescription.text = _itemDefinition.shortDescription;
            itemLongDescription.text = _itemDefinition.fullDescription;
            count.text = "" + _slot.Count;
        }
    }
}