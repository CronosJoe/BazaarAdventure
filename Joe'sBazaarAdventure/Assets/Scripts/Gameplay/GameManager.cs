using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int playerMoney;
    public InventoryObject playerInventory;
    public MerchantScript currentMerchant; //might want to create some gets/sets for these
    public int currentMerchantIndex;
    public List<MerchantScript> merchants = new List<MerchantScript>();
    private List<DisplayItem> displayedInventorySlots = new List<DisplayItem>();
    public RectTransform displayParent;
    public GameObject itemSlotPrefab;
    public int GetPlayerMoney() 
    {
        return playerMoney;
    }
    public void AddPlayerMoney(int amount)
    {
        playerMoney += amount;
    }
    public void UpdateCurrentInventory(InventoryObject inv) //not sure if I want to add in a parameter for which inventory I want to load
    {
        for(int i = 0; i < inv.Container.Count; i++) 
        {

        }
    }
    public void DisplayInventory(InventoryObject inv)
    {
        //okay so here me out this definitely won't be inefficient
        //check
        if (displayedInventorySlots.Count < inv.Container.Count) //change this so it also handles player inventory
        {
            //add slots if needed
            for (int i = displayedInventorySlots.Count - 1; i < inv.Container.Count; i++)
            {
                displayedInventorySlots.Add(Instantiate(itemSlotPrefab, displayParent).GetComponent<DisplayItem>());
                //displayedInventorySlots[i].itemButton.onClick.AddListener()
            }
            UpdateCurrentInventory(inv);
        }//disable excess
    }
    public void PurchaseItem(int index) 
    {
        //do a confirm message
        //call the add item for the player inventory
        playerInventory.AddItem(currentMerchant.merchantInventory.Container[index].item, currentMerchant.merchantInventory.Container[index].amount);
    }

}
