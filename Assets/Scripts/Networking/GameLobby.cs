using Mono.CSharp.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameLobby : MonoBehaviour
{
    public static GameLobby Instance { get; private set; }

    public event Action<string> OnActionStarted;
    public event Action<string> OnActionFailed;
    public event Action<List<Lobby>> OnLobbyListChanged;

    [SerializeField] private float heartBeatTimerMax = 15f;
    [SerializeField] private float listLobbiesTimerMax = 3f;

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;

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

        InitializeUnityAuthentication();
    }


    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicListLobbies();
    }

    private void HandlePeriodicListLobbies()
    {
        if (joinedLobby != null && !AuthenticationService.Instance.IsSignedIn) return;

        listLobbiesTimer -= Time.deltaTime;
        if(listLobbiesTimer <= 0f)
        {
            listLobbiesTimer = listLobbiesTimerMax;
            ListLobbies();
        }
    }

    private void HandleHeartbeat()
    {
        if (!IsLobbyHost()) return;

        heartbeatTimer -= Time.deltaTime;

        if(heartbeatTimer <= 0f)
        {
            heartbeatTimer = heartBeatTimerMax;
            LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized) return;

        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(UnityEngine.Random.Range(0,10000).ToString());

        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(queryResponse.Results);

        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        string lobbytype = isPrivate ? "PRIVATE" : "PUBLIC";

        if(lobbyName == "")
        {
            OnActionFailed?.Invoke($"FAILED TO CREATE {lobbytype} LOBBY. NO NAME WAS ENTERED!");
            return;
        }

        OnActionStarted?.Invoke($"CREATING {lobbytype} LOBBY ({lobbyName})...");

        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions { IsPrivate = isPrivate });

            GameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);

        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnActionFailed?.Invoke($"FAILED TO CREATE {lobbytype} LOBBY!");
        }
        
    }

    public async void QuickJoin()
    {
        OnActionStarted?.Invoke("JOINING LOBBY...");

        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            GameMultiplayer.Instance.StartClient();

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnActionFailed?.Invoke("COULD NOT FIND A LOBBY TO QUICK JOIN!");
        }
        
    }

    public async void JoinWithId(string lobbyId, string lobbyName = "")
    {
        OnActionStarted?.Invoke($"JOINING LOBBY ({lobbyName})... ");

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            GameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnActionFailed?.Invoke($"FAILED TO JOIN LOBBY ({lobbyName})!");
        }

    }

    public async void JoinWithCode(string lobbyCode)
    {
        lobbyCode = lobbyCode.ToUpper();
        if(lobbyCode == "")
        {
            OnActionFailed?.Invoke("FAILED TO JOIN LOBBY. NO CODE WAS ENTERED!");
            return;
        }

        OnActionStarted?.Invoke($"JOINING LOBBY ({lobbyCode})... ");

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            GameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnActionFailed?.Invoke($"FAILED TO JOIN LOBBY ({lobbyCode})!");
        }
        
    }

    public async void DeleteLobby()
    {
        if (joinedLobby == null) return;

        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            joinedLobby = null;
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public async void LeaveLobby()
    {
        if (joinedLobby == null) return;

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public async void KickPlayer(string playerId)
    {
        if (!IsLobbyHost()) return;

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
