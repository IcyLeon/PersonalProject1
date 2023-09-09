using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Category
{
    FOOD,
    ARTIFACTS
}

public abstract class Item {
    private bool FirstCreate;

    public bool isNew
    {
        get { return FirstCreate; }
        set { FirstCreate = value; }
    }
    protected Category category;
    protected ItemTemplate itemScriptableObject;
    public Category GetCategory
    {
        get { return category; }
        set { category = value; }
    }

    protected ItemTemplate item
    {
        get { return itemScriptableObject; }
        set { itemScriptableObject = value; }
    }

    public virtual string GetItemType()
    {
        return "Unknown Type";
    }
    public virtual string GetItemName()
    {
        return item.ItemName;
    }
    public virtual Sprite GetItemSprite()
    {
        return item.ItemSprite;
    }
    public virtual string GetItemDesc()
    {
        return item.ItemDesc;
    }

    public virtual Rarity GetRarity()
    {
        return item.Rarity;
    }

    public Item()
    {
        FirstCreate = true;
    }
}
