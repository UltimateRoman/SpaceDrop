using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI wallet;

    // Start is called before the first frame update
    void Start() {
        Wallet.SetWalletObject(wallet);
        Wallet.SetGameAmount(0);
    }

    // Update is called once per frame
    void Update() { }
}