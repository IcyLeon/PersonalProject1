using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerExitHandler, IDropHandler
{
    private Color ImageColor;
    private Image MainImage;

    [SerializeField] GameObject slotItemButtonPrefab;
    [SerializeField] GameObject ContentContainer;
    private ItemButton itembutton;

    public delegate void SendSlotClick(Slot slot);
    public event SendSlotClick onSlotClick;

    public delegate void onSlotItemButtonChanged(SendSlotInfo sendslotInfo, Item item);
    public event onSlotItemButtonChanged SlotItemButtonChanged;

    private void Awake()
    {
        MainImage = GetComponent<Image>();
        ImageColor = MainImage.color;
    }

    void Start()
    {
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, 0);
    }
    public void SetItemButton(Item item)
    {
        if (itembutton != null)
        {
            if (itembutton.GetItemREF() != null && item != null)
                return;
        }
        else
        {
            itembutton = Instantiate(slotItemButtonPrefab, transform).GetComponent<ItemButton>();
            itembutton.onButtonSpawn += OnItemSpawned;
        }

        itembutton.CanvasGroup.blocksRaycasts = false;
        if (item != null)
            itembutton.SetItemREF(item);

        SlotItemButtonChanged?.Invoke(
            new SendSlotInfo
            {
                slot = this,
                itemButtonREF = itembutton
            },
            item
        );

        if (item == null)
        {
            itembutton.onButtonSpawn -= OnItemSpawned;
            Destroy(itembutton.gameObject);
        }
    }
    private void OnItemSpawned(ItemButton itemButton)
    {
        itemButton.DisableNewImage();
    }

    public ItemButton GetItemButton()
    {
        return itembutton;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MainImage.color = ImageColor;

        onSlotClick?.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Color newColor = new Color32(255, 255, 255, 255);
        MainImage.color = newColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MainImage.color = ImageColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;

        SetItemButton(itemButton.GetItemREF());
    }
}
