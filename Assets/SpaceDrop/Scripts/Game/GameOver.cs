using SpaceDrop;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class GameOver : MonoBehaviour {
    public int continuePrice;

    public RocketController rocketController;
    public GameObject menu;
    public TextMeshProUGUI mintAmountText;
    public Animation notEnough;
    public LevelLoader levelLoader;
    [HideInInspector] public bool crashed;

    public static GameOver instance;

    private Animation anim;

    void Start() {
        instance = this;
        anim = this.GetComponent<Animation>();
        crashed = false;
    }

    // When player crashes to obstacle.
    public void Crashed() {
        // Used to disable pause/resume function when player crashed.
        crashed = true;
        rocketController.Crashed();
        // Play game over window open animation.
        anim.Play("Game-Over-In");
        // Disable game menu gameobject with all buttons.
        menu.SetActive(false);
        Wallet.DisplayAmount();
        mintAmountText.text = Wallet.GetGameCoinIncrement().ToString();
    }

    // If player selects continue button.
    public void Mint() {
        int mintAmount = Wallet.GetGameCoinIncrement();

        // TODO: Mint mintAmount to player crypto wallet.
        StartCoroutine(FlowInterface.MintTokens(mintAmount, () => {
                Debug.Log("Minted " + mintAmount + " tokens.");
                mintAmountText.text = "0";
                Wallet.SaveGameCoinIncrement();
            },
            () => { Debug.Log("Failed to mint tokens."); }));
    }
}