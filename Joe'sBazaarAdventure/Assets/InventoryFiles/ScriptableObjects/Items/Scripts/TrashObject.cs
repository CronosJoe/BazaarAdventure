using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Trash Object", menuName = "Inventory System/Items/Trash")]
public class TrashObject : ItemObject
{
    public int ToxicLevel;
    public void Awake()
    {
        type = ItemType.Trash;
    }
}
