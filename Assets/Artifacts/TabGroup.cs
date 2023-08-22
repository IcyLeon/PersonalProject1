using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    List<TabButton> tabs = new List<TabButton>();
    public event EventHandler onTabChanged;

    [Serializable]
    public struct TabMenu
    {
        public GameObject TabPanel;
        public ArtifactsListInfo.ArtifactType ArtifactType;
    }
    [SerializeField] TabMenu[] TabMenuList;
    private TabButton selectedtab;
    private TabMenu currentPanel;

    public TabMenu[] GetTabMenuList()
    {
        return TabMenuList;
    }

    public TabMenu GetCurrentTabPanel()
    {
        return currentPanel;
    }

    public int GetTabPanelIdx(ArtifactsListInfo.ArtifactType ArtifactType)
    {
        for (int i = 0; i < TabMenuList.Length; i++)
        {
            if (TabMenuList[i].ArtifactType == ArtifactType)
            {
                return i;
            }
        }
        return -1;
    }

    private void OpenPanel()
    {
        for (int i = 0; i < TabMenuList.Length; i++)
        {
            TabMenuList[i].TabPanel.SetActive(false);
        }
        currentPanel = TabMenuList[GetTabPanelIdx(selectedtab.artifactType)];
        currentPanel.TabPanel.SetActive(true);
    }

    void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].artifactType == ArtifactsListInfo.ArtifactType.FLOWER)
                OnTabSelected(tabs[i]);
        }
    }

    public TabButton GetTabSelected()
    {
        return selectedtab;
    }

    public void Subscribe(TabButton tb)
    {
        tabs.Add(tb);
    }

    public void OnTabSelected(TabButton tb)
    {
        OnTabReset();
        selectedtab = tb;
        selectedtab.SelectedTabIcons();
        OpenPanel();
        onTabChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnTabReset()
    {
        foreach(TabButton tb in tabs)
        {
            tb.GetComponent<Image>().sprite = tb.tabButtonImage;
            tb.ResetTabIcons();
        }
    }
}
