using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;//矩形的位置、大小、锚点和轴心信息。
    [SerializeField] private Sprite greenCursorSprite = null;//绿色方框
    [SerializeField] private Sprite redCursorSprite = null;//红色方框
    [SerializeField] private SO_CropDetailsList so_cropDetailsLists = null;

    private bool _cursorPositionIsVaild = false; //光标位置是否有效

    public bool CursorPositionIsVaild { get => _cursorPositionIsVaild; set => _cursorPositionIsVaild = value; }

    private int _itemUseGridRadius = 0;//item使用网格半径

    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value;}

    private ItemType _selectItemType;//item类型：有商品，种子，锄头等等

    public ItemType SelectItemType { get => _selectItemType; set => _selectItemType = value; }

    private bool _cursorIsEnable = false;

    public bool CursorIsEnable { get => _cursorIsEnable; set => _cursorIsEnable = value; }//Cursor光标 



    private void OnEnable()
    {
        EventHander.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHander.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void SceneLoaded()
    {
        grid = Object.FindObjectOfType<Grid>();
    }

    //start会在第一次帧更新之前被调用
    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    //update会在第一帧更新后调用
    private void Update()
    {
        if (CursorIsEnable)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            //获取光标的网格位置,由光标在世界坐标转为网格坐标
            Vector3Int gridPosition = GetGridPositionForCursor();

            //获取玩家的网格信息
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            //设置光标sprite
            SetCursorValidity(gridPosition, playerGridPosition);

            //获取光标的矩形变换位置
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnable = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnable = true;
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);

    }

    //得到鼠标的网格坐标
    public Vector3Int GetGridPositionForCursor()
    {
        //z 是物体在相机前面的距离 -相机在 -10 所以物体在前面 （-）-10 =10
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

        return grid.WorldToCell(worldPosition);// grid.WorldToCell将世界位置转换为单元格位置。
    }

    //得到player的网格坐标
    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    //得到鼠标的世界坐标
    public Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }

    /// <summary>
    ///通过丢弃半径，地面属性综合判断该处是否可以丢弃
    /// </summary>
    /// <param name="cursorGridPosition"></param>
    /// <param name="playerGridPositin"></param>
    private void SetCursorValidity(Vector3Int cursorGridPosition,Vector3Int playerGridPositin)//validity有效性
    {
        SetCursorToValid(); 

        //检查项目半径是否有效
        if(Mathf.Abs(cursorGridPosition.x-playerGridPositin.x)>ItemUseGridRadius
            || Mathf.Abs(cursorGridPosition.y - playerGridPositin.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        //获取所选项目详细信息
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        //获取光标位置的网格属性详细信息
        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            //根据所选库存中所选取的item和网格属性详细信息确定光标的有效性
            if (itemDetails.itemType == ItemType.Seed)
            {
                if (!IsCursorValidForSeed(gridPropertyDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
            }
            else if (itemDetails.itemType == ItemType.Commodity)
            {
                if (!IsCursorValidForCommodity(gridPropertyDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
            }
            else if (itemDetails.itemType == ItemType.Hoeing_tool
                || itemDetails.itemType == ItemType.Watering_tool
                || itemDetails.itemType==ItemType.Chopping_tool
                ||itemDetails.itemType==ItemType.Breaking_tool
                ||itemDetails.itemType==ItemType.Reaping_tool
                ||itemDetails.itemType==ItemType.Collecting_tool)
            {
                if (!IsCursorValidForTool(gridPropertyDetails, itemDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
            }

        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

 
    //设置光标有效
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsVaild = true;
    }

    //设置光标无效
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsVaild = false;
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    /// <summary>
    /// 将光标设置为无效或对目标网格有效属性详细信息。如果有效则返回true，如果无效则返回 false
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <param name="itemDetails"></param>
    /// <returns></returns>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        //Switch on tool
        //hoeing tool
        if (itemDetails.itemType == ItemType.Hoeing_tool)
        {
            if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1)
            {
                #region 需要在现场获得任何物品，以便我们可以检查它们是否可以收获
                //得到鼠标的世界坐标
                Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                //获取光标位置处的项目列表
                List<Item> itemList = new List<Item>();

                HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);
                #endregion

                //循环浏览itemlist，看看是否有可收割的类型 - 我们不会让玩家挖掘那里有可收割 items
                bool foundReapable = false;
                foreach(Item item in itemList)
                {
                    if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        foundReapable = true;
                        break;
                    }
                }
                if (foundReapable)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
         return false;
        }

        //watering tool
        else if (itemDetails.itemType == ItemType.Watering_tool)
        {
            if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //chop tool
        //collection tool
        //breaking tool
        else if (itemDetails.itemType == ItemType.Collecting_tool||itemDetails.itemType==ItemType.Chopping_tool||itemDetails.itemType==ItemType.Breaking_tool)
        {
            //check if item can harvested with selected tool, check item is grown

            //check if seed planted
            if (gridPropertyDetails.seedItemCode != -1)
            {
                //get crop details for seed 
                CropDetails cropDetails = so_cropDetailsLists.GetCropDetails(gridPropertyDetails.seedItemCode);

                //if crop details found
                if (cropDetails != null)
                {
                    //check if crop fully grown
                    if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length-1])
                    {
                        //check if crop can be harvested with selected tool
                        if (cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        return false;
    }

   
}
