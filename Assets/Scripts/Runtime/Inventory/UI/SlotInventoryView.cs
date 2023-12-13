namespace HandcraftedGames.Inventory.UI
{
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine.UI;
    using UnityEngine;
    using HandcraftedGames.Common;
    using UniRx;
    using System;
    using HandcraftedGames.Common.GlobalDI;
    using HandcraftedGames.Common.Events;

    public class SlotInventoryView : InjectableBehaviour
    {
        GlobalDI<InventoryEventBus> inventoryEventBus = new();
        public string inventoryID;

        [NonSerialized]
        public SlotInventory targetInventory;
        public GameObject slotPrefab;
        public RectTransform slotsTarget;
        public SlotDetailView detailView;

        public Button UseButton;
        public Button DropButton;

        public SlotView SelectedSlotView { get; private set; }

        public void Toggle(bool show)
        {
            if(show)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        void OnEnable()
        {
            if(inventoryEventBus.Resolved == null)
                return;
            inventoryEventBus.Resolved.onEvent += OnInventoryEvent;
            FindInventory();
            FillSlots();
        }

        void OnDisable()
        {
            if(inventoryEventBus.Resolved == null)
                return;
            inventoryEventBus.Resolved.onEvent -= OnInventoryEvent;
        }

        private void OnInventoryEvent(IInventoryEvent e)
        {
            if(e is Events.InventoryChangedEvent changedEvent)
            {
                if(changedEvent.inventory.InventoryId != inventoryID)
                    return;
                FindInventory();
                FillSlots();
                detailView.gameObject.SetActive(false);
                DropButton.interactable = false;
                UseButton.interactable = false;
            }
        }

        public void FindInventory()
        {
            if(targetInventory != null)
                return;
            var inventories = FindObjectsOfType<SlotInventory>();
            foreach (var i in inventories)
            {
                if (i.InventoryId == inventoryID)
                {
                    targetInventory = i;
                    break;
                }
            }
        }

        public void FillSlots()
        {
            detailView.gameObject.SetActive(false);
            while (slotsTarget.childCount > 0)
            {
                var c = slotsTarget.GetChild(0);
                c.SetParent(null);
                DestroyImmediate(c.gameObject);
            }
            for (int i = 0; i < targetInventory.slots.Length; i++)
            {
                // if(targetInventory.slots[i].Count == 0)
                //     continue;
                // var slot = targetInventory.Content[i];
                var slotGO = GameObject.Instantiate(slotPrefab);
                slotGO.transform.SetParent(slotsTarget);
                var slotView = slotGO.GetComponent<SlotView>();
                if (slotView == null)
                {
                    Debug.Log("No inventory view in slot prefab");
                    return;
                }
                slotView.targetInventory = targetInventory;
                slotView.transform.localScale = Vector3.one;
                slotView.transform.localPosition = Vector3.zero;
                slotView.targetSlot = targetInventory.slots[i];
                slotView.Refresh();
                slotView.gameObject.layer = slotsTarget.gameObject.layer;
                slotView.OnSelectEvent += slot =>
                {
                    SelectedSlotView = slot;
                    detailView.Slot = slot.targetSlot;
                    detailView.gameObject.SetActive(slot.targetSlot.Count > 0);
                    DropButton.interactable = slot.targetSlot.ItemDefinition != null && slot.targetSlot.ItemDefinition.GetProperty<ItemPropertyDroppable>() != null;
                    UseButton.interactable = slot.targetSlot.ItemDefinition != null && slot.targetSlot.ItemDefinition.GetProperty<ItemPropertyUsable>() != null;
                };
            }
        }

        public void OnDropButton()
        {
            var itemId = SelectedSlotView.targetSlot.ItemDefinition.itemId;
            targetInventory.DropItem(itemId);
        }

        public void OnUseButton()
        {
            SelectedSlotView.targetSlot[0].GetProperty<ItemPropertyUsable>().Use(SelectedSlotView.targetSlot[0]);
        }
    }
}
