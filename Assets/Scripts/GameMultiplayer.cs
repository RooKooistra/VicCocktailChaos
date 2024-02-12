using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMultiplayer : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;
    public static GameMultiplayer Instance { get; private set; }

    public event Action OnTryingToJoinGame;
    public event Action OnFailedToJoinGame;
    public event Action OnPlayerDataNetworkListChanged; // for character select player scene

    [SerializeField] KitchenObjectListSO kitchenObjectListSO;
    [SerializeField] private List<Color> playerColourList = new List<Color>();

    private NetworkList<PlayerData> playerDataNetworkList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {Instance.name}! {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NewtworkManager_Server_OnClientDisconnectCallback;

        NetworkManager.Singleton.StartHost();
    }

    private void NewtworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for(int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if(playerData.clientId == clientId)
            {
                // disconnected
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong newConnectedClientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = newConnectedClientId,
            colourId = GetFirstUnusedColourId(),
        });
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if(SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            response.Approved = false;
            response.Reason = "GAME HAS ALREADY STARTED";
            return;
        }

        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            response.Approved = false;
            response.Reason = "GAME IS FULL";
            return;
        }

        response.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke();

        NetworkManager.Singleton.OnClientDisconnectCallback += NewtworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NewtworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke();
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        // need to parse an int and networkobjectreference as the kitchenobjectso and ikitchenobjectparent are not serializable
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        Transform kitchenObjectTransform = Instantiate(GetKitchenObjectSOFromIndex(kitchenObjectSOIndex).prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        ClearKitchenObjectParentClientRpc(kitchenObjectNetworkObjectReference);

        GetKitchenObjectFromNetworkObjectReference(kitchenObjectNetworkObjectReference).DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        GetKitchenObjectFromNetworkObjectReference(kitchenObjectNetworkObjectReference).ClearKitchenObjectOnParent();
    }

    private KitchenObject GetKitchenObjectFromNetworkObjectReference(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        return kitchenObjectNetworkObject.GetComponent<KitchenObject>();
    }

    public bool IsPlayerConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public PlayerData GetPlayerDataFromClientID(ulong clientID)
    {
        foreach (var playerData in playerDataNetworkList)
        {
            if(playerData.clientId == clientID)
                return playerData;
        }
        return default;
    }

    public int GetPlayerDataIndexFromClientID(ulong clientID)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientID)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientID(NetworkManager.Singleton.LocalClientId);
    }

    public Color GetPlayerColor(int colorId)
    {
        return playerColourList[colorId];
    }

    public List<Color> GetPlayerColorList()
    {
        return playerColourList;
    }

    public void ChangePlayerColour(int colourId)
    {
        ChangePlayerColourServerRpc(colourId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColourServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if(!IsColourAvailable(colorId))
        {
            // not available
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientID(serverRpcParams.Receive.SenderClientId);

        // because this is a server serialised struct you need to grab it, modify it, then upload/save the changes
        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.colourId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColourAvailable(int colourId)
    {
        foreach(var playerData in playerDataNetworkList)
        {
            if (playerData.colourId == colourId)
            {
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColourId()
    {
        for(int i = 0; i < playerColourList.Count; i++) 
        {
            if (IsColourAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);

        // callback does not get called when kicking player so needs to be triggered manually
        NewtworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
