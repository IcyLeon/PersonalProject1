using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToggle
{
    void ToggleSelection(bool toggle);
}

public class AssetManager : MonoBehaviour
{
    [SerializeField] MessagePanel InfomationPanel;
    [SerializeField] PopupPanel PopupPanel;
    private GameObject DraggingItem;
    public GameObject ItemBorderPrefab;
    public GameObject SlotPrefab;
    public Canvas canvas;

    private static AssetManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    public GameObject GetDragItem()
    {
        return DraggingItem;
    }
    public void SetDragItem(GameObject go)
    {
        DraggingItem = go;
    }
    public static AssetManager GetInstance()
    {
        return instance;
    }

    public void OpenPopupPanel(string text)
    {
        PopupPanel.SetMessage(text);
    }
    public void OpenMessagePanel(string headtext, string bodytext)
    {
        //PopupPanel.SetMessage(text);
    }

    public void UpdateCurrentSelectionOutline(IToggle prev, IToggle current)
    {
        if (prev != null)
        {
            prev.ToggleSelection(false);
        }
        if (current != null)
        {
            current.ToggleSelection(true);
        }
    }

}
