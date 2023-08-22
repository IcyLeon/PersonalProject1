using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] ArtifactsListInfo.ArtifactType ArtifactType;

    private TabGroup tabGroup;
    private Image tabIcon;
    [SerializeField] Sprite TabButtonImage;

    public ArtifactsListInfo.ArtifactType artifactType
    {
        get { return ArtifactType;  }
    }
    public Sprite tabButtonImage
    {
        get { return TabButtonImage; }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    // Start is called before the first frame update
    void Awake()
    {
        tabGroup = transform.parent.GetComponent<TabGroup>();
        tabIcon = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void ResetTabIcons()
    {
        var tempColor = tabIcon.color;
        tempColor.a = 0.2f;
        tabIcon.color = tempColor;
    }
    public void SelectedTabIcons()
    {
        var tempColor = tabIcon.color;
        tempColor.a = 1.0f;
        tabIcon.color = tempColor;
    }
}
