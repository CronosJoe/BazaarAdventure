using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType //I will be providing base types of items you can expand upon this list for your game
{
    Food,
    Equipment,
    Default
}
public abstract class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public int cost; //this is a standard in most games with inventories so this is what I'll use to sort the list
    public ItemType type; //storing this items type
    [TextArea(15, 20)]
    public string description; //storing the items description
    public int amountCap; //how much you can stack of each item
}
