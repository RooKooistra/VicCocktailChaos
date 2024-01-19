using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseMultiplayerUI : MonoBehaviour
{

    private void Start()
    {
        GameManager.Instance.OnMultiplayerGamePause += GameManager_OnMultiplayerGamePause;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnMultiplayerGamePause -= GameManager_OnMultiplayerGamePause;
    }

    private void GameManager_OnMultiplayerGamePause(bool isGamePaused)
    {
        gameObject.SetActive(isGamePaused);
    }
}
