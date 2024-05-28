using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTranform;//矩形变换
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    [SerializeField] private GridCursor gridCursor = null;

    private bool _cursorIsEnable = false;
    public bool CursorIsEnabled { get => _cursorIsEnable; set => _cursorIsEnable = value; }

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType=value; }

    private float _itemUseRadius = 0f;
    public float ItemUseRadius { get => _itemUseRadius; set => _itemUseRadius = value; }

    //start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    //update is called once per frame
    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private void DisplayCursor()
    {
        //get position for cursor
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();

        //set cursor sprite 
        SetCursorValidity(cursorWorldPosition, Player.Instance.GetPlayerCentrePosition());

        //get rect transform position for cursor
        cursorRectTranform.position = GetRectTransformPositionForCursor();
    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPostion)
    {
        SetCursorToValidity();

        //Check use radius corners

        if (cursorPosition.x > (playerPostion.x + ItemUseRadius / 2f) && cursorPosition.y > (playerPostion.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPostion.x - ItemUseRadius / 2f) && cursorPosition.y > (playerPostion.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPostion.x - ItemUseRadius / 2f) && cursorPosition.y < (playerPostion.y - ItemUseRadius / 2f)
            ||
            cursorPosition.x > (playerPostion.x + ItemUseRadius / 2f) && cursorPosition.y < (playerPostion.y - ItemUseRadius / 2f))
        {
            SetCursorToInvalidity();
            return;
        }

        //check item use radius is valid
        if(Mathf.Abs(cursorPosition.x-playerPostion.x)>ItemUseRadius
            || Mathf.Abs(cursorPosition.y - playerPostion.y) > ItemUseRadius)
        {
            SetCursorToInvalidity();
        }

        //get selected item details
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null)
        {
            SetCursorToInvalidity();
            return;
        }

        if ( itemDetails.itemType == ItemType.Reaping_tool)
        {
            if (!SetCursorValidityTool(cursorPosition, playerPostion, itemDetails))
            {
                SetCursorToInvalidity();
                return;
            }
        }
    }

    private void SetCursorToInvalidity()
    {
        cursorImage.sprite = transparentCursorSprite;
        CursorPositionIsValid = false;
        gridCursor.EnableCursor();
    }

    private void SetCursorToValidity()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
        gridCursor.DisableCursor();
    }

    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    /// <summary>
    /// 设置游标对目标的有效性或无效性。 如果有效，返回ture，如果无效，返回false。
    /// </summary>
    /// <param name="cursorPosition"></param>
    /// <param name="playerPosition"></param>
    /// <param name="itemDetails"></param>
    /// <returns></returns>
    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition,ItemDetails itemDetails)
    {
        if(itemDetails.itemType== ItemType.Reaping_tool)
        {
            return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);
        }
        return false;
    }

    private bool SetCursorValidityReapingTool(Vector3 cursorPosition,Vector3 playerPosition,ItemDetails itemDetails)
    {
        List<Item> itemList = new List<Item>();

        if(HelperMethods.GetComponentsAtCursorLocation<Item>(out itemList, cursorPosition))
        {
            if (itemList.Count != 0)
            {
                foreach(Item item in itemList)
                {
                    if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary) 
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public Vector3 GetWorldPositionForCursor()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y,0f);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);//将 position 从屏幕空间变换为世界空间。屏幕空间以像素定义。屏幕的左下角为(0, 0)，右上角 为(pixelWidth, pixelHeight)。z 位置为与摄像机的距离，采用世界单位。

        return worldPosition;
    }

    public Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTranform, canvas);//将屏幕空间中的给定点转换为像素校正点。
    }
}
