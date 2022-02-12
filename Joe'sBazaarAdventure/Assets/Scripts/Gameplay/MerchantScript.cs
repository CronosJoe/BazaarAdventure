using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Merchant", menuName = "Inventory System/Merchants/Merchant")]
public class MerchantScript : ScriptableObject
{
    public InventoryObject merchantInventory; //each merchants inventory
    public float favoredItemModifier;
    public float unfavoredItemMod;
    public float exchangeRate;
    [SerializeField] ItemType favoredItem;
    public void ItemBought(int itemIndex, int amount) //merchant selling to player
    {
        if (GameManager.instance.PlayerMoney >= merchantInventory.Container[itemIndex].item.cost)
        {
            GameManager.instance.playerInventory.AddItem(merchantInventory.Container[itemIndex].item, amount);
            merchantInventory.RemoveItemAmount(merchantInventory.Container[itemIndex].item, amount);
        }
    }
    public void ItemSold(int itemIndex, int amount) //player selling item to the merchant
    {
        //hacker handling could go here but atm I trust my player
        GameManager.instance.AddPlayerMoney(GameManager.instance.playerInventory.Container[itemIndex].item.cost *amount);
        GameManager.instance.playerInventory.RemoveItemAmount(GameManager.instance.playerInventory.Container[itemIndex].item, amount);
    }
    public int GetItemCost(ItemObject item, int amount) 
    {
        if(item.type == favoredItem) 
        {
            return (int)((exchangeRate + favoredItemModifier) * item.cost * amount);
        }
        else
        {
            return (int)((exchangeRate - unfavoredItemMod) * item.cost * amount);
        }
    }
}
