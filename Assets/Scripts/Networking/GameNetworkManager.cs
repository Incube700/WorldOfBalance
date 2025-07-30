using UnityEngine;
using Mirror;

public class GameNetworkManager : NetworkManager
{
    [Header("Game Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    [Header("Network Settings")]
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private bool autoStartServer = false;
    
    private int currentSpawnIndex = 0;
    
    void Start()
    {
        if (autoStartServer)
        {
            StartServer();
        }
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server started");
    }
    
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("Server stopped");
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Client started");
    }
    
    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client stopped");
    }
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Find spawn point
        Transform spawnPoint = GetNextSpawnPoint();
        
        // Create player
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Add player to server
        NetworkServer.AddPlayerForConnection(conn, player);
        
        Debug.Log($"Player spawned at {spawnPoint.position}");
    }
    
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("Player disconnected");
    }
    
    private Transform GetNextSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            // Create default spawn point
            GameObject defaultSpawn = new GameObject("DefaultSpawn");
            defaultSpawn.transform.position = Vector3.zero;
            return defaultSpawn.transform;
        }
        
        Transform spawnPoint = spawnPoints[currentSpawnIndex];
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;
        
        return spawnPoint;
    }
    
    public void StartHost()
    {
        StartHost();
        Debug.Log("Host started");
    }
    
    public void StartClient()
    {
        StartClient();
        Debug.Log("Client started");
    }
    
    public void StartServer()
    {
        StartServer();
        Debug.Log("Server started");
    }
    
    public void StopNetwork()
    {
        StopHost();
        Debug.Log("Network stopped");
    }
} 