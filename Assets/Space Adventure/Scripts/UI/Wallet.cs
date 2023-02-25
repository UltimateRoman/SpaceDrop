using System;
using StarDrop;
using UnityEngine;
using TMPro;

public class Wallet : MonoBehaviour {
    private static Wallet _instance;

    private int amount;
    private int gameCoinIncrement = 0;
    private TextMeshProUGUI walletText;

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
    }

    void Start() {
        amount = PlayerPrefs.GetInt("WalletAmount", 0);
        
        walletText = this.GetComponent<TextMeshProUGUI>();
        DisplayAmount();
    }

    // Get current player wallet amount.
    public static int GetAmount() {
        return _instance.amount;
    }

    // Set player amount to custom value.
    public static void SetAmount(int amountToSet) {
        _instance.amount = amountToSet;
        DisplayAmount();
        PlayerPrefs.SetInt("WalletAmount", _instance.amount);
    }

    public static void IncrementAmount() {
        _instance.gameCoinIncrement++;
    }

    public static int GetGameCoinIncrement() {
        return _instance.gameCoinIncrement;
    }

    public static void SaveGameCoinIncrement() {
        _instance.amount += _instance.gameCoinIncrement;
        DisplayAmount();
        PlayerPrefs.SetInt("WalletAmount", _instance.amount);
        _instance.gameCoinIncrement = 0;
    }

    // Display player amount to the screen.
    private static void DisplayAmount() {
        _instance.walletText.text = _instance.amount.ToString();
    }
}