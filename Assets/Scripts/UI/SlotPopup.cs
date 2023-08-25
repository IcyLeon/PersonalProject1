using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SendSlotInfo : EventArgs
{
    public Slot slot;
    public ItemButton itemButtonREF;
}

public class SlotPopup : MonoBehaviour
{
    [SerializeField] GameObject UI_GO;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] ItemContentManager ItemInfoContent;
    public event EventHandler<SendSlotInfo> onSlotSend, onSlotItemRemove;
    [SerializeField] bool AllowDragnDrop;
    private Slot SlotREF;
    private ItemButton selectedItemButton, previousselectedItemButton;

    // Update is called once per frame
    void Start()
    {
        CharacterManager.GetInstance().GetPlayerStats().onInventoryListChanged += OnInventoryListChanged;
        OnInventoryListChanged();
    }

    public ItemContentManager GetItemInfoContent()
    {
        return ItemInfoContent;
    }
    public void OnInventoryListChanged()
    {
        HideItem(null, true);

        for (int i = 0; i < CharacterManager.GetInstance().GetPlayerStats().GetINVList().Count; i++)
        {
            GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab, scrollRect.content);
            ItemButton itemButton = go.GetComponent<ItemButton>();
            DragnDrop dragnDrop = itemButton.GetComponent<DragnDrop>();
            dragnDrop.parentTransform = transform;
            itemButton.SetItemREF(CharacterManager.GetInstance().GetPlayerStats().GetINVList()[i]);

            itemButton.onButtonSpawn += OnItemSpawned;
            itemButton.onButtonClick += GetItemSelected;
            itemButton.onButtonRemoveClick += OnItemRemove;

            if (AllowDragnDrop)
            {
                dragnDrop.onBeginDragEvent += OnBeginDrag;
                dragnDrop.onDragEvent += OnDrag;
                dragnDrop.onEndDragEvent += OnEndDrag;
            }
        }
    }
    private void OnItemSpawned(ItemButton itemButton)
    {
        itemButton.DisableNewImage();
    }

    private void OnBeginDrag(PointerEventData eventData, Transform parentTransform)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;

        ItemInfoContent.TogglePopup(false);
        itemButton.OnBeginDrag(eventData, parentTransform);
    }
    private void OnDrag(PointerEventData eventData)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;
        itemButton.OnDrag(eventData);
    }
    private void OnEndDrag(PointerEventData eventData)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;
        itemButton.OnEndDrag(eventData);
    }

    public void HideItem(Item item, bool hideall = false)
    {
        foreach (RectTransform rt in scrollRect.content)
        {
            ItemButton itemButton = rt.GetComponent<ItemButton>();
            DragnDrop dragnDrop = itemButton.GetComponent<DragnDrop>();
            if (itemButton.GetItemREF() == item || hideall)
            {
                itemButton.onButtonClick -= GetItemSelected;
                itemButton.onButtonSpawn -= OnItemSpawned;
                itemButton.onButtonRemoveClick -= OnItemRemove;

                if (AllowDragnDrop)
                {
                    dragnDrop.onBeginDragEvent -= OnBeginDrag;
                    dragnDrop.onDragEvent -= OnDrag;
                    dragnDrop.onEndDragEvent -= OnEndDrag;
                }
                Destroy(itemButton.gameObject);
            }
        }

    }

    public ItemButton GetItemButton(Item item)
    {
        foreach (RectTransform rt in scrollRect.content)
        {
            ItemButton itemButton = rt.GetComponent<ItemButton>();
            if (itemButton.GetItemREF() == item)
            {
                return itemButton;
            }
        }
        return null;
    }

    private void OnItemRemove(ItemButton itemButton)
    {
        if (SlotREF == null)
            return;

        onSlotItemRemove?.Invoke(this, new SendSlotInfo { slot = SlotREF, itemButtonREF = itemButton });
    }

    private void GetItemSelected(ItemButton itemButton)
    {
        previousselectedItemButton = selectedItemButton;
        selectedItemButton = itemButton;
        AssetManager.GetInstance().UpdateCurrentSelectionOutline(previousselectedItemButton, selectedItemButton);

        if (SlotREF == null)
            return;

        onSlotSend?.Invoke(this, new SendSlotInfo { slot = SlotREF, itemButtonREF = itemButton });
    }

    public void SubscribeSlot(Slot Slot)
    {
        Slot.onSlotClick += OnSlotClick;
    }

    public void UnSubscribeSlot(Slot Slot)
    {
        Slot.onSlotClick -= OnSlotClick;
    }

    private void OnSlotClick(Slot slot)
    {
        SlotREF = slot;

        if (SlotREF.GetItemButton())
        {
            ItemInfoContent.SetItemButtonREF(SlotREF.GetItemButton());
            previousselectedItemButton = selectedItemButton;
            selectedItemButton = GetItemButton(SlotREF.GetItemButton().GetItemREF());

            AssetManager.GetInstance().UpdateCurrentSelectionOutline(previousselectedItemButton, selectedItemButton);

            ItemInfoContent.TogglePopup(true);
        }

        TogglePopup(true);
    }

    private void TogglePopup(bool active)
    {
        UI_GO.SetActive(active);
    }
}
