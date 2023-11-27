using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    public event Action OnGameStateChanged;

    private enum State { WaitingToStart, CountdownToStart, GamePlaying, GameOver}
    private State state;

    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer = 5f;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {Instance.name}! {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // waiting to start will be used for multiplayer but for now just start after a timer
        state = State.WaitingToStart;
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

        Debug.Log(state);
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
}
