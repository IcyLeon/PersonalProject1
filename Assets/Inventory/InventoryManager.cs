using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{
    private PlayerStats PlayerStats;
    private List<Characters> equipcharacterslist;
    private Characters currentequipCharacter;

    public InventoryManager()
    {
        PlayerStats = new PlayerStats();
        equipcharacterslist = new List<Characters>();
    }

    public PlayerStats GetPlayerStats()
    {
        return PlayerStats;
    }

    public List<Characters> GetEquipCharactersList()
    {
        return equipcharacterslist;
    }

    public Characters GetCurrentEquipCharacter()
    {
        return currentequipCharacter;
    }

    public void SetCurrentEquipCharacter(Characters characters)
    {
        currentequipCharacter = characters;
    }
}
