using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler
{
    private Color ImageColor;
    [Header("Get Item Reference")]
    private Item itemREF;
    private Sprite itemSprite;

    public delegate void SendItemButtonInfo(ItemButton itemButton);
    public event SendItemButtonInfo onButtonClick;
    public event SendItemButtonInfo onButtonRemoveClick;
    public event SendItemButtonInfo onButtonSpawn;

    [SerializeField] Image Outline;
    [SerializeField] Button RemoveImage;
    [SerializeField] Image ItemImage;
    [SerializeField] Image NewImage;
    [SerializeField] LockItem lockItem;
    [SerializeField] CanvasGroup canvasGroup;

    private RectTransform ItemButton_Rect;
    private ItemButton ItemButton_Drag;

    public RectTransform GetItemButtonRect()
    {
        return ItemButton_Rect;
    }

    public CanvasGroup CanvasGroup
    {
        get { return canvasGroup; }
    }

    public Sprite GetItemImage
    {
        get { return itemSprite; }
    }

    private void Awake()
    {
        ItemButton_Rect = GetComponent<RectTransform>();
        RemoveImage.onClick.AddListener(RemoveItemEvent);
    }
    private void RemoveItemEvent()
    {
        onButtonRemoveClick.Invoke(this);
    }

    private void CopyItemButton(Transform parentTransform)
    {
        if (parentTransform == null)
            parentTransform = AssetManager.GetInstance().canvas.transform;

        GameObject go = Instantiate(AssetManager.GetInstance().ItemBorderPrefab, parentTransform, false);
        go.transform.SetAsLastSibling();
        ItemButton_Drag = go.GetComponent<ItemButton>();
        ItemButton_Drag.CanvasGroup.blocksRaycasts = false;
        ItemButton_Drag.SetItemREF(ItemButton_Rect.GetComponent<ItemButton>().GetItemREF());
        ItemButton_Drag.GetItemREF().isNew = false;
        AssetManager.GetInstance().SetDragItem(ItemButton_Drag.transform.gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData, Transform parentTransform)
    {
        ItemButton itemButton = eventData.pointerDrag.GetComponent<ItemButton>();
        if (itemButton == null)
            return;

        CopyItemButton(parentTransform);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (ItemButton_Drag == null)
            return;

        ItemButton_Drag.GetItemButtonRect().anchoredPosition = eventData.position - (AssetManager.GetInstance().canvas.renderingDisplaySize / 2f);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(ItemButton_Drag.gameObject);
        ItemButton_Drag = null;
    }


    // Update is called once per frame
    void Start()
    {
        SetButtonSprites();
        onButtonSpawn?.Invoke(this);

        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y , 0);
    }

    private void Update()
    {
        UpdateHideLock();
    }

    public void ToggleRemoveItemImage(bool input)
    {
        RemoveImage.gameObject.SetActive(input);
    }

    private void UpdateHideLock()
    {
        if (itemREF is UpgradableItems)
        {
            UpgradableItems upgradableItems = (UpgradableItems)itemREF;
            if (!upgradableItems.GetLockStatus())
                lockItem.gameObject.SetActive(false);
            else
            {
                lockItem.gameObject.SetActive(true);
                lockItem.SetItemREF(itemREF);
            }

            return;
        }

        lockItem.gameObject.SetActive(false);
    }
    public void SetButtonSprites()
    {
        if (itemREF != null)
        {
            itemSprite = itemREF.GetItemSprite();

            if (itemREF is Artifacts)
            {
                Artifacts artifacts = itemREF as Artifacts;
                itemSprite = ArtifactsManager.instance.GetArtifactPiece(artifacts.type, artifacts.artifactsInfo).ArtifactImage;
            }

            if (itemREF.isNew)
            {
                NewImage.gameObject.SetActive(true);
            }

            ItemImage.sprite = itemSprite;
            ItemImage.enabled = true;
        }
    }

    public void DisableNewImage()
    {
        NewImage.gameObject.SetActive(false);
    }

    public void ToggleSelection(bool toggle)
    {
        Outline.gameObject.SetActive(toggle);
    }
    public void SetItemREF(Item item)
    {
        itemREF = item;
    }

    public Item GetItemREF()
    {
        return itemREF;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemREF != null)
        {
            if (itemREF.isNew)
            {
                itemREF.isNew = false;
                DisableNewImage();
            }
        }

        onButtonClick?.Invoke(this);

        // MainImage.color = ImageColor;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
       // MainImage.color = ImageColor;
    }

    private void OnDestroy()
    {
        DragnDrop dragnDrop = GetComponent<DragnDrop>();
        dragnDrop.onBeginDragEvent -= OnBeginDrag;
        dragnDrop.onDragEvent -= OnDrag;
        dragnDrop.onEndDragEvent -= OnEndDrag;
        AssetManager.GetInstance().SetDragItem(null);
    }
}
