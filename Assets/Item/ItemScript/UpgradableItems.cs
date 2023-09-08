using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradableItems : Item
{
    protected int Level, MaxLevel;
    protected bool locked;
    protected float ExpAmount;

    public virtual void Upgrade()
    {
        Level++;
        Level = Mathf.Clamp(Level, 0, MaxLevel);
    }

    public int GetLevel()
    {
        return Level;
    }

    public float GetExpAmount()
    {
        return ExpAmount;
    }

    public void SetExpAmount(float amt)
    {
        ExpAmount = amt; 
    }
    public override string GetItemType()
    {
        return "Unknown Upgradable Item";
    }

    public UpgradableItems() : base()
    {
        MaxLevel = 20;
        Level = 0;
        ExpAmount = 0;
        locked = false;
    }

    public void SetLockStatus(bool lockstatus)
    {
        locked = lockstatus;
    }

    public bool GetLockStatus()
    {
        return locked;
    }
}
