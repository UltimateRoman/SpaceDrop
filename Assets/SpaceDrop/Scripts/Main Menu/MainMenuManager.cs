using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceDrop;
using TMPro;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject failedLogin;
    [SerializeField] private TextMeshProUGUI wallet;

    private void Start() {
        Wallet.SetWalletObject(wallet);
        Wallet.DisplayTotalAmount();

        if (FlowInterface.IsLoggedIn) {
            menu.SetActive(true);
            failedLogin.SetActive(false);
        }
        else {
            menu.SetActive(false);
            failedLogin.SetActive(false);
            FlowInterface.LoginSuccessEvent.AddListener(OnLoginSuccess);
            FlowInterface.LoginFailedEvent.AddListener(OnLoginFailed);
        }
    }

    private void OnLoginSuccess() {
        menu.SetActive(true);
        failedLogin.SetActive(false);
    }

    private void OnLoginFailed() {
        menu.SetActive(false);
        failedLogin.SetActive(true);
    }
}