using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = System.Random;
using UnityEngine.InputSystem;
using System;

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
    [SerializeField]
    private int playerReliefPackage;

    public int PlayerMoney => playerMoney;

    [Space(10)]
    public InventoryObject playerInventory;
    public int currentMerchantIndex;
    public List<MerchantScript> merchants = new List<MerchantScript>();

    [Space(10)]
    public int DayCount;
    private bool QuestRunning = false;
    [SerializeField] private int costOfAdventure;
    [SerializeField] private Button confirmQuest;
    private int QuestDayGoal;
    private int daysOnQuest;
    public ItemType goldenType;
    private ItemType selectedType;
    private int currentQuestCost;
    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject questButtons;
    [SerializeField] private GameObject questDisplay;
    [SerializeField] private GameObject itemTypeOfDay;
    [SerializeField] private GameObject gameWonScreen;
    [SerializeField] private TMP_Text gameWonText;
    [Space(10)]
    [SerializeField] private PlayerInput inputTracker;
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
    public SpriteRenderer merchantDisplay;

    private int currentItemIndex;
    private void OnEnable() //this will enable all of our input events when the object is enabled in the scene
    {
        inputTracker.currentActionMap["Quit"].performed += QuitGame;
    }


    private void OnDisable() //this will let me disable our input events when the object leaves
    {
        try
        {
            inputTracker.currentActionMap["Quit"].performed -= QuitGame;
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Failed to unsubscribe from the input event methods due to the input manager being removed first, this will always occur in non builds", this);
        }
    } 

    private void QuitGame(InputAction.CallbackContext obj)
    {
        Application.Quit();
    }

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

    public void StartNewDay() 
    {
        Random rand = new Random();
        AddPlayerMoney(playerReliefPackage);
        UpdatePlayerBalanceDisplay();
        playerReliefPackage /= 2;
        for (int i = 0; i < merchants.Count; i++) 
        {
            merchants[i].NewDayRestock();
        }
        DayCount++;
        goldenType = (ItemType)rand.Next(0, 4);//we have 4 items, so max exclusive
        questDisplay.GetComponent<TMP_Text>().text = QuestDayGoal-DayCount + " Days Left!";
        itemTypeOfDay.GetComponent<TMP_Text>().text = "Item Type of the Day: " + goldenType;
        if (DayCount == QuestDayGoal) 
        {
            GiveQuestReward();
        }
        DisplayInventory();
        //UpdateCurrentInventory();
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
        DisplayCurrentMerchant();
    }
    public void DisplayCurrentMerchant() 
    {
        merchantDisplay.sprite = CurrentMerchant.merchantSprite;
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
        Debug.Assert(CurrentInventory.Container.Count <= displayedInventorySlots.Count, "Not enough slots for display!");
        for(int i = 0; i < CurrentInventory.Container.Count; i++) 
        {
            displayedInventorySlots[i].gameObject.SetActive(true);
            displayedInventorySlots[i].textToUpdate.SetText(CurrentInventory.Container[i].item.itemName + " $" + CurrentMerchant.GetItemCost(CurrentInventory.Container[i].item, 1));
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
                    TakePlayerMoney(CurrentMerchant.GetItemCost(curItem.item, itemQuantity));

                    playerInventory.AddItem(curItem.item, itemQuantity);
                    CurrentMerchant.merchantInventory.RemoveItemAmount(curItem.item, itemQuantity);

                    UpdateCurrentInventory();
                    if(curItem.item.type == ItemType.Default && itemQuantity!=0)
                    {
                        GameWin();
                    }
                }
                break;
            case GameState.Sell:
                AddPlayerMoney(CurrentMerchant.GetItemCost(curItem.item, itemQuantity));
                playerInventory.RemoveItemAmount(curItem.item, itemQuantity);
                UpdateCurrentInventory();
                break;
        }
        UpdatePlayerBalanceDisplay();
        ToggleActive();
    }

    public void ToggleQuest() 
    {
        questPanel.SetActive(!questPanel.activeSelf);
        if (questPanel.activeSelf) 
        {
            confirmQuest.interactable = false;
            questDisplay.SetActive(QuestRunning);
            questButtons.SetActive(!QuestRunning);
        }
    }
    public void AdjustQuestLength(int selection) 
    {
        currentQuestCost = (selection + 1) * costOfAdventure;
        daysOnQuest = selection + 1;
        QuestDayGoal = DayCount + daysOnQuest;
        confirmQuest.interactable = currentQuestCost <= playerMoney;
    }
    public void ConfirmQuest() 
    {
        selectedType = goldenType;
        playerMoney -= currentQuestCost;
        QuestRunning = true;
        questDisplay.GetComponent<TMP_Text>().text = QuestDayGoal - DayCount + " Days Left!";
        questDisplay.SetActive(QuestRunning);
        questButtons.SetActive(!QuestRunning);
    }
    public void GiveQuestReward()
    {
        Random rand = new Random();
        for(int i =0; i < merchants.Count; i++) 
        {
            if(merchants[i].FavoredItem == selectedType) 
            {
                ItemObject chosenItem = merchants[i].sellableItems[rand.Next(0, merchants[i].sellableItems.Count)];
                playerInventory.AddItem(chosenItem, daysOnQuest*2);
            }
        }
        QuestRunning = false;
    }
    private void GameWin() 
    {
        gameWonScreen.SetActive(true);
        gameWonText.SetText("You completed the game in "  + DayCount +" days!");
    }
}
