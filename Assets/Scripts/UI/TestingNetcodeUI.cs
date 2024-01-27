using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] Button startHostButton;
    [SerializeField] Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() =>
        {
            GameMultiplayer.Instance.StartHost();
            Debug.Log("HOST");
            Hide();
        });

        startClientButton.onClick.AddListener(() =>
        {
            GameMultiplayer.Instance.StartClient();
            Debug.Log("CLIENT");
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
