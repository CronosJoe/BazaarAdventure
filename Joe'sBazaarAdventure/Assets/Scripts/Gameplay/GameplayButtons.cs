using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayButtons : MonoBehaviour
{
    public GameManager gm;

    public void NextMerchant() 
    {
        if(gm.currentMerchantIndex != gm.merchants.Count) 
        {
            gm.currentMerchantIndex++;
            gm.currentMerchant = gm.merchants[gm.currentMerchantIndex];
            //will probably want to call some method that wil swap out the image/update inventory
        }
    }
    public void PrevMerchant() 
    {
        if (gm.currentMerchantIndex != 0)
        {
            gm.currentMerchantIndex--;
            gm.currentMerchant = gm.merchants[gm.currentMerchantIndex];
            //will probably want to call some method that wil swap out the image/update inventory
        }
    }
    public void QuestButton() 
    {
        //call some kind of overlay to handle the quest system
    }
    public void DisplayPlayerInventory() 
    {
        //this will be an update display but with the user's inventory instead of a merchants inventory
    }
}
