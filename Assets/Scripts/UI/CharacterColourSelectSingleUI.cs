using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColourSelectSingleUI : MonoBehaviour
{
    /// <summary>
    /// script for changing player colour on the network. Use this as a base for changing names, character select etc
    /// </summary>

    [SerializeField] private int colourId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameMultiplayer.Instance.ChangePlayerColour(colourId);
        });
    }

    private void Start()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged += GameMultiplayer_OnPlayerDataNetworkListChanged;
    }

    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= GameMultiplayer_OnPlayerDataNetworkListChanged;
    }

    private void GameMultiplayer_OnPlayerDataNetworkListChanged()
    {
        UpdateIsSelected();
    }

    public void SetUpButton(Color playerColour, int colourId)
    {
        this.colourId = colourId;
        image.color = playerColour;

        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        selectedGameObject.SetActive(GameMultiplayer.Instance.GetPlayerData().colourId == this.colourId);
    }
}
