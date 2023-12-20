using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class GamePausedUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    [Header ("Audio Components")]
    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private TextMeshProUGUI soundEffectButtonText;
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI musicButtonText;

    [Header("Bindings Components")]
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAltButton;
    [SerializeField] private Button gamepadPauseButton;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAltText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;
    [SerializeField] private Transform pressToRebindTransform;
    [SerializeField] private TextMeshProUGUI bindingText;


    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        soundEffectsButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.ChangeSFXVolume();
            UpdateVisual();
        });

        musicButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.ChangeMusicVolume();
            UpdateVisual();
        });

        moveUpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveUp); });
        moveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveDown); });
        moveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveLeft); });
        moveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveRight); });
        interactButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
        interactAltButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.InteractAlt); });
        pauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });
        gamepadInteractButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.GamepadInteract); });
        gamepadInteractAltButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.GamepadInteractAlt); });
        gamepadPauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.GamepadPause); });
    }
    private void Start()
    {
        GameManager.Instance.OnGameTogglePause += GameManager_OnGameTogglePause;
        UpdateVisual();

        HidePressToRebindKey();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameTogglePause -= GameManager_OnGameTogglePause;
    }

    private void GameManager_OnGameTogglePause(bool isGamePaused)
    {
        gameObject.SetActive(isGamePaused);
        if (isGamePaused) resumeButton.Select();
    }

    private void UpdateVisual()
    {
        soundEffectButtonText.text = $"SOUND EFFECTS: {Mathf.Round(AudioManager.Instance.GetSFXVolume() * 10f)}";
        musicButtonText.text = $"MUSIC: {Mathf.Round(AudioManager.Instance.GetMusicVolume() * 10f)}";

        // update visuals for binding data
        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        gamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteract);
        gamepadInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteractAlt);
        gamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadPause);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKeyTransform();
        bindingText.text = binding.ToString();

        GameInput.Instance.RebindBinding(binding, () =>
        {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }

    private void ShowPressToRebindKeyTransform()
    {
        pressToRebindTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        pressToRebindTransform.gameObject.SetActive(false);
    }
}
