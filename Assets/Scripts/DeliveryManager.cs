using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;

    public event Action<List<RecipeSO>> OnRecipeSpawned;
    public event Action<List<RecipeSO>,Transform> OnRecipeCompleted;
    public event Action<Transform> OnRecipeFail;

    [SerializeField] private RecipeListSO recipeListSO;

    //scriptable object holding recipies (also scriptable objects holding kitchenSO's)
    private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();

    private float spawnRecipeTimer;
    [SerializeField] private float spawnRecipeTimerMax = 4f;
    [SerializeField] private float waitingRecipeMax = 4f;

    private int successfulRecipesDelivered = 0;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {Instance.name}! {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        spawnRecipeTimer = spawnRecipeTimerMax;
    }
    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if(waitingRecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(waitingRecipeSOList);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        foreach(RecipeSO waitingRecipeSO in waitingRecipeSOList)
        {
            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateContentsMatchesRecipe = true;
                // has the same number of ingredients so check that recipe

                foreach (var recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    // cycling through all kitchen recipes
                    if (!plateKitchenObject.GetKitchenObjectSOList().Contains(recipeKitchenObjectSO)){
                        plateContentsMatchesRecipe = false;
                        break;
                    }
                }

                if(plateContentsMatchesRecipe)
                {
                    // player delivered a correct recipe
                    successfulRecipesDelivered++;
                    waitingRecipeSOList.Remove(waitingRecipeSO);

                    OnRecipeCompleted?.Invoke(waitingRecipeSOList, transform);
                    return;
                }
            }
        }

        // no matches found
        // player did not deliver the correct recipe
        OnRecipeFail?.Invoke(transform);
    }

    public int GetSuccessfulRecipesDelivered()
    {
        return successfulRecipesDelivered;
    }

}
