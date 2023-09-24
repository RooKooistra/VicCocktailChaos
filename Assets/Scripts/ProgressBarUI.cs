using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] CuttingCounter cuttingCounter;
    [SerializeField] private Image barImage;

    private void Start()
    {
        cuttingCounter.OnProgressChanged += CuttingCounter_OnProgressChanged;

        barImage.fillAmount = 0;
        gameObject.SetActive(false);
    }

    private void CuttingCounter_OnProgressChanged(float progressBarNormalized)
    {
        barImage.fillAmount = progressBarNormalized;

        gameObject.SetActive(!(progressBarNormalized == 0f || progressBarNormalized == 1f));
    }
}
