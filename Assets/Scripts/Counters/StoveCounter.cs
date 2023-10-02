using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{

    public event Action<State> OnStateChanged;
    public event Action<float,bool> OnProgressChanged;

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    public enum State { IDLE, FRYING, FRIED, BURNT}
    private State state;

    private float fryingTimer = 0;
    private float burningTimer = 0;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;


    private void Start()
    {
        state = State.IDLE;
    }
    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.IDLE:

                    break;

                case State.FRYING:
                    fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke((fryingTimer / fryingRecipeSO.fryingTimerMax), false);

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                        // kitchen object has been fried

                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        burningTimer = 0f;
                        state = State.FRIED;

                        OnStateChanged?.Invoke(state);
                    }
                    break;

                case State.FRIED:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke((burningTimer / burningRecipeSO.burningTimerMax), true);

                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {
                        // kitchen object has been fried

                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.BURNT;

                        OnProgressChanged?.Invoke(0f, false);
                        OnStateChanged?.Invoke(state);
                    }
                    break;

                case State.BURNT:

                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // there is no kitchen object on counter
            if (player.HasKitchenObject())
            {
                // player is carrying something - transfer kitchen object to counter
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // player carrying something that can be fried... might remove this so non fryable objects can still be placed and ruined
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    fryingTimer = 0f;
                    OnProgressChanged?.Invoke((fryingTimer / fryingRecipeSO.fryingTimerMax), false);

                    state = State.FRYING;

                    OnStateChanged?.Invoke(state);
                }
            }
            else
            {
                // player not carrying anything - do nothing
            }
        }
        else
        {
            // there is a kitchen object on counter
            if (player.HasKitchenObject())
            {
                // player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player holding a plate - add ingredient to plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();

                        // reset state machine
                        state = State.IDLE;

                        OnProgressChanged?.Invoke(0f, false);
                        OnStateChanged?.Invoke(state);
                    }
                }
            }
            else
            {
                // player not carrying anything - give to player
                GetKitchenObject().SetKitchenObjectParent(player);

                // reset state machine
                state = State.IDLE;

                OnProgressChanged?.Invoke(0f, false);
                OnStateChanged?.Invoke(state);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        return GetFryingRecipeSOWithInput(kitchenObjectSO) != null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }

        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }

        return null;
    }

}
