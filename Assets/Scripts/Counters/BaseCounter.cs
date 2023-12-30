using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    public static event Action<Transform> OnAnyObjectPlacedHere;
    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }

    [SerializeField] protected Transform countertopPoint;

    private KitchenObject kitchenObject = null;

    public abstract void Interact(Player player);

    public virtual void InteractSecond(Player player)
    {
        // Only implemented on some counters
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return countertopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        // object dropped on counter
        this.kitchenObject = kitchenObject;

        if (kitchenObject == null) return;
        OnAnyObjectPlacedHere?.Invoke(transform);
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

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
