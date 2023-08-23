using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerStats;

public class DisplayItemsStatsManager : MonoBehaviour
{
    [Header("Display Upgradable in Inventory")]
    [SerializeField] ScrollRect ScrollRect;
    [SerializeField] GameObject DetailsPanel;
    [SerializeField] LockItem LockButton;
    [SerializeField] GameObject UpgradeButton;
    [SerializeField] Image SelectedItemImage;
    [SerializeField] ItemContentDisplay ItemContentDisplay;

    [Header("Display Artifacts")]
    [SerializeField] TabGroup TabGroup;
    private ItemButton selectedItemButton;

    // Start is called before the first frame update
    void Start()
    {
        CharacterManager.GetInstance().GetPlayerStats().onInventoryListChanged += OnInventoryListChanged;
        OnInventoryListChanged();
        TabGroup.onTabChanged += onTabChangedEvent;
        DetailsPanel.SetActive(false);
    }


    // Update is called once per frame
    void DisplaySelectedItem()
    {
        if (selectedItemButton != null)
        {
            LockButton.SetItemREF(selectedItemButton.GetItemREF());
            UpgradeButton.GetComponent<UpgradeCanvasTransition>().SetItemButtonREF(selectedItemButton);
            ItemContentDisplay.RefreshItemContentDisplay(selectedItemButton.GetItemREF());
        }
    }

    private void onTabChangedEvent(object sender, EventArgs e)
    {
        ScrollRect.content = TabGroup.GetCurrentTabPanel().TabPanel.GetComponent<RectTransform>();
    }


    private void OnInventoryListChanged()
    {
        for (int i = 0; i < TabGroup.GetTabMenuList().Length; i++)
        {
            foreach (ItemButton itemButton in TabGroup.GetTabMenuList()[i].TabPanel.GetComponentsInChildren<ItemButton>())
            {
                itemButton.onButtonClick -= GetItemSelected;
                Destroy(itemButton.gameObject);
            }
        }

        for (int i = 0; i < CharacterManager.GetInstance().GetPlayerStats().GetINVList().Count; i++)
        {
            GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab);
            ItemButton itemButton = go.GetComponent<ItemButton>();
            itemButton.SetItemREF(CharacterManager.GetInstance().GetPlayerStats().GetINVList()[i]);

            UpgradableItems UpgradableItemREF = itemButton.GetItemREF() as UpgradableItems;
            switch (UpgradableItemREF.GetCategory)
            {
                case Category.ARTIFACTS:
                    Artifacts artifacts = UpgradableItemREF as Artifacts;
                    itemButton.gameObject.transform.SetParent(TabGroup.GetTabMenuList()[TabGroup.GetTabPanelIdx(artifacts.type)].TabPanel.transform);
                    break;
            }
            itemButton.onButtonClick += GetItemSelected;
        }
    }

    private void GetItemSelected(ItemButton itemButton)
    {
        selectedItemButton = itemButton;
        SelectedItemImage.sprite = selectedItemButton.GetItemREF().GetItemSprite();

        if (selectedItemButton == null)
            return;

        if (selectedItemButton.GetItemREF() != null)
            DetailsPanel.SetActive(true);
        else
            DetailsPanel.SetActive(false);

        DisplaySelectedItem();
    }

    public Item GetItemCurrentSelected()
    {
        if (selectedItemButton == null)
            return null;

        return selectedItemButton.GetItemREF();
    }
}
