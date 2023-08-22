using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemContentDisplay : MonoBehaviour
{
    [Header("Artifacts Content")]
    [SerializeField] TextMeshProUGUI ArtifactNameText;
    [SerializeField] TextMeshProUGUI ArtifactPieceText;
    [SerializeField] TextMeshProUGUI ArtifactSetText;
    [SerializeField] TextMeshProUGUI ArtifactLevelText;
    [SerializeField] TextMeshProUGUI ArtifactDescText;
    [SerializeField] TextMeshProUGUI Artifact2PieceText;
    [SerializeField] TextMeshProUGUI Artifact4PieceText;
    private DisplayArtifactStats[] ArtifactsStatsContainer;
    private Image[] StarDisplayList;
    [SerializeField] GameObject DetailsPanel;
    [SerializeField] GameObject RarityStarGO;

    void Awake()
    {
        if (DetailsPanel)
            ArtifactsStatsContainer = DetailsPanel.GetComponentsInChildren<DisplayArtifactStats>();

        if (RarityStarGO)
            StarDisplayList = RarityStarGO.GetComponentsInChildren<Image>();
    }

    void ShowArtifactsItemContent(Artifacts artifacts)
    {
        if (artifacts == null)
            return;

        ArtifactNameText.text = artifacts.GetItemName();
        ArtifactPieceText.text = artifacts.GetItemType();
        ArtifactSetText.text = artifacts.artifactsInfo.ArtifactsSetName + ":";
        Artifact2PieceText.text = artifacts.Get2PieceType();
        Artifact4PieceText.text = artifacts.Get4PieceType();
        ArtifactLevelText.text = "+" + artifacts.GetLevel().ToString();
        ArtifactDescText.text = artifacts.GetItemDesc();

        if (ArtifactsStatsContainer != null)
        {
            for (int i = 0; i < ArtifactsStatsContainer.Length; i++)
            {
                DisplayArtifactStats stats = ArtifactsStatsContainer[i];
                if (stats != null)
                {
                    if (i <= (int)artifacts.GetRarity())
                    {
                        stats.DisplayArtifactsStat(artifacts.GetArtifactStatsName(i), artifacts.GetStats(i), artifacts.GetArtifactStatsValue(i));
                        stats.gameObject.SetActive(true);
                    }
                    else
                        stats.gameObject.SetActive(false);
                }
            }
        }
    }
    public void RefreshItemContentDisplay(Item SelectedItem)
    {
        if (SelectedItem == null)
            return;

        UpgradableItems UpgradableItemREF = SelectedItem as UpgradableItems;

        switch (SelectedItem.GetCategory)
        {
            case Category.ARTIFACTS:
                ShowArtifactsItemContent((Artifacts)UpgradableItemREF);
                break;
        }

        if (RarityStarGO)
        {
            for (int i = 0; i < StarDisplayList.Length; i++)
            {
                if (i <= (int)SelectedItem.GetRarity())
                {
                    StarDisplayList[i].gameObject.SetActive(true);
                }
                else
                {
                    StarDisplayList[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
