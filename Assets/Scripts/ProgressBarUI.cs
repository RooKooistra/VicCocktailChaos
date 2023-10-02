using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;

    private IHasProgress hasProgress;

    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();

        if(hasProgress == null )
        {
            Debug.LogError($"Game Object {hasProgressGameObject} does not have the interface IHasProgress implemented");
        }

        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;

        barImage.fillAmount = 0;
        gameObject.SetActive(false);
    }

    private void HasProgress_OnProgressChanged(float progressBarNormalized, bool isUrgent)
    {
        barImage.fillAmount = progressBarNormalized;
        barImage.color = isUrgent ? Color.magenta : Color.cyan;

        gameObject.SetActive(!(progressBarNormalized == 0f || progressBarNormalized == 1f));
    }
}
