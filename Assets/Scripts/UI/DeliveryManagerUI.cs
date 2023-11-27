using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplatePrefab;

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
    }

    private void DeliveryManager_OnRecipeSpawned(List<RecipeSO> waitingRecipeSOList)
    {
        UpdateVisual(waitingRecipeSOList);
    }

    private void DeliveryManager_OnRecipeCompleted(List<RecipeSO> waitingRecipeSOList, Transform deliveryManagerTransform)
    {
        UpdateVisual(waitingRecipeSOList);
    }

    private void UpdateVisual(List<RecipeSO> waitingRecipeSOList)
    {
        // clean up existing icons so there are no double ups
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        foreach(var recipeSO in waitingRecipeSOList)
        {
            Transform recipeTransform = Instantiate(recipeTemplatePrefab, container);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }
}
