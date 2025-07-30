using UnityEngine;
using Mirror;

public class SimpleNetworkManager : NetworkManager
{
    public static new SimpleNetworkManager singleton { get; private set; }

    void Awake()
    {
        // Убеждаемся, что это единственный NetworkManager
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Автоматически запускаем как Host для локальной игры
        if (!NetworkServer.active && !NetworkClient.active)
        {
            StartHost();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Сервер запущен!");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Клиент подключен!");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"Игрок {conn.connectionId} присоединился!");
    }
} 