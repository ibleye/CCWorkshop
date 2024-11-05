using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using System.Linq;
using FishNet;
using TMPro;
using System.Threading.Tasks;
using System;
using FishNet.Managing;
using FishNet.Transporting;
#if UNITY_EDITOR
using ParrelSync;
#endif

//temporary setup to test networking

public class SimpleMatchmaker : MonoBehaviour
{
    [SerializeField] private GameObject _buttons;

    private Lobby _connectedLobby;
    private QueryResponse _lobbies;
    private Transport _transport;
    private const string JoinCodeKey = "j";
    private string _playerId;
    /**
    private void Awake() => _transport = FindObjectOfType<Transport>();

    public async void CreateOrJoinLobby()
    {
        await Authenticate();

        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();

        if (_connectedLobby != null) _buttons.SetActive(false);
    }

    public async Task Authenticate()
    {
        var options = new InitializationOptions();

#if UNITY_EDITOR
        options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _playerId = AuthenticationService.Instance.PlayerId;
    }

    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            SetTransformAsClient(a);

            NetworkManager.Singleton.StartClient();
            return lobby;
        }catch(Exception e)
        {
            Debug.Log($"No lobbies avalable via quick join");
            return null;
        }
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            const int maxPlayers = 100;

            var a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };
            var lobby = await Lobbies.Instance.CreateLobbyAsync("Useless Lobby Name", maxPlayers, options);

            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

            NetworkManager.Singleton.StartHost();
            return lobby;
        }
        catch(Exception e)
        {
            Debug.LogFormat("Failed creating a lobby");
            return null;
        }
    }

    private void SetTransformAsClient(JoinAllocation a)
    {
        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }

    private static IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private void OnDestroy()
    {
        try
        {
            StopAllCoroutines();
            if(_connectedLobby != null)
            {
                if (_connectedLobby.HostId == _playerId) Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                else Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
            }
        }catch(Exception e)
        {
            Debug.Log($"Error shutting down lobby: {e}");
        }
    }
    **/
}
