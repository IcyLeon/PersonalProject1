using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStats
{
    private int Load;
    private int MaxLoad;
    private int mora;
    List<Item> InventoryList;

    public event Action onInventoryListChanged;

    public int Mora
    {
        get { return mora; }
        set { mora = value; }
    }

    public List<Item> GetINVList()
    {
        return InventoryList;
    }

    public void CallRefreshInventory()
    {
        onInventoryListChanged?.Invoke();
    }

    public void AddItems(Item item)
    {
        InventoryList.Add(item);
        CallRefreshInventory();
    }

    public void RemoveItems(Item item)
    {
        InventoryList.Remove(item);
        CallRefreshInventory();
    }

    public PlayerStats()
    {
        Load = 0;
        MaxLoad = 10000;
        mora = 0;
        InventoryList = new List<Item>();
    }
}
