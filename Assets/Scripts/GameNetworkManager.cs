using UnityEngine;
using Mirror;

public class GameNetworkManager : NetworkManager
{
    public static new GameNetworkManager singleton => (GameNetworkManager)NetworkManager.singleton;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server started!");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("Server stopped!");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Client started!");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client stopped!");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"Player {conn.connectionId} joined!");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log($"Player {conn.connectionId} disconnected!");
    }
} 