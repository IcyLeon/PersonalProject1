using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ItemsList;

public class GenerateBorder : MonoBehaviour
{
    private Image[] StarDisplayList;
    [SerializeField] ItemsList itemlisttemplate;
    [SerializeField] GameObject StarDisplayContainer;
    [SerializeField] ItemButton itemButton;
    [SerializeField] TextMeshProUGUI ConsumableAmountTxt;
    [SerializeField] TextMeshProUGUI UpgradableTxt;

    private void Awake()
    {
        StarDisplayList = StarDisplayContainer.GetComponentsInChildren<Image>();
    }
    private void Start()
    {
        if (itemButton.GetItemREF() != null)
        {
            StarDisplayContainer.SetActive(true);
            SetDisplayStars(itemButton.GetItemREF().GetRarity());

            switch(itemButton.GetItemREF())
            {
                case Item item when item is UpgradableItems:
                    UpgradableTxt.gameObject.SetActive(true);
                    break;
                case Item item when item is ConsumableItem:
                    ConsumableAmountTxt.gameObject.SetActive(true);
                    break;
            }
        }
    }

    private void Update()
    {
        if (itemButton.GetItemREF() != null)
        {
            switch (itemButton.GetItemREF())
            {
                case Item item when item is UpgradableItems:
                    UpgradableItems upgradableItems = (UpgradableItems)item;
                    UpgradableTxt.text = "+" + upgradableItems.GetLevel();
                    break;
                case Item item when item is ConsumableItem:
                    ConsumableItem consumableItem = (ConsumableItem)item;
                    ConsumableAmountTxt.text = consumableItem.GetAmount().ToString();
                    break;
            }
        }
    }

    public ItemsList GetItemListTemplate()
    {
        return itemlisttemplate;
    }
    public void SetDisplayStars(Rarity rarity)
    {
        for (int i = 0; i < StarDisplayList.Length; i++)
        {
            if (i <= (int)rarity)
            {
                StarDisplayList[i].gameObject.SetActive(true);
            }
            else
            {
                StarDisplayList[i].gameObject.SetActive(false);
            }
        }
        GetComponent<Image>().sprite = itemlisttemplate.raritylist[(int)rarity].rarityborderimage;
    }
}
