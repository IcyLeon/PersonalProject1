using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PlayerStats;

public class EnhancementManager : MonoBehaviour
{
    [Header("Enhancement Infomation")]
    [SerializeField] int EnhancementItemToSell;
    [SerializeField] UpgradeArtifactCanvas upgradeArtifactCanvas;
    [SerializeField] Button AutoAddBtn;
    [SerializeField] Button UpgradeBtn;
    [SerializeField] TMP_Dropdown AutoAddSelection;
    List<GameObject> EnhancementItemList = new List<GameObject>();
    private int NoofItemsAdded;
    private int RaritySelection;

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
            GameObject enhancementitemslot = Instantiate(AssetManager.GetInstance().SlotPrefab);
            enhancementitemslot.transform.SetParent(transform.GetChild(0));
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
    
    // Start is called before the first frame update
    void Start()
    {
        CharacterManager.GetInstance().GetPlayerStats().onInventoryListChanged += OnInventoryListChanged;
        OnInventoryListChanged();

        LoadEmptySlots();
        AutoAddBtn.onClick.AddListener(AutoAdd);
        UpgradeBtn.onClick.AddListener(UpgradeItem);
        AutoAddSelection.onValueChanged.AddListener(delegate
            {
                RaritySelection = AutoAddSelection.value;
            }
        );

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

        upgradeArtifactCanvas.EnhanceUpgrade();

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
