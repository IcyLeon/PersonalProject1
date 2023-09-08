using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class EnhancementManager : MonoBehaviour
{
    [SerializeField] GameObject MaxLevelContent;
    [SerializeField] GameObject EnhancementContent;

    [Header("Enhancement Infomation")]
    [SerializeField] int EnhancementItemToSell;
    [SerializeField] UpgradeArtifactCanvas upgradeArtifactCanvas;
    [SerializeField] Image selectedItemImage;
    [SerializeField] TextMeshProUGUI LevelDisplay, ExpAmountDisplay, IncreaseAmountExpDisplay, IncreaseLevelDisplay;
    [SerializeField] Button AutoAddBtn;
    [SerializeField] Button UpgradeBtn;
    [SerializeField] Button ClearAllBtn;
    [SerializeField] Slider upgradeProgressSlider, previewProgressSlider;
    [SerializeField] TMP_Dropdown AutoAddSelection;
    [SerializeField] ItemsList itemList;
    [SerializeField] Transform SlotsParent;
    List<GameObject> EnhancementItemList = new List<GameObject>();
    private int NoofItemsAdded;
    private int RaritySelection;
    private int[] CostList;
    private float PreviewUpgradeEXP;
    private bool UpgradinginProgress = false;

    [Header("Artifacts Stats")]
    [SerializeField] GameObject[] ArtifactsStatsContainer;

    // Start is called before the first frame update
    void Start()
    {
        CharacterManager.GetInstance().GetPlayerStats().onInventoryListChanged += OnInventoryListChanged;
        OnInventoryListChanged();

        LoadEmptySlots();
        AutoAddBtn.onClick.AddListener(AutoAdd);
        UpgradeBtn.onClick.AddListener(UpgradeItem);
        ClearAllBtn.onClick.AddListener(ClearAll);
        AutoAddSelection.onValueChanged.AddListener(delegate
        {
            RaritySelection = AutoAddSelection.value;
        }
        );

    }

    public void SetExpDisplay()
    {
        UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;

        CostList = itemList.GetCostListStatus(GetItemREF().GetRarity());
        if (UpgradableItemREF.GetLevel() < CostList.Length)
            SetProgressUpgrades(0, CostList[UpgradableItemREF.GetLevel()]);
        else
            SetProgressUpgrades(0, 0);

        upgradeProgressSlider.value = UpgradableItemREF.GetExpAmount();
    }

    private void Update()
    {
        for (int i = EnhancementItemList.Count - 1; i >= 0; i--)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton()?.GetItemREF() is UpgradableItems upgradableItem && upgradableItem.GetLockStatus())
            {
                slot.SetItemButton(null);
            }
        }

        UpdatePreviewEXP();
        if (!UpgradinginProgress)
            UpdateContent();
    }

    public Item GetItemREF()
    {
        return upgradeArtifactCanvas.GetItemREF();
    }

    private ItemButton CheckIfItemalreadyExist(Item item)
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null) {
                if (slot.GetItemButton().GetItemREF() == item)
                    return slot.GetItemButton();
            }
        }
        return null;
    }


    private Slot GetSlotAt(ItemButton itembutton)
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
            {
                if (slot.GetItemButton().GetItemREF() == itembutton.GetItemREF())
                {
                    return slot;
                }
            }
        }
        return null;
    }

    private Slot FindAvailableSlot()
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            
            if (slot.GetItemButton() == null)
            {
                return slot;
            }
            else
            {
                if (slot.GetItemButton().GetItemREF() == null)
                    return slot;
            }
        }
        return null;
    }

    private int GetLevelIncrease()
    {
        UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
        int increaseLevel = 0;
        float current = PreviewUpgradeEXP;

        for (int i = 0; i < CostList.Length; i++)
        {
            if (i >= UpgradableItemREF.GetLevel())
            {
                if (current < CostList[i])
                    break;

                current -= CostList[i];
                increaseLevel++;
            }
        }
        return increaseLevel;
    }

    private void UpdatePreviewEXP()
    {
        UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
        PreviewUpgradeEXP = UpgradableItemREF.GetExpAmount() + GetTotalEXP();
        previewProgressSlider.minValue = upgradeProgressSlider.minValue;
        previewProgressSlider.maxValue = upgradeProgressSlider.maxValue;
        previewProgressSlider.value = PreviewUpgradeEXP;
    }

    private void UpdateContent()
    {
        ClearAllBtn.gameObject.SetActive(GetNoofSlotsTaken() != 0);

        if (GetItemREF() != null)
        {
            UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
            selectedItemImage.sprite = UpgradableItemREF.GetItemSprite();
            LevelDisplay.text = "+" + UpgradableItemREF.GetLevel();
            ExpAmountDisplay.text = Mathf.RoundToInt(upgradeProgressSlider.value) + "/" + upgradeProgressSlider.maxValue.ToString();

            IncreaseAmountExpDisplay.text = "+" + ((int)GetTotalEXP());
            IncreaseAmountExpDisplay.gameObject.SetActive(GetNoofSlotsTaken() != 0);

            IncreaseLevelDisplay.text = "+" + GetLevelIncrease();
            IncreaseLevelDisplay.gameObject.SetActive(GetLevelIncrease() != 0);

            MaxLevelContent.SetActive(UpgradableItemREF.GetLevel() == CostList.Length);
            EnhancementContent.SetActive(UpgradableItemREF.GetLevel() < CostList.Length);

            Artifacts selectedartifacts = UpgradableItemREF as Artifacts;
            for (int i = 0; i < ArtifactsStatsContainer.Length; i++)
            {
                DisplayArtifactStats stats = ArtifactsStatsContainer[i].GetComponent<DisplayArtifactStats>();
                if (stats != null)
                {
                    if (i <= (int)selectedartifacts.GetRarity())
                    {
                        stats.DisplayArtifactsStat(selectedartifacts.GetArtifactStatsName(i), selectedartifacts.GetStats(i), selectedartifacts.GetArtifactStatsValue(i));
                        stats.gameObject.SetActive(true);
                    }
                    else
                        stats.gameObject.SetActive(false);
                }
            }
        }
    }

    private int GetTotalSlots()
    {
        return EnhancementItemList.Count;
    }

    private int GetNoofSlotsTaken()
    {
        int total = 0;
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
                if (slot.GetItemButton().GetItemREF() != null)
                    total++;
        }
        return total;
    }

    private List<GameObject> GetEnhancementItemList()
    {
        return EnhancementItemList;
    }
    private void LoadEmptySlots()
    {
        upgradeArtifactCanvas.SlotPopup.onSlotSend -= ManualAddItems;
        for (int i = EnhancementItemList.Count - 1; i >= 0; i--)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            upgradeArtifactCanvas.SlotPopup.UnSubscribeSlot(slot);
            slot.onSlotClick -= RemoveSelfItemButton;
            slot.SlotItemButtonChanged -= OnSlotItemButtonChanged;
            Destroy(EnhancementItemList[i].gameObject);
        }

        for (int i = 0; i < EnhancementItemToSell; i++)
        {
            GameObject enhancementitemslot = Instantiate(AssetManager.GetInstance().SlotPrefab, SlotsParent);
            Slot slot = enhancementitemslot.GetComponent<Slot>();
            upgradeArtifactCanvas.SlotPopup.SubscribeSlot(slot);
            slot.onSlotClick += RemoveSelfItemButton;
            slot.SlotItemButtonChanged += OnSlotItemButtonChanged;
            EnhancementItemList.Add(enhancementitemslot);
        }
        upgradeArtifactCanvas.SlotPopup.onSlotSend += ManualAddItems;
        upgradeArtifactCanvas.SlotPopup.onSlotItemRemove += ManualRemoveItems;
    }

    private void OnSlotItemButtonChanged(SendSlotInfo sendslotInfo, Item item)
    {
        if (sendslotInfo.slot)
        {
            if (!sendslotInfo.itemButtonREF)
                return;

            ItemButton SlotPopupitemButton = upgradeArtifactCanvas.SlotPopup.GetItemButton(sendslotInfo.itemButtonREF.GetItemREF());
            for (int i = 0; i < EnhancementItemList.Count; i++)
            {
                Slot slot = EnhancementItemList[i].GetComponent<Slot>();
                AssetManager.GetInstance().UpdateCurrentSelectionOutline(slot, null);
            }

            if (!SlotPopupitemButton)
                return;

            SlotPopupitemButton.ToggleRemoveItemImage(item != null);
        }
    }

    private void RemoveSelfItemButton(Slot selectedSlot)
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            AssetManager.GetInstance().UpdateCurrentSelectionOutline(slot, null);
        }
        AssetManager.GetInstance().UpdateCurrentSelectionOutline(null, selectedSlot);

        upgradeArtifactCanvas.SlotPopup.HideItem(GetItemREF());
    }
    private void OnInventoryListChanged()
    {
        ClearAll();
        RemoveSelfItemButton(null);
    }

    private void ManualRemoveItems(object sender, SendSlotInfo e)
    {
        ItemButton ExistitemButton = CheckIfItemalreadyExist(e.itemButtonREF.GetItemREF());

        if (ExistitemButton != null)
            GetSlotAt(ExistitemButton).SetItemButton(null);
    }

    public void ClearAll()
    {
        for (int i = 0; i < EnhancementItemList.Count; i++)
        {
            Slot slot = EnhancementItemList[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
                GetSlotAt(slot.GetItemButton()).SetItemButton(null);
        }
    }

    private void ManualAddItems(object sender, SendSlotInfo e)
    {
        ItemButton ExistitemButton = CheckIfItemalreadyExist(e.itemButtonREF.GetItemREF());

        if (ExistitemButton == null)
        {
            UpgradableItems upgradableItems = e.itemButtonREF.GetItemREF() as UpgradableItems;
            if (upgradableItems != null)
            {
                if (upgradableItems.GetLockStatus())
                    return;

                if (upgradableItems is Artifacts)
                {
                    Artifacts artifacts = upgradableItems as Artifacts;
                    if (artifacts.GetCharacter())
                        return;
                }
            }

            if (FindAvailableSlot() != null)
                FindAvailableSlot().SetItemButton(e.itemButtonREF.GetItemREF());
            else
                DisplayFullError();
        }
    }
   

    private IEnumerator UpgradeProgress()
    {
        float smoothTime = 2f;
        float UpgradeElasped = 0;
        UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;

        while (GetItemREF() != null)
        {
            UpgradinginProgress = true;
            if (UpgradableItemREF.GetLevel() <= CostList.Length)
            {
                if (Mathf.Approximately(upgradeProgressSlider.value, upgradeProgressSlider.maxValue))
                {
                    if ((UpgradableItemREF.GetLevel() + 1) < CostList.Length)
                    {
                        UpgradableItemREF.SetExpAmount(UpgradableItemREF.GetExpAmount() - upgradeProgressSlider.maxValue);
                        SetProgressUpgrades(0, CostList[UpgradableItemREF.GetLevel() + 1]);
                    }
                    else
                    {
                        SetProgressUpgrades(0, 0);
                    }

                    upgradeProgressSlider.value = upgradeProgressSlider.minValue;
                    UpgradableItemREF.Upgrade();
                }
            }

            if (!Mathf.Approximately(upgradeProgressSlider.value, UpgradableItemREF.GetExpAmount()))
                upgradeProgressSlider.value = Mathf.Lerp(upgradeProgressSlider.value, UpgradableItemREF.GetExpAmount(), UpgradeElasped / smoothTime);
            else
                break;

            UpgradeElasped += Time.deltaTime;
            yield return null;
        }

        UpgradinginProgress = false;
        Debug.Log("Enhanced Completed");
    }

    private void SetProgressUpgrades(float min, float max)
    {
        upgradeProgressSlider.minValue = min;
        upgradeProgressSlider.maxValue = max;
    }

    private float GetTotalEXP()
    {
        float total = 0;

        for (int i = 0; i < GetEnhancementItemList().Count; i++)
        {
            Slot slot = GetEnhancementItemList()[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
            {
                switch(slot.GetItemButton().GetItemREF().GetRarity())
                {
                    case Rarity.ThreeStar: // hardcode for now
                        total += 500f;
                        break;
                    case Rarity.FourStar:
                        total += 1000f;
                        break;
                    case Rarity.FiveStar:
                        total += 2500f;
                        break;
                }
            }
        }

        return total;
    }
    public void EnhanceUpgrade()
    {
        if (GetItemREF() != null)
        {
            UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
            UpgradableItemREF.SetExpAmount(PreviewUpgradeEXP);
            StartCoroutine(UpgradeProgress());
        }
    }

    private void AutoAdd()
    {
        StartCoroutine(GetRandomsItemButton(CharacterManager.GetInstance().GetPlayerStats().GetINVList()));

        if (NoofItemsAdded <= 0 && FindAvailableSlot())
            AssetManager.GetInstance().OpenPopupPanel("No Consumable Found");

        DisplayFullError();
    }

    private void DisplayFullError()
    {
        if (!FindAvailableSlot())
            AssetManager.GetInstance().OpenPopupPanel("Slots Full");
    }

    private void UpgradeItem()
    {
        if (GetNoofSlotsTaken() == 0)
        {
            AssetManager.GetInstance().OpenPopupPanel("Select EXP materials");
            return;
        }

        EnhanceUpgrade();

        for (int i = 0; i < GetEnhancementItemList().Count; i++)
        {
            Slot slot = GetEnhancementItemList()[i].GetComponent<Slot>();
            if (slot.GetItemButton() != null)
            {
                if (slot.GetItemButton().GetItemREF() != null)
                {
                    CharacterManager.GetInstance().GetPlayerStats().RemoveItems(slot.GetItemButton().GetItemREF());
                    slot.SetItemButton(null);
                }
            }
        }
    }

    private IEnumerator GetRandomsItemButton(List<Item> itemlistREF)
    {
        List<Item> itemsList = new List<Item>(itemlistREF);
        itemsList.RemoveAll(item => item.GetCategory != GetItemREF().GetCategory || (int)item.GetRarity() > RaritySelection);

        NoofItemsAdded = 0;
        int slotsTaken = GetNoofSlotsTaken();

        if ((GetNoofSlotsTaken() >= GetTotalSlots()) || itemsList.Count == 0 || !FindAvailableSlot())
            yield break;

        Item[] shuffledItems = itemsList.OrderBy(item => UnityEngine.Random.value).OrderBy(item => (int)item.GetRarity()).ToArray();

        for (int i = 0; i < itemsList.Count; i++)
        {
            Item item = shuffledItems[i];

            if (GetNoofSlotsTaken() >= GetTotalSlots())
                break;

            if (CheckIfItemalreadyExist(item) != null || item == GetItemREF())
                continue;

            UpgradableItems upgradableItems = item as UpgradableItems;
            if (upgradableItems != null)
            {
                if (upgradableItems.GetLockStatus())
                    continue;

                if (upgradableItems is Artifacts)
                {
                    Artifacts artifacts = upgradableItems as Artifacts;
                    if (artifacts.GetCharacter())
                        continue;
                }
            }


            FindAvailableSlot().SetItemButton(item);
            NoofItemsAdded++;
            slotsTaken++;
            yield return null;
        }

        yield return null;
    }
}
