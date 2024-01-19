using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPlayersUI : MonoBehaviour
{
    /// <summary>
    /// Script to show 'Waiting for Players' canvas when not last ready in multiplayer
    /// </summary>

    private void Start()
    {
        GameManager.Instance.OnLocalPLayerReadyChanged += GameManager_OnLocalPLayerReadyChanged;
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;

        Hide();
    }

    private void GameManager_OnGameStateChanged()
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
        }
    }

    private void GameManager_OnLocalPLayerReadyChanged()
    {
        if (GameManager.Instance.IsLocalPLayerReady())
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
