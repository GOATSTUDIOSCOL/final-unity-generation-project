using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;

public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private const float MAX_HEART_BEAT_TIME = 0.15f;
    private float currentHeartBeatTimer = 0;

    private string playerName;

    private void Awake()
    {
        playerName = "goat" + UnityEngine.Random.Range(1000, 10000);
    }
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartBeat();
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "generation";
            int maxPlayers = 4;
            CreateLobbyOptions lobbyOptions = new()
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject> {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "CaptureTheFlag")}
                }
            };
            Lobby hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);

            Debug.Log("Created lobby: " + hostLobby.Name + " " + hostLobby.MaxPlayers + " code: " + hostLobby.LobbyCode);
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException exp)
        {
            Debug.Log("Exception Message: " + exp.Message);
        }
    }

    public async void ListLobbies()
    {
        try
        {

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value);
            }

        }
        catch (LobbyServiceException exp)
        {
            Debug.Log("Exception Message: " + exp.Message);
        }
    }

    public async void HandleLobbyHeartBeat()
    {
        currentHeartBeatTimer += Time.deltaTime;
        if (currentHeartBeatTimer > MAX_HEART_BEAT_TIME)
        {
            if (hostLobby != null)
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            currentHeartBeatTimer = 0;
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new()
            {
                Player = GetPlayer(),
            };

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            Debug.Log("Joinned at lobby with code: " + lobbyCode);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException exp)
        {
            Debug.Log("Exception Message: " + exp.Message);
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            await Lobbies.Instance.QuickJoinLobbyAsync();
            Debug.Log("Quick Join Lobby");
        }
        catch (LobbyServiceException exp)
        {
            Debug.Log("Excepcion Message: " + exp.Message);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                       { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                    }
        };
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby" + lobby.Name + " " + lobby.Data["GameMode"].Value);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>{
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)},
                }
            });
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException exp)
        {
            Debug.Log("Excepcion Message: " + exp.Message);
        }
    }

}
