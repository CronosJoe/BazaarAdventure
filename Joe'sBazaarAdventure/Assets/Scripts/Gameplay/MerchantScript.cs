using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantScript : MonoBehaviour
{
    public InventoryObject merchantInventory; //each merchants inventory
    public float favoredItemModifier;
    public float unfavoredItemMod;
    public float exchangeRate;
    [SerializeField] GameManager gameManager;
    [SerializeField] ItemType favoredItem;
    public void ItemBought(int itemIndex, int amount) //merchant selling to player
    {
        if (gameManager.GetPlayerMoney() >= merchantInventory.Container[itemIndex].item.cost)
        {
            gameManager.playerInventory.AddItem(merchantInventory.Container[itemIndex].item, amount);
            merchantInventory.RemoveItemAmount(merchantInventory.Container[itemIndex].item, amount);
        }
    }
    public void ItemSold(int itemIndex, int amount) //player selling item to the merchant
    {
        //hacker handling could go here but atm I trust my player
        gameManager.AddPlayerMoney(gameManager.playerInventory.Container[itemIndex].item.cost*amount);
        gameManager.playerInventory.RemoveItemAmount(gameManager.playerInventory.Container[itemIndex].item, amount);
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
