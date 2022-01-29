using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;
    public InventorySlot(ItemObject _item, int _amount) 
    {
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value) 
    {
        amount += value;
    }
    public void RemoveAmount(int value) 
    {
        amount -= value;
    }
}
