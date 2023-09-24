using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] protected Transform countertopPoint;

    private KitchenObject kitchenObject = null;

    public abstract void Interact(Player player);

    public virtual void InteractSecond(Player player)
    {
        Debug.LogError("BaseCounter.InteractSecond();");
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return countertopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        this.kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return this.kitchenObject != null;
    }
}
