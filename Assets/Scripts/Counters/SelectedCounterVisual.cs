using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    /// <summary>
    ///  Simple script to see if this interactable is currently one selected then setactive the selected item visual
    /// </summary>
    
    [SerializeField] BaseCounter baseCounter;

    private void Start()
    {
        if(Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
    }

    private void Player_OnAnyPlayerSpawned()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChange -= Player_OnSelectedCounterChange; // to ensure only a single listener is subscribed
            Player.LocalInstance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
        }
    }

    private void Player_OnSelectedCounterChange(BaseCounter selectedCounter)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(selectedCounter == baseCounter);
        }
        
    }
}
