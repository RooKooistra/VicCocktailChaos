using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayTimerUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;

        // hide text at start
        GameManager_OnGameStateChanged();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Update()
    {
        timerImage.fillAmount = GameManager.Instance.GetGameplayTimerNormalized();
    }

    private void GameManager_OnGameStateChanged()
    {
        gameObject.SetActive(GameManager.Instance.IsGamePlaying());
    }
}
