using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeCanvasTransition : MonoBehaviour, IPointerClickHandler
{
    private ItemButton ItemButtonREF;
    [SerializeField] GameObject UpgradeArtifactCanvas;
    [SerializeField] GameObject ItemShowcaseCanvas;

    public void OnPointerClick(PointerEventData eventData)
    {
        SpawnUI();
    }

    void SpawnUI()
    {
        if (ItemButtonREF == null)
            return;
        else
        {
            if (ItemButtonREF.GetItemREF() is not UpgradableItems)
                return;
        }

        UpgradeArtifactCanvas.GetComponent<UpgradeArtifactCanvas>().OpenUpgradeItemCanvas(ItemButtonREF);
        ItemShowcaseCanvas.SetActive(false);
    }

    public void SetItemButtonREF(ItemButton itembutton)
    {
        ItemButtonREF = itembutton;
    }
}
