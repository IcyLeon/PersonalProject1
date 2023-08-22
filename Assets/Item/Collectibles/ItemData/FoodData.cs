using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "ScriptableObjects/Food")]
public class FoodData : ScriptableObject
{
    public ItemTemplate itemTemplates;
    public float Heal;
}