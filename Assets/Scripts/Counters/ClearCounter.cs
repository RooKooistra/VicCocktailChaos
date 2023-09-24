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
                // player is carrying something - do nothing
            }
            else
            {
                // player not carrying anything - give to player
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
