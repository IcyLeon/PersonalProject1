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
    private ItemButton ItemButtonREF;
    public void SetItemButtonREF(ItemButton itembutton)
    {
        ItemButtonREF = itembutton;
        DisplayContent();
    }

    private void DisplayContent()
    {
        if (!ItemButtonREF)
            return;
        else
            if (ItemButtonREF.GetItemREF() == null)
                    return;

        foreach (GameObject go in ItemContent)
        {
            go.SetActive(false);
            if (ItemButtonREF.GetItemREF() is UpgradableItems && go == ItemContent[0])
                ItemContent[0].SetActive(true);
            else if (ItemButtonREF.GetItemREF() is ConsumableItem && go == ItemContent[1])
                ItemContent[1].SetActive(true);
        }

        ItemSprite.sprite = ItemButtonREF.GetItemREF().GetItemSprite();
        ItemCardImage.sprite = ItemButtonREF.GetComponent<GenerateBorder>().GetItemListTemplate().raritylist[(int)ItemButtonREF.GetItemREF().GetRarity()].ItemCardImage;
        ItemContentDisplay.RefreshItemContentDisplay(ItemButtonREF.GetItemREF());
        LockButton.SetItemREF(ItemButtonREF.GetItemREF());
    }
    public void TogglePopup(bool active)
    {
        gameObject.SetActive(active);
    }
}
