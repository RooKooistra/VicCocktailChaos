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
        // Player.Instance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
    }

    private void Player_OnSelectedCounterChange(BaseCounter selectedCounter)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(selectedCounter == baseCounter);
        }
        
    }
}
