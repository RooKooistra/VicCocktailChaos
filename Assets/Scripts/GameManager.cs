using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance {  get; private set; }

    public event Action OnGameStateChanged;
    public event Action<bool> OnLocalToggleGamePause;
    public event Action<bool> OnMultiplayerGamePause;
    public event Action OnLocalPLayerReadyChanged;

    // game states change with timer for testing
    [SerializeField] private float gamePlayingTimerMax = 20f;
    [SerializeField] private float countdownToStartDuration = 3f;


    private enum State { WaitingToStart, CountdownToStart, GamePlaying, GameOver}
    private bool isLocalPlayerReady;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

    private Dictionary<ulong, bool> playerReadyDictionary = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> playerPausedDictionary = new Dictionary<ulong, bool>();

    private NetworkVariable<float> gamePlayingTimer;
    private NetworkVariable<float> countdownToStartTimer;
    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool AutoTestPauseGameState = false; // used to delay for one frame for Server to update Client ID list




    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {Instance.name}! {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gamePlayingTimer = new NetworkVariable<float>(gamePlayingTimerMax);
        countdownToStartTimer = new NetworkVariable<float>(countdownToStartDuration);
    }

    private void Start()
    {
        GameInput.Instance.OnPausedAction += GameInput_OnPausedAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    
    public override void OnDestroy()
    {
        GameInput.Instance.OnPausedAction -= GameInput_OnPausedAction;
        GameInput.Instance.OnInteractAction -= GameInput_OnInteractAction;
    }
    

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    
    public override void OnNetworkDespawn()
    {
        state.OnValueChanged -= State_OnValueChanged;
        isGamePaused.OnValueChanged -= IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }
    

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        AutoTestPauseGameState = true;
    }


    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        Time.timeScale = isGamePaused.Value ? 0f : 1f;

        OnMultiplayerGamePause?.Invoke(isGamePaused.Value);
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnGameStateChanged?.Invoke();
    }

    private void GameInput_OnInteractAction()
    {
        if(state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;

            OnLocalPLayerReadyChanged?.Invoke();

            SetPlayerReadyServerRpc();
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        switch (state.Value)
        {
            case State.WaitingToStart:

                break;

            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value > 0f) return;

                state.Value = State.GamePlaying;
                gamePlayingTimer.Value = gamePlayingTimerMax; // set the gameplay timer
                
                break;

            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value > 0f) return;

                state.Value = State.GameOver;

                break;

            case State.GameOver:
                break;
        }

        //Debug.Log(state);
    }

    private void LateUpdate()
    {
        if (AutoTestPauseGameState)
        {
            AutoTestPauseGameState = false;
            TestGamePauseState();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) // dictionary does not contain the client ID or does contain but is false
            {
                // This player is NOT ready
                allClientsReady = false;
                break;
            }
        }

        if(allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }
    }

    private void GameInput_OnPausedAction()
    {
        TogglePauseGame();
    }

    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        // Time.timeScale = isLocalGamePaused? 0f : 1f; moved to server event

        TogglePauseGameServerRpc(isLocalGamePaused);

        OnLocalToggleGamePause?.Invoke(isLocalGamePaused);
    }

    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOverActive()
    {
        return state.Value == State.GameOver;
    }

    public float GetGameplayTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }

    public bool IsLocalPLayerReady()
    {
        return isLocalPlayerReady;
    }

    [ServerRpc(RequireOwnership = false)]
    private void TogglePauseGameServerRpc(bool isGamePaused, ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = isGamePaused;

        TestGamePauseState();
    }

    private void TestGamePauseState()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                // this player is paused
                this.isGamePaused.Value = true;
                return;
            }
        }

        this.isGamePaused.Value = false;
    }

        
}
