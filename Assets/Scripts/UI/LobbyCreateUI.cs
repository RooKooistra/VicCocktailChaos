using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private Toggle isPrivateToggle;


    private void Awake()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyName.text, isPrivateToggle.isOn);
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
