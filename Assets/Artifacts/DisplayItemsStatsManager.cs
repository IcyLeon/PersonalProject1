using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItemsStatsManager : MonoBehaviour
{
    [System.Serializable]
    public struct Background
    {
        public GameObject BackgroundGO;
        public Color ParticlesColor;
        public Rarity Rarity;
    }
    [SerializeField] ParticleSystem fog;
    [SerializeField] Background[] Backgrounds;

    [Header("Display Upgradable in Inventory")]
    [SerializeField] ScrollRect ScrollRect;
    [SerializeField] GameObject DetailsPanel;
    [SerializeField] LockItem LockButton;
    [SerializeField] GameObject UpgradeButton;
    [SerializeField] Image SelectedItemImage;
    [SerializeField] ItemContentDisplay ItemContentDisplay;
    [SerializeField] ParticleSystem burst;

    [Header("Display Artifacts")]
    [SerializeField] ArtifactTabGroup TabGroup;
    private Item SelectedItem, PreviousSelectedItem;
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


    private void ChangeParticleColor(Color color)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[fog.particleCount];
        int numParticles = fog.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            particles[i].startColor = color;
        }

        fog.SetParticles(particles, numParticles);

        var fogMain = fog.main;
        fogMain.startColor = color;
    }

    void SetCurrentBackground(Rarity rarity)
    {
        for (int i = 0; i < Backgrounds.Length; i++)
        {
            Backgrounds[i].BackgroundGO.SetActive(false);
            if (Backgrounds[i].Rarity == rarity)
            {
                Backgrounds[i].BackgroundGO.SetActive(true);
                ChangeParticleColor(Backgrounds[i].ParticlesColor);
            }
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
            Item item = CharacterManager.GetInstance().GetPlayerStats().GetINVList()[i];
            if (item is not UpgradableItems)
                continue;

            GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab);
            ItemButton itemButton = go.GetComponent<ItemButton>();
            itemButton.SetItemREF(item);

            UpgradableItems UpgradableItemREF = itemButton.GetItemREF() as UpgradableItems;
            switch (UpgradableItemREF.GetCategory)
            {
                case Category.ARTIFACTS:
                    Artifacts artifacts = UpgradableItemREF as Artifacts;
                    itemButton.gameObject.transform.SetParent(TabGroup.GetTabMenuList()[TabGroup.GetTabPanelIdx(artifacts.type)].TabPanel.transform);
                    break;
            }
            itemButton.onButtonClick += GetItemSelected;
            UpgradableItemREF.onLevelChanged += onItemUpgrade;
            itembuttonlist.Add(itemButton);
        }

        UpdateOutlineSelection();
    }

    private void GetItemSelected(ItemButton itemButton)
    {
        PreviousSelectedItem = SelectedItem;
        SelectedItem = itemButton.GetItemREF();
        UpdateOutlineSelection();

        DetailsPanel.SetActive(SelectedItem != null);

        if (SelectedItem == null)
            return;

        SelectedItemImage.sprite = SelectedItem.GetItemSprite();
        if (SelectedItem != PreviousSelectedItem)
        {
            burst.Emit(1);
        }

        DisplaySelectedItem();
    }

    private void onItemUpgrade()
    {
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
        UpgradeButton.GetComponent<UpgradeCanvasTransition>().SetItemREF(SelectedItem);
        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, SelectedItemButton);
    }
}
