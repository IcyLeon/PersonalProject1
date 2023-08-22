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
    [SerializeField] ItemsList itemList;
    [SerializeField] float ScoreUpgradepts;
    [SerializeField] Image selectedItemImage;
    [SerializeField] Slider upgradeprogressslider;
    [SerializeField] TextMeshProUGUI LevelDisplay, ExpAmountDisplay;
    [SerializeField] Image UpgradeItemsIcon;
    [SerializeField] TextMeshProUGUI UpgradeItemsType;
    [SerializeField] SlotPopup slotPopup;
    [SerializeField] ItemContentManager ItemContentManager;

    [Header("Artifacts Stats")]
    DisplayArtifactStats[] ArtifactsStatsContainer;
    private float UpgradeElasped;
    private ItemButton ItemButtonREF;
    private float smoothDampVelocity;

    public Item GetItemREF()
    {
        return ItemButtonREF.GetItemREF();
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

    // Start is called before the first frame update
    void Start()
    {
        int[] CostList = itemList.GetCostListStatus(ItemButtonREF.GetItemREF().GetRarity());
        ArtifactsStatsContainer = GetComponentsInChildren<DisplayArtifactStats>();
        SetProgressUpgrades(0, CostList[0]);
    }


    // Update is called once per frame
    public void OpenUpgradeItemCanvas(ItemButton ItemButton)
    {
        ItemButtonREF = ItemButton;

        if (GetItemREF() == null)
            return;
        else if (GetItemREF() is not UpgradableItems)
            return;

        gameObject.SetActive(true);
        UpgradeItemsType.text = GetItemREF().GetItemType() + " / " + GetItemREF().GetItemName();
        ItemContentManager.SetItemButtonREF(ItemButtonREF);
        slotPopup.OnInventoryListChanged();
    }

    IEnumerator UpgradeProgress()
    {
        UpgradableItems UpgradableItemREF = ItemButtonREF.GetItemREF() as UpgradableItems;
        int[] CostList = itemList.GetCostListStatus(ItemButtonREF.GetItemREF().GetRarity());

        while (ItemButtonREF.GetItemREF() != null)
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

    private void Update()
    {
        UpgradableItems UpgradableItemREF = ItemButtonREF.GetItemREF() as UpgradableItems;
        selectedItemImage.sprite = ItemButtonREF.GetItemREF().GetItemSprite();

        LevelDisplay.text = "+" + UpgradableItemREF.GetLevel().ToString();
        ExpAmountDisplay.text = ((int)upgradeprogressslider.value).ToString() + "/" + upgradeprogressslider.maxValue.ToString();

        Artifacts selectedartifacts = ItemButtonREF.GetItemREF() as Artifacts;
        for (int i = 0; i < ArtifactsStatsContainer.Length; i++)
        {
            DisplayArtifactStats stats = ArtifactsStatsContainer[i];
            if (stats != null)
            {
                if (i <= (int)ItemButtonREF.GetItemREF().GetRarity())
                {
                    stats.DisplayArtifactsStat(selectedartifacts.GetArtifactStatsName(i), selectedartifacts.GetStats(i), selectedartifacts.GetArtifactStatsValue(i));
                    stats.gameObject.SetActive(true);
                }
                else
                    stats.gameObject.SetActive(false);
            }
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
}