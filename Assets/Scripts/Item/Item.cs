using UnityEngine;

public class Item : MonoBehaviour
{
    [ItemCodeDescriptionAttributer]
    [SerializeField]
    private int _itemCode;

    private SpriteRenderer SpriteRenderer;

    public int ItemCode { get { return _itemCode; } set { _itemCode = value; } }

    private void Awake()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    public void Init(int itemCodePAram)//初始化
    {
        if (itemCodePAram != 0)
        {
            ItemCode = itemCodePAram;

            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);

            SpriteRenderer.sprite = itemDetails.itemSprite;

            //如果项目类型是可互动的，则添加可轻推产生旋转效果的组件
            if (itemDetails.itemType == ItemType.Reapable_scenary)
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }
}
