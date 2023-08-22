using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactsData", menuName = "ScriptableObjects/ArtifactsData")]
public class ArtifactsListInfo : ScriptableObject
{
    public enum ArtifactsSet
    {
        THUNDERING_FURY,
        NOBLESSE_OBLIGE
    }

    public enum ArtifactType
    {
        FLOWER,
        PLUME,
        SANDS,
        GOBLET,
        CIRCLET
    }

    [System.Serializable]
    public class CommonArtifactPiece
    {
        public ArtifactType artifactType;
        public string artifactPieceName;
    }

    [Header("Artifacts Set Information")]
    public ArtifactsInfo[] artifactsInfo;

    [Header("Artifacts Piece Name")]
    public CommonArtifactPiece[] artifactsInfoTypeName;
}
