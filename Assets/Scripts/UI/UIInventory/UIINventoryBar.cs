using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIINventoryBar : MonoBehaviour
{
    [SerializeField]private Sprite blank16x16sperite=null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;
    public GameObject inventoryBarDraggedItem;
    [HideInInspector] public GameObject inventoryTextBoxGameobject;
        
    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    private void OnEnable()
    {
        EventHander.InventoryUpdateEvent += InventoryUpdated;
    }


    private void OnDisable()
    {
        EventHander.InventoryUpdateEvent -= InventoryUpdated;
    }


    private void Update()
    {
        //选择inventorybar位置通过player的位置
        SwitchInventoryBarPosition();
    }


    public void ClearHighlightOnInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            //loop all highlights from inventory slots and clear highlight sprites
            for(int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].isSelected)
                {
                    inventorySlots[i].isSelected = false;
                    inventorySlots[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);

                    //update inventory to show item as not selected
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }


   ///<summary>
   ///通过人物在屏幕中的位置判断，选取inventorybar的位置应该是位于上还是下
   /// </summary>
   private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();
        if (playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom==false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if (playerViewportPosition.y < 0.3f && IsInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }

    /// <summary>
    /// set the selected highlight if set on all inventory item position
    /// </summary>
    public void SetHighlightedInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for(int i = 0; i < inventorySlots.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }

    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if (inventorySlots.Length > 0 && inventorySlots[itemPosition].itemDetails != null)
        {
            if (inventorySlots[itemPosition].isSelected)
            {
                inventorySlots[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);

                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlots[itemPosition].itemDetails.itemCode);
            }
        }
    }

    /// <summary>
    /// 清空物品栏
    /// </summary>
    private void ClearInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            //循环浏览物品栏并使用空白精灵进行更新
            for(int i=0;i<inventorySlots.Length;i++)
            {
                inventorySlots[i].inventorySlotImage.sprite = blank16x16sperite;
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemQuantity = 0;
                SetHighlightedInventorySlots(i);
            }
        }
    }


    /// <summary>
    /// 玩家物品栏处理
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="inventoryList"></param>
    private void InventoryUpdated(InventoryLocation inventoryLocation,List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();
            if (inventorySlots.Length > 0 && inventoryList.Count > 0)//如果物品栏数量大于物品数量就可以去更像物品栏
            {
                //循环遍历库存槽并使用相应的库存列表项进行更新
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (i < inventoryList.Count)//把所有的物品数量全部放入，如果已经全部进入那么break跳出 
                    {
                        int itemCode = inventoryList[i].itemCode;

                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);//从inventorymanager中获取到该信息

                        if (itemDetails != null)
                        {
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;//更新其对应的图片
                            inventorySlots[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();//更新其所对应的数量
                            inventorySlots[i].itemDetails = itemDetails;//更新其所对应的具体信息
                            inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                            SetHighlightedInventorySlots(i);
                        }
                        else
                        {
                            break;
                        }

                    }
                }
            }
        }
    }


    public void ClearCurrentlySelectedItem()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].ClearSelectedItem();
        }
    }

    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].draggedItem != null)
            {
                Destroy(inventorySlots[i].draggedItem);
            }
        }
    }
}
