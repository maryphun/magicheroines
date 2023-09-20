using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using Assets.SimpleLocalization.Scripts;

public class Inventory
{
    // Monobehaviourを継承しないシングルトン
    static Inventory instance = null;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Inventory();
            }

            return instance;
        }
    }

    private Inventory()
    {
        // load inventory object
        var origin = Resources.Load("Prefabs/Inventory");
        var gameobject = (GameObject)GameObject.Instantiate(origin);
        obj = gameobject.GetComponent<InventoryManager>();
        
        // Create Canvas
        GameObject canvas = new GameObject("InventoryCanvas(Don't Destroy)");

        canvasObj = canvas.AddComponent<Canvas>();
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        /// setting canvas
        canvasObj.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.vertexColorAlwaysGammaSpace = true;
        canvasObj.sortingOrder = 99;

        obj.transform.SetParent(canvas.transform);

        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(1320.0f, 580.0f);
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
        obj.Initialize();

        gameobject.name = "Inventory";
        GameObject.DontDestroyOnLoad(canvas);
    }

    public Canvas canvasObj = null;
    public InventoryManager obj;
}

public class InventoryManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform originItemBox;
    [SerializeField] private float animTime;

    [Header("Debug")]
    [SerializeField] private Canvas parent;
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool isOpened;
    [SerializeField] private RectTransform[] itemSlots;
    [SerializeField] private Action onCloseCallback;
    [SerializeField] private List<Tuple<RectTransform, ItemDefine>> itemsInInventory;

    // 設定
    const int ItemColumnCount = 12;
    const int ItemRowCount = 4;
    private Vector2 ItemSlotSize = new Vector2(100.0f, 100.0f);
    private Vector2 SlotGap = new Vector2(5.0f, 5.0f);
    private Vector2 MarginGap = new Vector2(0.0f, 35.0f);

    public void Initialize()
    {
        if (this.isInitialized) return;

        itemSlots = new RectTransform[ItemColumnCount * ItemRowCount];

        int index = 0;
        for (int y = ItemRowCount - 1; y >= 0; y--)
        {
            for (int x = 0; x < ItemColumnCount; x++)
            {
                var obj = Instantiate(originItemBox.gameObject, originItemBox.transform.parent);

                obj.SetActive(true);

                itemSlots[index] = obj.GetComponent<RectTransform>();
                itemSlots[index].sizeDelta = ItemSlotSize;
                float positionX = (-((ItemColumnCount-1) * (ItemSlotSize.x + SlotGap.x)) * 0.5f) + x * (ItemSlotSize.x + SlotGap.x) + MarginGap.x;
                float positionY = (-((ItemRowCount-1   ) * (ItemSlotSize.y + SlotGap.y)) * 0.5f) + y * (ItemSlotSize.y + SlotGap.y) - MarginGap.y;
                itemSlots[index].localPosition = new Vector2(positionX, positionY);
                itemSlots[index].name = "Slot [" + index.ToString() + "]";
                itemSlots[index].GetComponent<Image>().raycastTarget = false;
                index++;
            }
        }

        panel = gameObject.AddComponent<CanvasGroup>();
        panel.alpha = 0.0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;

        parent = transform.parent.GetComponent<Canvas>();

        isInitialized = true;
    }

    public void ConfigureItem()
    {
        this.itemsInInventory = new List<Tuple<RectTransform, ItemDefine>>();
        var itemList = ProgressManager.Instance.GetItemList(false);
        for (int i = 0; i < itemList.Count;i++)
        {
            var itemImg = new GameObject().AddComponent<Image>();
            string itemName = LocalizationManager.Localize(itemList[i].itemNameID);
            itemImg.name = "Item [" + itemName + "]";
            itemImg.sprite = itemList[i].Icon;
            itemImg.transform.SetParent(itemSlots[i].transform);

            var itemRect = itemImg.GetComponent<RectTransform>();
            itemRect.sizeDelta = ItemSlotSize;
            itemRect.localPosition = Vector2.zero;

            itemsInInventory.Add(new Tuple<RectTransform, ItemDefine>(itemRect, itemList[i]));
        }
    }

    public void OpenInventory(Action callbackWhenClose)
    {
        if (!isInitialized) Initialize();
        if (isOpened) return;
        ConfigureItem();

        onCloseCallback = callbackWhenClose;
        panel.DOFade(1.0f, animTime);
        panel.interactable = true;
        panel.blocksRaycasts = true;
        isOpened = true;
    }

    public void CloseInventory()
    {
        if (!isOpened) return;
        isOpened = false;

        onCloseCallback?.Invoke();
        panel.DOFade(0.0f, animTime);
        panel.interactable = false;
        panel.blocksRaycasts = false;

        // remove all item
        foreach (Tuple<RectTransform, ItemDefine> item in itemsInInventory)
        {
            Destroy(item.Item1.gameObject);
        }
        itemsInInventory.Clear();
        itemsInInventory = null;
    }

    /// <summary>
    /// 所持アイテム数の最大値を取得
    /// </summary>
    public int GetMaxItemCount()
    {
        return ItemColumnCount * ItemRowCount;
    }

    private void Update()
    {
        if (!isOpened || !isInitialized) return;

        // マウスクリックを検知
        Vector3 mousePosition = Input.mousePosition / parent.scaleFactor;

        foreach (Tuple<RectTransform, ItemDefine> item in itemsInInventory)
        {
            if (   mousePosition.x > (item.Item1.position.x - ItemSlotSize.x * 0.5f)
                && mousePosition.x < (item.Item1.position.x + ItemSlotSize.x * 0.5f)
                && mousePosition.y > (item.Item1.position.y - ItemSlotSize.y * 0.5f)
                && mousePosition.y < (item.Item1.position.y + ItemSlotSize.y * 0.5f))
            {
                // 資料表示
                /// ...
                /// ...

                // 選択・使用
                if (Input.GetMouseButtonDown(0))
                {
                    // effect
                    UseItem(item);

                    // remove this item from list
                    ProgressManager.Instance.RemoveItemFromInventory(item.Item2);
                }

                // 残りのループを省略
                return;
            }
        }
    }

    private void UseItem(Tuple<RectTransform, ItemDefine> item)
    {
        switch (item.Item2.itemType)
        {
            case ItemType.SelfCast:
                ItemExecute.Instance.Invoke(item.Item2.functionName, 0);
                CloseInventory();
                break;
            case ItemType.Teammate:

                break;
            case ItemType.Enemy:

                break;
            default:
                break;
        }
    }
}
