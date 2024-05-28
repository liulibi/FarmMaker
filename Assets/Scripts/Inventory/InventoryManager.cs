using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>, ISaveable
{
    private UIINventoryBar inventoryBar;

    private Dictionary<int, ItemDetails> itemDetailDictionary;

    private int[] selectedInventoryItem;//数组的索引是库存列表，item code

    public List<InventoryItem>[] inventoryLists;//库存清单

    [HideInInspector] public int[] inventoryListCapacityIntArray;//数组的索引是清单列表

    [SerializeField] private So_ItemList itemList = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    protected override void Awake()
    {
        base.Awake();

        CreatrItemDetailsDictionary();

        CreateInventoryLists();

        selectedInventoryItem = new int[(int)InventoryLocation.count];//有两个存储空间

        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;//未选择状态都是-1
        }

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();
    }
    

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void Start()
    {
        inventoryBar = FindObjectOfType<UIINventoryBar>();
    }



    /// <summary>
    /// 从可编写脚本的对象项列表中填充“项详细信息字典”
    /// </summary>
    private void CreatrItemDetailsDictionary()
    {
        itemDetailDictionary = new Dictionary<int, ItemDetails>();

        foreach(ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }


    ///<summary>
    ///返回item细节从SO_item或者判断是否为空
    ///</summary>
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;
        if(itemDetailDictionary.TryGetValue(itemCode,out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// 得到这个item type的描述将item type作为string返回
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType) 
        {
            case ItemType.Breaking_tool:
                itemTypeDescription = Settings.BreakingTool;
                break;
            case ItemType.Chopping_tool:
                itemTypeDescription = Settings.ChopingTool;
                break;
            case ItemType.Hoeing_tool:
                itemTypeDescription = Settings.HoeingTool;
                break;
            case ItemType.Reaping_tool:
                itemTypeDescription = Settings.ReapingTool;
                break;
            case ItemType.Watering_tool:
                itemTypeDescription = Settings.WateringTool;
                break;
            case ItemType.Collecting_tool:
                itemTypeDescription = Settings.CollectingTool;
                break;
            default:
                itemTypeDescription = itemType.ToString();
                break;
        }
        return itemTypeDescription;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectlist.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        //Create new scene save
        SceneSave sceneSave = new SceneSave();

        //Remove any existing scene save for persistent scene for this gameobject
        GameObjectSave.sceneDate.Remove(Settings.PersistentScene);

        //Add inventory lists array to persistent scene save
        sceneSave.listInvItemArray = inventoryLists;

        //Add inventory list caoacity array to persistent
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapacityArray", inventoryListCapacityIntArray);

        //Add scene save for gameobject
        GameObjectSave.sceneDate.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectDate.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            //Need to find inventory list - start by trying to locate saceScene for game object
            if (gameObjectSave.sceneDate.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                //list inv items array exists for persistent scene
                if (sceneSave.listInvItemArray != null)
                {
                    inventoryLists = sceneSave.listInvItemArray;

                    //Send events that inventory has been updated
                    for (int i = 0; i < (int)InventoryLocation.count; i++)
                    {
                        EventHander.CallInventoryUpdatedEvent((InventoryLocation)i, inventoryLists[i]);
                    }

                    //Clear any items player was carrying
                    Player.Instance.ClearCarriedItem();

                    //Clear any highlights on inventory bar
                    inventoryBar.ClearHighlightOnInventorySlots();
                }

                //int array dictionary exists for scene
                if (sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray", out int[] inventoryCapacityArray))
                {
                    inventoryListCapacityIntArray = inventoryCapacityArray;
                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {

    }

    public void ISaveableRestoreScene(string sceneName)
    {

    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];

        for(int i=0;i<(int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        //初始化库存列表容量阵列
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        //初始化玩家库存列表容量
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }


    ///<summary>
    ///添加物品到背包中，然后destroy这个物品
    ///</summary>
    public void AddItem(InventoryLocation inventoryLocation,Item item,GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }


    ///<summary>
    ///添加物品
    ///<summary>
    public void AddItem(InventoryLocation inventoryLocation,Item item)//添加Item
    {
        int itemcode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        int itemPosition = FindItemInventory(inventoryLocation, itemcode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemcode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemcode);
        }
        //发送库存已更新的事件
        EventHander.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// add an item of type to the inventory list for the inventorylaction
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void AddItem(InventoryLocation inventoryLocation,int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //check if inventory already contains the item
        int itemPosition = FindItemInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        //send event that inventory has been updated
        EventHander.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    ///<summary>
    ///添加item到inventory中
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList,int itemcode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemcode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

       //DebugPrintInventoryList(inventoryList);

    }

    ///<summary>
    ///添加物品
    ///</summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itmeCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryList[position].itemQuantity + 1;
        inventoryItem.itemCode = itmeCode;
        inventoryItem.itemQuantity = quantity;

        inventoryList[position] = inventoryItem;

        //DebugPrintInventoryList(inventoryList);
    }


    /// <summary>
    /// 交换物品
    /// </summary>
    public void SwapInventoryItem(InventoryLocation inventoryLocation,int fromItem,int toItem)
    {
        //  如果可以交换物品
        if (fromItem < inventoryLists[(int)inventoryLocation].Count&&toItem<inventoryLists[(int)inventoryLocation].Count
            && fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;
            inventoryLists[(int)inventoryLocation][toItem] = fromInventoryItem;

            //send event that inventory has been updated
            EventHander.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }


    /// <summary>
    /// Clear the selected inventory item for inventoryLocation
    /// </summary>
    /// <param name="inventoryLocation"></param>

    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)//使选中的存储空间变为未选择状态
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }

    ///<summary>
    ///如果物品code已经存在则返回position
    ///-1表示其不在物品栏中
    /// </summary>
    public int FindItemInventory(InventoryLocation inventoryLocation,int itemcode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        for(int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemcode)
            {
                return i;
            }
        }
        return -1;
    }

    
    /// <summary>
    /// 删除物品从inventory中,并且创建一个物品在掉落处
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void RemoveItem(InventoryLocation inventoryLocation,int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //检查库存是否已包含该商品
        int itemPosition = FindItemInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        //发送已更新的事件
        EventHander.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);

    }

    /// <summary>
    /// 删除一个物体
    /// </summary>
    /// <param name="inventoryList"></param>
    /// <param name="itemCode"></param>
    /// <param name="position"></param>
    private void  RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode,int position)
    {
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryList[position].itemQuantity - 1;//quantity 数量
        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }
        
    }


    /// <summary>
    /// set the selected inventory item for inventoryLocation to itemcode
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation,int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }//表明该存储栏存储的物品

   


   /// <summary>
   ///得到选择的物品对于选择的背包，返回itemcode or -1如果没有东西被选中 
   /// </summary>
   /// <param name="inventoryLocation"></param>
   /// <returns></returns>
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }


    /// <summary>
    /// 返回itemDetails（从SO_ItemList中）对于在当前背包中选中的物品，或者返回null如果无物品被选中
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <returns></returns>
    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }
}
