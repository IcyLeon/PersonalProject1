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
using static PlayerStats;

public class EnhancementManager : MonoBehaviour
{
    [Header("Enhancement Infomation")]
    [SerializeField] int EnhancementItemToSell;
    [SerializeField] float ScoreUpgradepts;
    [SerializeField] UpgradeArtifactCanvas upgradeArtifactCanvas;
    [SerializeField] Image selectedItemImage;
    [SerializeField] TextMeshProUGUI LevelDisplay, ExpAmountDisplay;
    [SerializeField] Button AutoAddBtn;
    [SerializeField] Button UpgradeBtn;
    [SerializeField] Button ClearAllBtn;
    [SerializeField] Slider upgradeprogressslider;
    [SerializeField] TMP_Dropdown AutoAddSelection;
    [SerializeField] ItemsList itemList;
    [SerializeField] Transform SlotsParent;
    List<GameObject> EnhancementItemList = new List<GameObject>();
    private int NoofItemsAdded;
    private int RaritySelection;

    [Header("Artifacts Stats")]
    [SerializeField] GameObject[] ArtifactsStatsContainer;
    private float UpgradeElasped;
    private float smoothDampVelocity;

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


        int[] CostList = itemList.GetCostListStatus(GetItemREF().GetRarity());
        SetProgressUpgrades(0, CostList[0]);
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

    private void UpdateContent()
    {
        ClearAllBtn.gameObject.SetActive(GetNoofSlotsTaken() != 0);

        if (GetItemREF() != null)
        {
            UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
            selectedItemImage.sprite = UpgradableItemREF.GetItemSprite();
            LevelDisplay.text = "+" + UpgradableItemREF.GetLevel().ToString();
            ExpAmountDisplay.text = ((int)upgradeprogressslider.value).ToString() + "/" + upgradeprogressslider.maxValue.ToString();

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
            ItemButton SlotPopupitemButton = upgradeArtifactCanvas.SlotPopup.GetItemButton(sendslotInfo.itemButtonREF.GetItemREF());

            if (!SlotPopupitemButton)
                return;

            SlotPopupitemButton.ToggleRemoveItemImage(item != null);
        }
    }

    private void RemoveSelfItemButton(Slot slot) // remove the item that we want to upgrade from the Popup UI, slot is a dummy code
    {
        upgradeArtifactCanvas.SlotPopup.HideItem(GetItemREF());
    }
    private void OnInventoryListChanged()
    {
        RemoveSelfItemButton(null);
    }

    private void ManualRemoveItems(object sender, SendSlotInfo e)
    {
        ItemButton ExistitemButton = CheckIfItemalreadyExist(e.itemButtonREF.GetItemREF());
        upgradeArtifactCanvas.SlotPopup.GetItemInfoContent().TogglePopup(false);

        if (ExistitemButton != null)
            GetSlotAt(ExistitemButton).SetItemButton(null);
    }

    private void ClearAll()
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
        upgradeArtifactCanvas.SlotPopup.GetItemInfoContent().TogglePopup(true);
        upgradeArtifactCanvas.SlotPopup.GetItemInfoContent().SetItemButtonREF(e.itemButtonREF);

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
   

    IEnumerator UpgradeProgress()
    {
        UpgradableItems UpgradableItemREF = GetItemREF() as UpgradableItems;
        int[] CostList = itemList.GetCostListStatus(GetItemREF().GetRarity());

        while (GetItemREF() != null)
        {
            if (UpgradableItemREF.GetLevel() < CostList.Length)
            {
                float threshold = 1f;
                if (Mathf.Abs(upgradeprogressslider.value - upgradeprogressslider.maxValue) < threshold)
                {
                    if ((UpgradableItemREF.GetLevel() + 1) < CostList.Length)
                    {
                        SetProgressUpgrades(CostList[UpgradableItemREF.GetLevel()], CostList[UpgradableItemREF.GetLevel() + 1]);
                        //upgradeprogressslider.value = upgradeprogressslider.minValue;
                    }
                    UpgradableItemREF.Upgrade();
                }
            }

            float smoothTime = 0.2f;
            float maxSpeed = Mathf.Infinity;
            upgradeprogressslider.value = Mathf.SmoothDamp(upgradeprogressslider.value, ScoreUpgradepts, ref smoothDampVelocity, smoothTime, maxSpeed, Time.deltaTime);
            yield return null;
        }
    }

    void SetProgressUpgrades(float min, float max)
    {
        upgradeprogressslider.minValue = min;
        upgradeprogressslider.maxValue = max;
    }

    public void EnhanceUpgrade()
    {
        ScoreUpgradepts += 500;
        UpgradeElasped = 0;
        StartCoroutine(UpgradeProgress());
    }

    private void AutoAdd()
    {
        StartCoroutine(GetRandomItemButton(CharacterManager.GetInstance().GetPlayerStats().GetINVList()));

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

    private IEnumerator GetRandomItemButton(List<Item> itemlistREF)
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
