using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*开始拖拽：OnBeginDrag
正在拖动：OnDrag
停止拖动：OnEndDrag
与指针（鼠标/触摸）事件关联的事件有效负载:PointerEventData*/
public class UIInventorySlot : MonoBehaviour,IBeginDragHandler,IDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    private Camera mainCamera;//可以将屏幕坐标转换为世界坐标
    private Canvas parentCanvas;

    private Cursor cursor;
    private GridCursor gridCursor;

    private Transform parentItem;
    public GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIINventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextPrefabs = null;
    [HideInInspector] public bool isSelected = false;//判断这个是否为选择
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        textMeshProUGUI.raycastTarget = false;
        inventorySlotHighlight.raycastTarget = false;
    }

    private void OnEnable()
    {
        EventHander.AfterSceneLoadEvent += SceneLoaded;
        EventHander.DropSelectItemEvent += DropSelectedItemAtMousePosition;
        EventHander.RemoveSelectedItemFromInventoryEvent += RemoveSelectedItemFromInventory;
    }

    private void OnDisable()
    {
        EventHander.AfterSceneLoadEvent -= SceneLoaded;
        EventHander.DropSelectItemEvent -= DropSelectedItemAtMousePosition;
        EventHander.RemoveSelectedItemFromInventoryEvent -= RemoveSelectedItemFromInventory;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        cursor = FindObjectOfType<Cursor>();
        gridCursor = FindObjectOfType<GridCursor>();
    }

    private void ClearCursors()
    {
        //disable cursor
        gridCursor.DisableCursor();
        cursor.DisableCursor();

        //set item type to none
        gridCursor.SelectItemType = ItemType.none;
        cursor.SelectedItemType = ItemType.none;
    }


    public void ClearSelectedItem()//使本UISlot变为非选择状态
    {
        //清除光标
        ClearCursors();

        //clear currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        //set no item selected in inventory
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);//在Inventorymanager中清除选中物体

        Player.Instance.ClearCarriedItem();
    }

    private void SetSelectedItem()
    {
        //clear currently highlighTed items
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = true;

        inventoryBar.SetHighlightedInventorySlots();

        //设置物品使用范围
        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;
        cursor.ItemUseRadius = itemDetails.itemUseRadius;

        //如果项目需要网格光标，则启用光标
        if (itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }

        if (itemDetails.itemUseRadius > 0f)
        {
            cursor.EnableCursor();
        }
        else
        {
            cursor.DisableCursor();
        }

        //set item selected in inventory
        gridCursor.SelectItemType = itemDetails.itemType;
        cursor.SelectedItemType = itemDetails.itemType;

        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        if (itemDetails.canBeCarried == true)
        {
            //展示玩家正在携带物品
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);

        }
        else
        {
            //展示玩家什么都没有携带
            Player.Instance.ClearCarriedItem();
        }
    }

    /// <summary>
    /// 如果在当前鼠标位置选择，则删除项目由 drogitem 事件调用
    /// </summary>
    /// <param name="eventData"></param>
    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null && isSelected)
        {
            if (gridCursor.CursorPositionIsVaild)
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

                //如果可以把物品放在这里
                Vector3Int gridPosition = GridPropertyManager.Instance.grid.WorldToCell(worldPosition);
                GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);
                //在鼠标位置从预制件创建项目
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x,worldPosition.y-Settings.gridCellSize/2f,worldPosition.z), Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;

                //移除item从players inventory
                InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

                if (InventoryManager.Instance.FindItemInventory(InventoryLocation.player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }
            }
        }

    }


    private void RemoveSelectedItemFromInventory()
    {
        if (itemDetails != null && isSelected)
        {
            int itemCode = itemDetails.itemCode;

            //remove item from player inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, itemCode);

            //if  no more of item then clear selected
            if (InventoryManager.Instance.FindItemInventory(InventoryLocation.player, itemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)//开始拖动
    {
        if (itemDetails != null)
        {
            //使玩家输入失效
            Player.Instance.DisablePlayerInputAndRestMovement();

            //将游戏对象实例化为拖动项
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);//instantiate(要复制的物品,将指定给新对象的父对象)

            //得到image从drageditem中
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();

            draggedItemImage.sprite = inventorySlotImage.sprite;

            SetSelectedItem();
        }
    }

    public void OnDrag(PointerEventData eventData)//拖动中
    {
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)//拖动完成
    {
        if (draggedItem != null)
        {

            Destroy(draggedItem);
           

            //如果拖动结束于库存栏，则获取项目拖动结束并交换它们
            if (eventData.pointerCurrentRaycast.gameObject !=null&& eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>()!=null)
                {
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                InventoryManager.Instance.SwapInventoryItem(InventoryLocation.player, slotNumber,toSlotNumber);

                DestroyInventoryTextBox();

                ClearSelectedItem();
                }
            else//否则尝试拖动项目（如果可以拖动）
            {
                if (itemDetails.canBeDropped)
                {
                    DropSelectedItemAtMousePosition();
                }
            }
        }
        //重置玩家移动
        Player.Instance.EnablePlayerInput();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //填充信息
        if (itemQuantity != 0)
        {
            //instantiate inventory text box
            inventoryBar.inventoryTextBoxGameobject = Instantiate(inventoryTextPrefabs, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);//如果为 true，则修改父相对位置、缩放和旋转，以使对象保持与以前相同的世界空间位置、旋转和缩放。

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameobject.GetComponent<UIInventoryTextBox>();

            //设置item type 描述
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            //填充文本
            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            //设置文本位置通过物品栏的位置
            if(inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y +50f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }

        }
    }//当指针放到其上

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }//指针移出

    public void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameobject);
        }
    }//摧毁文本框

    public void OnPointerClick(PointerEventData eventData)//当指针点击时
    {
        //if left click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //如果当前选择了库存槽，则取消选择
            if (isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if (itemQuantity > 0)
                {
                    SetSelectedItem(); 
                }
            }
        }
    }

    public void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTranform).transform;
    }
}
