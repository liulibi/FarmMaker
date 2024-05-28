using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            //得到itemdetail
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            if (itemDetails.canBePickedUp == true)
            {
                //添加物品方法
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);

                //play sound 
                AudioManager.Instance.PlaySound(SoundName.effectPickupSound);
            }
    
        }
    }
}
