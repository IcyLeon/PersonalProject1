using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts", menuName = "ScriptableObjects/Artifacts")]
public class ArtifactsInfo : ScriptableObject
{
    [System.Serializable]
    public class ArtifactPiece
    {
        public ArtifactsListInfo.ArtifactType artifactType;
        public string artifactName;
        public Sprite ArtifactImage;
        [TextAreaAttribute]
        public string artifactDesc;
    }

    public ArtifactsListInfo.ArtifactsSet ArtifactSet;
    public string ArtifactsSetName;
    public ArtifactPiece[] artifactPiece;
    [TextAreaAttribute]
    public string TwoPieceDesc, FourPieceDesc;
}
