using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartSountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;

        // hide text at start
        GameManager_OnGameStateChanged();
    }

    private void Update()
    {
        countdownText.text = Mathf.Ceil(GameManager.Instance.GetCountdownToStartTimer()).ToString();
    }

    private void GameManager_OnGameStateChanged()
    {
        gameObject.SetActive(GameManager.Instance.IsCountdownToStartActive());
    }
}
