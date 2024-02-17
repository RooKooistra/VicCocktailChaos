using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playMultiplayerButton;
    [SerializeField] private Button playSingleplayerButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        Time.timeScale = 1f;

        playMultiplayerButton.onClick.AddListener(() =>
        {
            GameMultiplayer.playMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        playSingleplayerButton.onClick.AddListener(() =>
        {
            GameMultiplayer.playMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
