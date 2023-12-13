namespace HandcraftedGames.Inventory.UI
{
    using System.Collections.Generic;
    using System.Collections;
    using System;
    using HandcraftedGames.Common.Events;
    using HandcraftedGames.Common;
    using UniRx;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEngine;

    [Serializable]
    public class SlotViewUnityEvent : UnityEvent<SlotView> { }

    public class SlotView : Selectable
    {
        [Inject] private IEventBus eventBus;

        public SlotInventory targetInventory;
        public Slot targetSlot;

        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private Image highlightImage;
        // public int index;

        public event System.Action<SlotView> OnSelectEvent;
        public event System.Action<SlotView> OnDeselectEvent;

        public SlotViewUnityEvent OnSelectUnityEvent;
        public SlotViewUnityEvent OnDeselectUnityEvent;

        // [Button("Refresh")]
        public void Refresh()
        {
            iconImage.enabled = false;
            highlightImage.enabled = false;
            // var slot = targetInventory.Content[index];
            if (targetSlot != null && targetSlot.Count > 0)
            {
                var preview = targetSlot[0].GetProperty<ItemProperty2DPreview>();
                if (preview == null)
                    return;
                iconImage.enabled = true;
                iconImage.sprite = preview.icon;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            highlightImage.enabled = true;
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            highlightImage.enabled = false;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnSelectEvent?.Invoke(this);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            OnDeselectEvent?.Invoke(this);
        }
    }
}
