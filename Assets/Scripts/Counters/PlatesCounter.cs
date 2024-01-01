using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event Action OnPlateSpawned;
    public event Action OnPlateRemoved;

    [SerializeField] private float spawnPlateTimerMax = 4f;
    [SerializeField] private float platesSpawnedAmountMax = 4;
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer = 0f;
    private int platesSpawnedAmount = 0;


    private void Update()
    {
        if(!IsServer) return;

        spawnPlateTimer += Time.deltaTime;

        if(spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if(GameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
            {
                SpawnPlateClientRpc();
            }
        }
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnedAmount++;

        OnPlateSpawned?.Invoke();
    }
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject()) // player is not carrying anything
        {
            if(platesSpawnedAmount > 0)
            {
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                InteractLogicServerRpc();
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        platesSpawnedAmount--;

        OnPlateRemoved?.Invoke();
    }
}

