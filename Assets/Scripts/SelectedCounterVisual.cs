using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] ClearCounter clearCounter;
    [SerializeField] GameObject visualGameObject;
    private void Start()
    {
        Player.Instance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
    }

    private void Player_OnSelectedCounterChange(ClearCounter selectedCounter)
    {
        visualGameObject.SetActive(selectedCounter == clearCounter);
    }
}
