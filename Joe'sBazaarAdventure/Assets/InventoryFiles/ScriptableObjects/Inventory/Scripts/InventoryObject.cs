using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public int inventorySize; //this will cap the maximum amount of inventory slots there are for unique items
   
    //adding items
    public void AddItem(ItemObject _item, int _amount) 
    {
        for (int i = 0; i < Container.Count; i++) 
        {
            if(Container[i].item == _item && Container[i].item.amountCap > Container[i].amount) //check if we have space available in our current item slot
            {
                int currentSlotSpace = _item.amountCap-Container[i].amount;//the max space minus the current amount of items
                if (_amount <= currentSlotSpace) //if less then current slot amount MOST COMMON
                {
                    Container[i].AddAmount(_amount);
                    return;
                }
                else //this will handle if we have more the current slot amount and will likely need to be in a loop
                {
                    bool notDone = false;
                    int remainderAmount = _amount - currentSlotSpace;
                    Container[i].AddAmount(currentSlotSpace);
                    while (!notDone)
                    {
                        if (remainderAmount <= _item.amountCap) //if we're on our last iteration this will end it
                        {
                            AddNewSlot(_item, remainderAmount);
                            return; //get us outta here Scotty or whoever else is out there!
                        }
                        else //if the remainder is greater than the cap
                        {
                            remainderAmount -= _item.amountCap;
                            AddNewSlot(_item, _item.amountCap);
                        }
                    }
                }
            }
        }
        AddNewSlot(_item, _amount);
    }
    //Adding a new inventory slot to the list
    private void AddNewSlot(ItemObject _item, int _amount) 
    {
        if (!CheckIfInventoryFull()) //to confirm we don't go over cap, this only checks if ever slot has something in it
        {
            Container.Add(new InventorySlot(_item, _amount));
        }
        else 
        {
            Debug.Log("You went over slot count! Implent the behavior you'd like here InventoryObject Line 56");
        }
    }
    //removing items
    public void RemoveAtIndex(int index)
    {
        if((index>= 0)&&(index<Container.Count) )
        {
            //For your game add functionality in a shop or here to sell the item or simply drop it
            Container.RemoveAt(index);
        }
    }
    public void RemoveItemAmount(ItemObject _item, int _amount) 
    {
        for (int i = 0; i < Container.Count; i++) //should add in handling for if we go over a stack amount then might need to remove some from another stack
        {
            if (Container[i].item == _item)
            {
                Container[i].RemoveAmount(_amount);
                if (Container[i].amount <= 0) 
                {
                    RemoveAtIndex(i);
                }
                return;
            }
        }
    }
    //Clearing the inventory to completely empty it
    public void ClearInventory() 
    {
        Container.Clear();
    }
    //Sorting/swapping functionality
    public void Swap(int swappingIndex, int swappedIndex) //the first parameter will be swapped into the position of the second and visa versa
    {
        InventorySlot tmpInventorySlot = Container[swappedIndex]; //save the one that will be overrode
        Container[swappedIndex] = Container[swappingIndex]; //replace the one we saved in a local
        Container[swappingIndex] = tmpInventorySlot; //finish the swap
    }
    public void SortByType() 
    {
       Container.Sort(delegate (InventorySlot a, InventorySlot b)
        {
            return a.item.type.CompareTo(b.item.type);
        });
    } public void SortByCost() 
    {
       Container.Sort(delegate (InventorySlot a, InventorySlot b)
        {
            return a.item.cost.CompareTo(b.item.cost);
        });
    }
    public void ReplaceItem(int index, ItemObject itemToAdd, int amountToAdd) 
    {
        //nothing fancy here just combining method calls so that we can repalce an item if our inventory is too full
        RemoveAtIndex(index);
        AddItem(itemToAdd, amountToAdd);
    }
    public bool CheckIfInventoryFull() 
    {//we need to check if the current amount of inventory items is equal to the maximum if it is return true, if it ever goes over we have bigger problems
        return (Container.Count - 1 >= inventorySize);
    }
}
