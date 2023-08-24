using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeArtifactCanvas : MonoBehaviour
{
    [Header("Tab Toggle")]
    [SerializeField] ToggleGroup TabToggleGroup;
    [SerializeField] GameObject[] TabContent;
    private Toggle[] TabToggleGroupList;

    [Header("Upgradable Items")]
    [SerializeField] Image UpgradeItemsIcon;
    [SerializeField] TextMeshProUGUI UpgradeItemsType;
    [SerializeField] SlotPopup slotPopup;
    [SerializeField] ItemContentManager ItemContentManager;
    private ItemButton ItemButtonREF;
    private Item itemREF;

    public Item GetItemREF()
    {
        return itemREF;
    }
    public SlotPopup SlotPopup
    {
        get { return slotPopup; }
    }

    private void Awake()
    {
        TabToggleGroupList = TabToggleGroup.GetComponentsInChildren<Toggle>();
        foreach (var tabToggle in TabToggleGroupList)
        {
            int index = ArrayUtility.IndexOf(TabToggleGroupList, tabToggle);
            tabToggle.onValueChanged.AddListener(value => ToggleDetails(index));
        }
    }

    void ToggleDetails(int idx)
    {
        TabContent[idx].gameObject.SetActive(TabToggleGroupList[idx].isOn);
    }

    // Update is called once per frame
    public void OpenUpgradeItemCanvas(ItemButton ItemButton)
    {
        ItemButtonREF = ItemButton;
        itemREF = ItemButtonREF.GetItemREF();

        if (GetItemREF() == null)
            return;
        else if (GetItemREF() is not UpgradableItems)
            return;

        gameObject.SetActive(true);
        UpgradeItemsType.text = GetItemREF().GetItemType() + " / " + GetItemREF().GetItemName();
        itemREF = ItemButtonREF.GetItemREF();
        ItemContentManager.SetItemButtonREF(ItemButtonREF);
        slotPopup.OnInventoryListChanged();
    }
}