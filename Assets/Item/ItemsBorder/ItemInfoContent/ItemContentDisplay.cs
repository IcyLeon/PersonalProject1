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
    [SerializeField] GameObject[] ArtifactsStatsContainer;
    [SerializeField] Image[] StarDisplayList;


    void ShowArtifactsItemContent(Artifacts artifacts)
    {
        if (artifacts == null)
            return;

        ArtifactNameText.text = artifacts.GetItemName();
        ArtifactPieceText.text = artifacts.GetItemType();
        ArtifactSetText.text = artifacts.artifactsInfo.ArtifactsSetName + ":";
        Artifact2PieceText.text = "2-Piece Set: " + artifacts.Get2PieceType();
        Artifact4PieceText.text = "4-Piece Set: " + artifacts.Get4PieceType();
        ArtifactLevelText.text = "+" + artifacts.GetLevel().ToString();
        ArtifactDescText.text = artifacts.GetItemDesc();

        for (int i = 0; i < ArtifactsStatsContainer.Length; i++)
        {
            DisplayArtifactStats stats = ArtifactsStatsContainer[i].GetComponent<DisplayArtifactStats>();
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
    public void RefreshItemContentDisplay(Item SelectedItem)
    {
        if (SelectedItem == null)
            return;

        UpgradableItems UpgradableItemREF = SelectedItem as UpgradableItems;
        ShowArtifactsItemContent((Artifacts)UpgradableItemREF);



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
