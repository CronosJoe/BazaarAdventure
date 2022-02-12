using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState //I will be providing base types of items you can expand upon this list for your game
    {
        Sell,
        Buy
    }
    public static GameManager instance;

    public GameState currentState;
    [SerializeField]
    private int playerMoney = 0;

    public int PlayerMoney => playerMoney;

    [Space(10)]
    public InventoryObject playerInventory;
    public int currentMerchantIndex;
    public List<MerchantScript> merchants = new List<MerchantScript>();
    public MerchantScript CurrentMerchant
    {
        get => merchants[currentMerchantIndex];
    }

    public InventoryObject CurrentInventory
    {
        get
        {
            switch (currentState)
            {
                case GameState.Buy:
                    return CurrentMerchant.merchantInventory;
                case GameState.Sell:
                    return playerInventory;
                default:
                    return CurrentMerchant.merchantInventory;
            }
        }
    }

    [Space(10)]
    private List<DisplayItem> displayedInventorySlots = new List<DisplayItem>();
    public RectTransform displayParent;
    public GameObject itemSlotPrefab;

    [Space(10)]
    public GameObject amountCollector;
    public TMP_Text amountNumber;
    public Scrollbar amountSlider;
    public Button confirmButton;
    public TMP_Text playerBalance;

    private int currentItemIndex;

    private void Start()
    {
        if(instance !=null) 
        {
            Destroy(gameObject);
        }
        else 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        amountCollector.SetActive(false);
        currentState = GameState.Buy;
        DisplayInventory();
        UpdatePlayerBalanceDisplay();
    }

    public void AddPlayerMoney(int amount)
    {
        playerMoney += amount;
    }
    public void TakePlayerMoney(int amount) 
    {
        playerMoney -= amount;
    }

    public void IncrementMerchant(int increment) 
    {
        currentMerchantIndex = Mathf.Clamp(currentMerchantIndex + increment, 0, merchants.Count-1);
        currentState = GameState.Buy;
        DisplayInventory();
    }
    public void ToggleSelling()
    {
        if (currentState == GameState.Sell)
            currentState = GameState.Buy;
        else
            currentState = GameState.Sell;

        DisplayInventory();
    }
    public void DisplayInventory()
    {
        //okay so here me out this definitely won't be inefficient
        //check
        if (displayedInventorySlots.Count < CurrentInventory.Container.Count) //change this so it also handles player inventory
        {
            //add slots if needed
            for (int i = displayedInventorySlots.Count; i < CurrentInventory.Container.Count; i++)
            {
                displayedInventorySlots.Add(Instantiate(itemSlotPrefab, displayParent).GetComponent<DisplayItem>());
                int index = i;
                displayedInventorySlots[i].itemButton.onClick.AddListener(() => PurchaseItem(index));
            }
        }
            UpdateCurrentInventory();
    }
    public void UpdateCurrentInventory() //passing in the inventory that we are updating from
    {
        for(int i = 0; i < CurrentInventory.Container.Count; i++) 
        {
            displayedInventorySlots[i].gameObject.SetActive(true);
            displayedInventorySlots[i].textToUpdate.SetText(CurrentInventory.Container[i].item.name + " $" + CurrentInventory.Container[i].item.cost);
        }
        if (displayedInventorySlots.Count > CurrentInventory.Container.Count) 
        {
            for(int i = CurrentInventory.Container.Count; i < displayedInventorySlots.Count; i++) 
            {
                displayedInventorySlots[i].gameObject.SetActive(false);
            }
        }
    }
    public void UpdatePlayerBalanceDisplay() 
    {
        playerBalance.SetText("$" + PlayerMoney.ToString("0"));
    }

    public void PurchaseItem(int index)
    {
        ToggleActive(CurrentInventory.Container[index]);
        currentItemIndex = index;
    }

    public void ToggleActive() 
    {
        amountCollector.SetActive(!amountCollector.activeSelf);
    }
    public void ToggleActive(InventorySlot itemHeld) 
    {
        amountCollector.SetActive(!amountCollector.activeSelf);
        amountCollector.GetComponent<WhatToBuy>().itemHeld = itemHeld;
    }
    public void OnValueChanged()
    {
        InventorySlot curItem = CurrentInventory.Container[currentItemIndex];

        amountNumber.text = Mathf.RoundToInt(amountSlider.value * curItem.amount).ToString();
        confirmButton.interactable = currentState == GameState.Sell || Mathf.RoundToInt(amountSlider.value * curItem.amount) * curItem.item.cost <= playerMoney;    
        
    }

    public void ConfirmTransaction() 
    {
        InventorySlot curItem = CurrentInventory.Container[currentItemIndex];
        int itemQuantity = Mathf.RoundToInt(amountSlider.value * curItem.amount);

        switch (currentState) 
        {
            case GameState.Buy:
                if(playerMoney >= itemQuantity * curItem.item.cost) 
                {
                    TakePlayerMoney(itemQuantity * curItem.item.cost);

                    playerInventory.AddItem(curItem.item, itemQuantity);
                    CurrentMerchant.merchantInventory.RemoveItemAmount(curItem.item, itemQuantity);

                    UpdateCurrentInventory();
                }
                else 
                {
                    Debug.Log("not enough cash money");
                }
                break;
            case GameState.Sell:
                AddPlayerMoney(itemQuantity * curItem.item.cost);
                playerInventory.RemoveItemAmount(curItem.item, itemQuantity);
                UpdateCurrentInventory();
                break;
        }
        UpdatePlayerBalanceDisplay();
        ToggleActive();
    }
}
