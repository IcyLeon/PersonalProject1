using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public abstract class ConsumableItem : Item
{
    protected int amount;

    public override string GetItemType()
    {
        return "Unknown Consumable Item";
    }

    public virtual void Use(int Useamount)
    {
        if (amount <= 0)
        {
            CharacterManager.GetInstance().GetPlayerStats().RemoveItems(this);
            return;
        }

        amount = amount - Useamount;
        OverflowManager();
    }
    public int GetAmount()
    {
        return amount;
    }
    public void AddAmount(int amountAdd = 1)
    {

        amount += amountAdd;
        OverflowManager();
    }

    protected void OverflowManager()
    {
        amount = Mathf.Clamp(amount, 0, 1000);
    }

    public ConsumableItem() : base()
    {
        amount = 0;
    }
}
