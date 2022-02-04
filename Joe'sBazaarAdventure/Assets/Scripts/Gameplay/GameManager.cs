using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int playerMoney;
    public InventoryObject playerInventory;
    public int GetPlayerMoney() 
    {
        return playerMoney;
    }
    public void AddPlayerMoney(int amount)
    {
        playerMoney += amount;
    }
}
