using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // there is no kitchen object on counter
            if(player.HasKitchenObject() && kitchenObjectSO != null)
            {
                // player is carrying something - transfer kitchen object to counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
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
                if( player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player holding a plate - add ingredient to plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //GetKitchenObject().DestroySelf(); obsolete with mulyiplayer
                    }
                }
                else
                {
                    // player is not carrying a plate but something else
                    if(GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObjectHeld)) // check COUNTER for plate
                    {
                        // counter has a plate on top
                        if (plateKitchenObjectHeld.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) // try add ingredient to PLAYER
                        {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                            // player.GetKitchenObject().DestroySelf(); obsolete with multiplayer
                        }
                    }
                }

            }
            else
            {
                // player not carrying anything - give to player
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
