﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

[CreateAssetMenu(fileName = "New Merchant", menuName = "Inventory System/Merchants/Merchant")]
public class MerchantScript : ScriptableObject
{
    public InventoryObject merchantInventory; //each merchants inventory
    public Sprite merchantSprite;
    public List<ItemObject> sellableItems = new List<ItemObject>();

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
    public void NewDayRestock() 
    {
        //might want to add something to change modifiers here
        merchantInventory.ClearInventory(); //empty the inventory first
        Random rand =  new Random();
        for(int i =0; i<merchantInventory.inventorySize; i++) //be careful with this one, make sure inventory sizes are correct, might also want to make inventory sizes larger as days go on
        {
            ItemObject tmpItem = sellableItems[rand.Next(0, sellableItems.Count)];
            merchantInventory.AddItem(tmpItem, rand.Next(tmpItem.amountCap / 2)); //might want to adjust this formula later
        }
    }
}
