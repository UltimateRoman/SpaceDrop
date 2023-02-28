using System;
using SpaceDrop;
using UnityEngine;
using TMPro;
using SpaceDrop;

public class Wallet : MonoBehaviour {
    private static Wallet _instance;

    [SerializeField] private Garage garage;

    private TextMeshProUGUI walletText;
    private int amount = 0;
    private int gameCoinIncrement = 0;

    public static void SetWalletObject(TextMeshProUGUI walletTextObject) {
        _instance.walletText = walletTextObject;
    }

    // Get current player wallet amount.
    public static int GetAmount() {
        return _instance.amount;
    }

    // Set player amount to custom value.
    public static void SetAmount(int amountToSet) {
        _instance.amount = amountToSet;
        DisplayTotalAmount();
    }

    public static void SetGameAmount(int amountToSet) {
        _instance.gameCoinIncrement = amountToSet;
        DisplayGameAmount();
    }

    public static void IncrementGameAmount() {
        _instance.gameCoinIncrement++;
        DisplayGameAmount();
    }

    public static int GetGameCoinIncrement() {
        return _instance.gameCoinIncrement;
    }

    public static void SaveGameCoinIncrement() {
        _instance.amount += _instance.gameCoinIncrement;
        PlayerPrefs.SetInt("WalletAmount", _instance.amount);
        _instance.gameCoinIncrement = 0;
        DisplayTotalAmount();
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
    }

    private void Start() {
        FlowInterface.LoginSuccessEvent.AddListener(GetTokenBalance);
    }

    private async void GetTokenBalance() {
        amount = await FlowInterface.ExecuteGetTokens();
        PlayerPrefs.SetInt("WalletAmount", amount);
        DisplayTotalAmount();
    }


    // Display player amount to the screen.
    public static void DisplayTotalAmount() {
        _instance.walletText.text = (_instance.amount - _instance.garage.GetMoneySpent()).ToString();
    }

    // Display amount earned in current game
    public static void DisplayGameAmount() {
        _instance.walletText.text = _instance.gameCoinIncrement.ToString();
    }
}