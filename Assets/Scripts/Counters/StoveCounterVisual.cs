using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject StoveOnGameObject;
    [SerializeField] private GameObject ParticlesGameObject;

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void OnDisable()
    {
        stoveCounter.OnStateChanged -= StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(StoveCounter.State state)
    {
        bool showVisual =  (state == StoveCounter.State.FRYING || state == StoveCounter.State.FRIED);
        StoveOnGameObject.SetActive(showVisual);
        ParticlesGameObject.SetActive(showVisual);
    }
}
