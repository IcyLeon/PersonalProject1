using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactsManager : MonoBehaviour
{
    private static ArtifactsManager Singleton;
    public static ArtifactsManager instance
    {
        get { return Singleton; }
    }
    [SerializeField] ArtifactsListInfo artifactsListInfo;

    public ArtifactsListInfo GetArtifactsListInfo()
    {
        return artifactsListInfo;
    }

    private void Awake()
    {
        Singleton = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AddArtifactsToInventory(ArtifactsListInfo.ArtifactType.FLOWER, ArtifactsListInfo.ArtifactsSet.NOBLESSE_OBLIGE, Rarity.FiveStar);
            AddArtifactsToInventory(ArtifactsListInfo.ArtifactType.CIRCLET, ArtifactsListInfo.ArtifactsSet.NOBLESSE_OBLIGE, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactsListInfo.ArtifactType.GOBLET, ArtifactsListInfo.ArtifactsSet.NOBLESSE_OBLIGE, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactsListInfo.ArtifactType.FLOWER, ArtifactsListInfo.ArtifactsSet.NOBLESSE_OBLIGE, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactsListInfo.ArtifactType.PLUME, ArtifactsListInfo.ArtifactsSet.THUNDERING_FURY, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactsListInfo.ArtifactType.FLOWER, ArtifactsListInfo.ArtifactsSet.THUNDERING_FURY, Rarity.FourStar);
            AddArtifactsToInventory(ArtifactsListInfo.ArtifactType.SANDS, ArtifactsListInfo.ArtifactsSet.THUNDERING_FURY, Rarity.ThreeStar);
            AddArtifactsToInventory(ArtifactsListInfo.ArtifactType.GOBLET, ArtifactsListInfo.ArtifactsSet.THUNDERING_FURY, Rarity.ThreeStar);
        }

    }



    private Artifacts AddArtifactsToInventory(ArtifactsListInfo.ArtifactType type, ArtifactsListInfo.ArtifactsSet artifactsset, Rarity rarity)
    {
        ArtifactsInfo artifactsinfo = GetArtifactsInfo(artifactsset);

        if (artifactsinfo != null)
        {
            Artifacts artifacts = new Artifacts(artifactsinfo, type, rarity);
            artifacts.GenerateArtifactStats();
            CharacterManager.GetInstance().GetPlayerStats().AddItems(artifacts);
            return artifacts;
        }
        return null;
    }

    private ArtifactsInfo GetArtifactsInfo(ArtifactsListInfo.ArtifactsSet artifactsset)
    {
        for (int i = 0; i < artifactsListInfo.artifactsInfo.Length; i++)
        {
            if (artifactsListInfo.artifactsInfo[i].ArtifactSet == artifactsset)
            {
                return artifactsListInfo.artifactsInfo[i];
            }
        }
        return null;
    }

    public ArtifactsInfo.ArtifactPiece GetArtifactPiece(ArtifactsListInfo.ArtifactType ArtifactType, ArtifactsInfo artifactsInfo)
    {
        for (int i = 0; i < artifactsListInfo.artifactsInfo.Length; i++)
        {
            for (int j = 0; j < artifactsListInfo.artifactsInfo[i].artifactPiece.Length; j++)
            {
                if (artifactsInfo == artifactsListInfo.artifactsInfo[i])
                {
                    if (ArtifactType == artifactsListInfo.artifactsInfo[i].artifactPiece[j].artifactType)
                    {
                        return artifactsListInfo.artifactsInfo[i].artifactPiece[j];
                    }
                }
            }
        }
        return null;
    }

    public string GetArtifactPieceName(ArtifactsListInfo.ArtifactType ArtifactType)
    {
        for (int j = 0; j < artifactsListInfo.artifactsInfoTypeName.Length; j++)
        {
            if (ArtifactType == artifactsListInfo.artifactsInfoTypeName[j].artifactType)
            {
                return artifactsListInfo.artifactsInfoTypeName[j].artifactPieceName;
            }
        }
        return null;
    }
}
