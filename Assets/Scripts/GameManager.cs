using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }


    public event Action OnGameStateChanged;
    public event Action<bool> OnGameTogglePause;

    private enum State { WaitingToStart, CountdownToStart, GamePlaying, GameOver}
    private State state;

    // game states change with timer for testing
    [SerializeField] private float waitingToStartTimer = 1f;
    [SerializeField] private float countdownToStartTimer = 3f;
    [SerializeField] private float gamePlayingTimerMax = 20f;
    private float gamePlayingTimer;
    private bool isGamePaused = false;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {Instance.name}! {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // waiting to start will be used for multiplayer in the future
        state = State.WaitingToStart;
    }

    private void Start()
    {
        GameInput.Instance.OnPausedAction += GameInput_OnPausedAction;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPausedAction -= GameInput_OnPausedAction;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer > 0f) return;

                state = State.CountdownToStart;
                OnGameStateChanged?.Invoke();
                break;

            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer > 0f) return;

                state = State.GamePlaying;
                gamePlayingTimer = gamePlayingTimerMax; // set the gameplay timer
                OnGameStateChanged?.Invoke();
                break;

            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer > 0f) return;

                state = State.GameOver;
                OnGameStateChanged?.Invoke();
                break;

            case State.GameOver:
                break;
        }

        //Debug.Log(state);
    }

    private void GameInput_OnPausedAction()
    {
        TogglePauseGame();
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused? 0f : 1f;
        OnGameTogglePause?.Invoke(isGamePaused);
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public bool IsGameOverActive()
    {
        return state == State.GameOver;
    }

    public float GetGameplayTimerNormalized()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }
}
