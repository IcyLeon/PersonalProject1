using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemContentManager : MonoBehaviour
{
    [SerializeField] Image ItemSprite;
    [SerializeField] Image ItemCardImage;
    [SerializeField] GameObject[] ItemContent;
    [SerializeField] ItemContentDisplay ItemContentDisplay;
    [SerializeField] LockItem LockButton;
    private Item ItemREF;
    public void SetItemREF(Item item)
    {
        ItemREF = item;

        UpgradableItems upgradableItems = ItemREF as UpgradableItems;
        if (upgradableItems != null)
        {
            upgradableItems.onLevelChanged += onUpgradeLevelChanged;
        }
        DisplayContent();
    }

    private void onUpgradeLevelChanged()
    {
        DisplayContent();
    }
    private void DisplayContent()
    {
        if (ItemREF == null)
            return;

        foreach (GameObject go in ItemContent)
        {
            go.SetActive(false);
            if (ItemREF is UpgradableItems && go == ItemContent[0])
                ItemContent[0].SetActive(true);
            else if (ItemREF is ConsumableItem && go == ItemContent[1])
                ItemContent[1].SetActive(true);
        }

        ItemSprite.sprite = ItemREF.GetItemSprite();
        ItemCardImage.sprite = AssetManager.GetInstance().GetItemListTemplate().raritylist[(int)ItemREF.GetRarity()].ItemCardImage;
        ItemContentDisplay.RefreshItemContentDisplay(ItemREF);
        LockButton.SetItemREF(ItemREF);
    }
    public void TogglePopup(bool active)
    {
        gameObject.SetActive(active);
    }
}
