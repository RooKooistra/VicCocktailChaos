using System;
using UnityEngine;

[Serializable]
public struct KitchenObjectSO_GameObject
{
    // Used to link KitchenObjectSO to a game visual objects on plate when building menu
    // game object are turned on an off depending on which KitchenObjectSo is present on the plate

    public KitchenObjectSO kitchenObjectSO;
    public GameObject gameObject;
}
