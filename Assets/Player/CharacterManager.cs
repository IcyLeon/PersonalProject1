using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private static CharacterManager instance;
    private PlayerController playerController;
    [SerializeField] List<Characters> characters = new List<Characters>();

    private void Awake()
    {
        instance = this;
    }

    public PlayerStats GetPlayerStats()
    {
        return playerController.GetInventory().GetPlayerStats();

    }
    public Characters GetCurrentCharacter()
    {
        return playerController.GetCharacterRB().GetComponent<Characters>();
    }

    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public List<Characters> GetEquippedCharacter()
    {
        return playerController.GetInventory().GetEquipCharactersList();
    }
    public List<Characters> GetCharacterList()
    {
        return characters;
    }

    public static CharacterManager GetInstance()
    {
        return instance;
    }
}
