using System;
using DapperLabs.Flow.Sdk;
using DapperLabs.Flow.Sdk.Cadence;
using DapperLabs.Flow.Sdk.DataObjects;
using DapperLabs.Flow.Sdk.Unity;
using DapperLabs.Flow.Sdk.DevWallet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceDrop {
    public class FlowInterface : MonoBehaviour {
        private static FlowInterface _instance;

        [Header("FLOW Account")] public FlowControl.Account FLOW_ACCOUNT = null;

        [Header("Scripts")] [SerializeField] TextAsset GetTokensScript;
        [SerializeField] TextAsset GetNFTsScript;

        public GameObject ScriptsResult;

        [Header("Transactions")] [SerializeField]
        TextAsset loginTxn;

        [SerializeField] TextAsset mintTokenTxn;
        [SerializeField] TextAsset mintNFTTxn;

        public GameObject TxResultId;
        public GameObject TxResultScript;
        public GameObject TxResultRefBlockId;
        public GameObject TxResultGasLimit;
        public GameObject TxResultArgs;
        public GameObject TxResultAuthorizers;
        public GameObject TxResultPayloadSigs;
        public GameObject TxResultEnvelopeSigs;
        public GameObject TxResultPayer;
        public GameObject TxResultProposer;
        public GameObject TxResultStatus;
        public GameObject TxResultStatusCode;
        public GameObject TxResultError;
        public GameObject TxResultEvents;

        private string _flowAddress;

        public static UnityEvent LoginSuccessEvent { get; private set; }
        public static UnityEvent LoginFailedEvent { get; private set; }
        public static bool IsLoggedIn { get; private set; }

        private void Awake() {
            if (_instance == null) {
                _instance = this;
                IsLoggedIn = false;
                LoginSuccessEvent = new UnityEvent();
                LoginFailedEvent = new UnityEvent();
                DontDestroyOnLoad(this);
            }
            else {
                Destroy(this);
            }
        }

        private void Start() {
            // Initialize FlowSDK. You must pass in a FlowConfig object which specifies which protocol
            // and Url to connect to (only HTTP is currently supported). 
            // By default, connects to the emulator on localhost. 

            FlowConfig flowConfig = new FlowConfig();

            flowConfig.NetworkUrl =
                FlowControl.Data.EmulatorSettings.emulatorEndpoint ?? "http://localhost:8888/v1"; // local emulator
            //flowConfig.NetworkUrl = "https://rest-testnet.onflow.org/v1";  // testnet
            //flowConfig.NetworkUrl = "https://rest-mainnet.onflow.org/v1";  // mainnet

            flowConfig.Protocol = FlowConfig.NetworkProtocol.HTTP;
            FlowSDK.Init(flowConfig);
            FlowSDK.RegisterWalletProvider(ScriptableObject.CreateInstance<DevWalletProvider>());

            // FlowSDK.GetWalletProvider().Authenticate("", (string authAccount) =>
            //     {
            //         Debug.Log($"Authenticated: {authAccount}");
            //     }, () =>
            //     {
            //         Debug.Log("Authentication failed, aborting transaction.");
            //     });

            // Login to FLOW account
            Login(() => {
                Debug.Log("Login successful!");
                LoginSuccessEvent.Invoke();
                IsLoggedIn = true;
            }, () => {
                Debug.Log("Login failed!");
                LoginFailedEvent.Invoke();
                IsLoggedIn = false;
            });
        }


        private IEnumerator OnAuthSuccess(string flowAddress, System.Action onSuccessCallback,
            System.Action onFailureCallback) {
            _instance._flowAddress = flowAddress;

            // get flow account
            FLOW_ACCOUNT = FlowControl.Data.Accounts.FirstOrDefault(x => x.AccountConfig["Address"] == flowAddress);

            // execute log in transaction on chain
            Task<FlowTransactionResult> task = FLOW_ACCOUNT.SubmitAndWaitUntilExecuted(loginTxn.text);
            while (!task.IsCompleted) {
                // int dots = ((int)(Time.time * 2.0f) % 4);
                // UIManager.Instance.SetStatus("Connecting" + new string('.', dots));
                yield return null;
            }

            // check for error. if there was an error, break.
            if (task.Result.Error != null || task.Result.ErrorMessage != string.Empty ||
                task.Result.Status == FlowTransactionStatus.EXPIRED) {
                onFailureCallback();
                yield break;
            }

            // login successful!
            onSuccessCallback();
        }

        /// <summary>
        /// Clear the FLOW account object
        /// </summary>
        internal static void Logout() {
            _instance.FLOW_ACCOUNT = null;
            FlowSDK.GetWalletProvider().Unauthenticate();
        }

        public static void Login(System.Action onSuccessCallback, System.Action onFailureCallback) {
            // Authenticate an account with DevWallet
            FlowSDK.GetWalletProvider().Authenticate(
                "", // blank string will show list of accounts from Accounts tab of Flow Control Window
                (string flowAddress) =>
                    _instance.StartCoroutine(_instance.OnAuthSuccess(flowAddress, onSuccessCallback,
                        onFailureCallback)),
                onFailureCallback);
        }

        /// <summary>
        /// Demonstrates calling the Scripts.ExecuteAtLatestBlock() API with a script that is loaded from file (*.cdc). 
        /// The script takes a cadence Address argument and returns a cadence UFix64 value. 
        /// </summary>
        public static async Task<int> ExecuteGetTokens() {
            string script = _instance.GetTokensScript.text;

            FlowScriptRequest scriptReq = new FlowScriptRequest {
                Script = script
            };

            scriptReq.AddArgument(new CadenceAddress(_instance._flowAddress));

            FlowScriptResponse response = await Scripts.ExecuteAtLatestBlock(scriptReq);

            if (response.Error != null) {
                Debug.LogError(response.Error.Message);
                return 0;
            }

            CadenceNumber responseVal = (CadenceNumber)response.Value;
            return (int)float.Parse(responseVal.Value);
        }

        /// <summary>
        /// Demonstrates calling the Scripts.ExecuteAtLatestBlock() API with a script that is loaded from file (*.cdc). 
        /// The script takes a cadence Address argument and returns a list of UInt64 value. 
        /// </summary>
        public static async void ExecutGetNFTs(string userAddress) {
            string script = _instance.GetNFTsScript.text;

            FlowScriptRequest scriptReq = new FlowScriptRequest {
                Script = script
            };

            scriptReq.AddArgument(new CadenceAddress(userAddress));

            FlowScriptResponse response = await Scripts.ExecuteAtLatestBlock(scriptReq);

            if (response.Error != null) {
                Debug.LogError(response.Error.Message);
                return;
            }

            CadenceArray responseVal = (CadenceArray)response.Value;
        }

        public static IEnumerator MintTokens(int amount, System.Action onSuccessCallback,
            System.Action onFailureCallback) {
            // // get flow account
            // _instance.FLOW_ACCOUNT =
            //     FlowControl.Data.Accounts.FirstOrDefault(x => x.AccountConfig["Address"] == flowAddress);

            // execute log in transaction on chain
            Task<FlowTransactionResult> task = _instance.FLOW_ACCOUNT.SubmitAndWaitUntilExecuted(
                _instance.mintTokenTxn.text,
                new CadenceAddress(_instance._flowAddress),
                new CadenceNumber(CadenceNumberType.UFix64, amount.ToString("F1"))
            );

            while (!task.IsCompleted) {
                yield return null;
            }

            // check for error. if there was an error, break.
            if (task.Result.Error != null || task.Result.ErrorMessage != string.Empty ||
                task.Result.Status == FlowTransactionStatus.EXPIRED) {
                onFailureCallback();
                yield break;
            }

            // login successful!
            onSuccessCallback();
        }

        public static IEnumerator MintNFT(string flowAddress, System.Action onSuccessCallback,
            System.Action onFailureCallback) {
            // get flow account
            _instance.FLOW_ACCOUNT =
                FlowControl.Data.Accounts.FirstOrDefault(x => x.AccountConfig["Address"] == flowAddress);

            // execute log in transaction on chain
            Task<FlowTransactionResult> task = _instance.FLOW_ACCOUNT.SubmitAndWaitUntilExecuted(
                _instance.mintNFTTxn.text,
                new CadenceAddress(flowAddress)
            );

            while (!task.IsCompleted) {
                yield return null;
            }

            // check for error. if there was an error, break.
            if (task.Result.Error != null || task.Result.ErrorMessage != string.Empty ||
                task.Result.Status == FlowTransactionStatus.EXPIRED) {
                onFailureCallback();
                yield break;
            }

            // login successful!
            onSuccessCallback();
        }
    }
}