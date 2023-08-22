using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItemData", menuName = "ScriptableObjects/ConsumableItem")]
public class ItemTemplate : ScriptableObject
{
    public string ItemName;
    public string ItemDesc;
    public Sprite ItemSprite;
    public Rarity Rarity;
}
