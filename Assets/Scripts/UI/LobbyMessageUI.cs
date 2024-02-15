using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);

        //GameMultiplayer.Instance.OnTryingToJoinGame += GameMultiplayer_OnTryingToJoinGame;
        GameMultiplayer.Instance.OnFailedToJoinGame += GameMultiplayer_OnFailedToJoinGame;
        GameLobby.Instance.OnActionStarted += GameLobby_OnActionStarted;
        GameLobby.Instance.OnActionFailed += GameLobby_OnActionFailed;

        Hide();
    }

    private void GameLobby_OnActionFailed(string message)
    {
        ShowMessage(message);
    }

    private void GameLobby_OnActionStarted(string message)
    {
        ShowMessage(message);
    }

    private void GameMultiplayer_OnFailedToJoinGame()
    {
        string message = NetworkManager.Singleton.DisconnectReason == "" ? "FAILED TO CONNECT" : NetworkManager.Singleton.DisconnectReason;
        ShowMessage(message);

        Show();
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        //GameMultiplayer.Instance.OnTryingToJoinGame -= GameMultiplayer_OnTryingToJoinGame;
        GameMultiplayer.Instance.OnFailedToJoinGame -= GameMultiplayer_OnFailedToJoinGame;
    }
}
