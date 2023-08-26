using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerStats;

public class DisplayItemsStatsManager : MonoBehaviour
{
    [System.Serializable]
    public struct Background
    {
        public GameObject BackgroundGO;
        public Rarity Rarity;
    }
    [SerializeField] Background[] Backgrounds;

    [Header("Display Upgradable in Inventory")]
    [SerializeField] ScrollRect ScrollRect;
    [SerializeField] GameObject DetailsPanel;
    [SerializeField] LockItem LockButton;
    [SerializeField] GameObject UpgradeButton;
    [SerializeField] Image SelectedItemImage;
    [SerializeField] ItemContentDisplay ItemContentDisplay;

    [Header("Display Artifacts")]
    [SerializeField] TabGroup TabGroup;
    private Item SelectedItem;
    private ItemButton SelectedItemButton;
    private List<ItemButton> itembuttonlist = new();

    // Start is called before the first frame update
    void Start()
    {
        CharacterManager.GetInstance().GetPlayerStats().onInventoryListChanged += OnInventoryListChanged;
        OnInventoryListChanged();
        TabGroup.onTabChanged += onTabChangedEvent;
        DetailsPanel.SetActive(false);
    }

    private void Update()
    {
    }
    void SetCurrentBackground(Rarity rarity)
    {
        for (int i = 0; i < Backgrounds.Length; i++)
        {
            Backgrounds[i].BackgroundGO.SetActive(false);
            if (Backgrounds[i].Rarity == rarity)
                Backgrounds[i].BackgroundGO.SetActive(true);
        }
    }

    // Update is called once per frame
    void DisplaySelectedItem()
    {
        if (SelectedItem != null)
        {
            LockButton.SetItemREF(SelectedItem);
            ItemContentDisplay.RefreshItemContentDisplay(SelectedItem);
            SetCurrentBackground(SelectedItem.GetRarity());
        }
    }

    private void onTabChangedEvent(object sender, EventArgs e)
    {
        ScrollRect.content = TabGroup.GetCurrentTabPanel().TabPanel.GetComponent<RectTransform>();
    }

    private ItemButton GetItemButton(Item item)
    {
        foreach(ItemButton itemButton in itembuttonlist)
        {
            if (itemButton.GetItemREF() == item)
                return itemButton;
        }

        return null;
    }

    private void OnInventoryListChanged()
    {

        foreach (ItemButton itemButton in itembuttonlist)
        {
            itemButton.onButtonClick -= GetItemSelected;
            Destroy(itemButton.gameObject);
        }
        itembuttonlist.Clear();

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
            itembuttonlist.Add(itemButton);
        }

        UpdateOutlineSelection();
    }

    private void GetItemSelected(ItemButton itemButton)
    {
        SelectedItem = itemButton.GetItemREF();
        UpdateOutlineSelection();

        DetailsPanel.SetActive(SelectedItem != null);

        if (SelectedItem == null)
            return;

        SelectedItemImage.sprite = SelectedItem.GetItemSprite();

        DisplaySelectedItem();
    }

    public Item GetItemCurrentSelected()
    {
        return SelectedItem;
    }

    private void UpdateOutlineSelection()
    {
        foreach (ItemButton itemButton in itembuttonlist)
        {
            AssetManager.GetInstance().UpdateCurrentSelectionOutline(itemButton, null);
        }
        SelectedItemButton = GetItemButton(SelectedItem);
        UpgradeButton.GetComponent<UpgradeCanvasTransition>().SetItemButtonREF(SelectedItemButton);
        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, SelectedItemButton);
    }
}
