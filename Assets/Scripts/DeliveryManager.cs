using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public static DeliveryManager Instance;

    public event Action<List<RecipeSO>> OnRecipeSpawned;
    public event Action<List<RecipeSO>,Transform> OnRecipeCompleted;
    public event Action<Transform> OnRecipeFail;

    [SerializeField] private RecipeListSO recipeListSO;

    //scriptable object holding recipies (also scriptable objects holding kitchenSO's)
    private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();
    private float spawnRecipeTimer;
    [Tooltip("Seconds between new recipes spawned")]
    [SerializeField] private float spawnRecipeTimerMax = 4f;
    [Tooltip("Max number of recipes in waiting list")]
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
        if (!IsServer) return;

        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if(GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }

    // consider using network variables for the waitingRecipeSO list
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(waitingRecipeSOList);
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
                    int waitingRecipeSOIndex = waitingRecipeSOList.IndexOf(waitingRecipeSO); // can not parse RecipeSO through RPC so changed to int
                    DeliverCorrectRecipeServerRpc(waitingRecipeSOIndex);
                    return;
                }
            }
        }

        // no matches found
        // player did not deliver the correct recipe
        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOIndex)
    {
        successfulRecipesDelivered++;
        waitingRecipeSOList.RemoveAt(waitingRecipeSOIndex);

        OnRecipeCompleted?.Invoke(waitingRecipeSOList, transform);
        Debug.Log("Recipe Delivery Success");
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFail?.Invoke(transform);
        Debug.Log("Recipe Delivery FAIL");
    }

    public int GetSuccessfulRecipesDelivered()
    {
        return successfulRecipesDelivered;
    }

}
