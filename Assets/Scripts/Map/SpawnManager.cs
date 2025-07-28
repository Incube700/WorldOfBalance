using UnityEngine;
using Mirror;
using WorldOfBalance.Player;

namespace WorldOfBalance.Map
{
    /// <summary>
    /// Менеджер респауна игроков для PvP-игры "Мир Баланса"
    /// Управляет созданием игроков и их размещением на карте
    /// </summary>
    public class SpawnManager : NetworkBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform spawnPointA;
        [SerializeField] private Transform spawnPointB;
        [SerializeField] private Transform[] additionalSpawnPoints; // Для будущего расширения до 4 игроков
        
        [Header("Player Settings")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private float respawnDelay = 3f;
        
        [Header("Arena Settings")]
        [SerializeField] private Vector2 arenaSize = new Vector2(20f, 20f);
        [SerializeField] private Color playerAColor = Color.blue;
        [SerializeField] private Color playerBColor = Color.red;
        
        // Сетевые переменные
        [SyncVar] private int currentPlayerCount = 0;
        
        private NetworkManagerLobby networkManager;
        
        private void Start()
        {
            networkManager = FindObjectOfType<NetworkManagerLobby>();
            if (networkManager == null)
            {
                Debug.LogError("NetworkManagerLobby не найден на сцене!");
            }
        }
        
        /// <summary>
        /// Получение следующей точки респауна
        /// </summary>
        [Server]
        public Transform GetNextSpawnPoint()
        {
            if (currentPlayerCount == 0)
            {
                return spawnPointA;
            }
            else if (currentPlayerCount == 1)
            {
                return spawnPointB;
            }
            else
            {
                // Для будущего расширения до 4 игроков
                int spawnIndex = (currentPlayerCount - 2) % additionalSpawnPoints.Length;
                return additionalSpawnPoints[spawnIndex];
            }
        }
        
        /// <summary>
        /// Создание игрока в указанной точке
        /// </summary>
        [Server]
        public GameObject SpawnPlayer(Transform spawnPoint)
        {
            if (playerPrefab == null)
            {
                Debug.LogError("PlayerPrefab не назначен в SpawnManager!");
                return null;
            }
            
            if (spawnPoint == null)
            {
                Debug.LogError("SpawnPoint не назначен!");
                return null;
            }
            
            // Создаем игрока
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // Настраиваем цвет игрока
            SetupPlayerColor(player, currentPlayerCount);
            
            // Спавним в сети
            NetworkServer.Spawn(player);
            
            currentPlayerCount++;
            
            Debug.Log($"Игрок создан в позиции {spawnPoint.name}. Всего игроков: {currentPlayerCount}");
            
            return player;
        }
        
        /// <summary>
        /// Настройка цвета игрока
        /// </summary>
        [Server]
        private void SetupPlayerColor(GameObject player, int playerIndex)
        {
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color playerColor = playerIndex == 0 ? playerAColor : playerBColor;
                spriteRenderer.color = playerColor;
            }
        }
        
        /// <summary>
        /// Респаун игрока после смерти
        /// </summary>
        [Server]
        public void RespawnPlayer(GameObject player)
        {
            if (player == null) return;
            
            // Получаем следующую точку респауна
            Transform spawnPoint = GetNextSpawnPoint();
            
            // Перемещаем игрока
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
            
            // Восстанавливаем здоровье
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.ResetHealth();
            }
            
            // Восстанавливаем компоненты
            NetworkPlayerController playerController = player.GetComponent<NetworkPlayerController>();
            if (playerController != null)
            {
                playerController.RpcRespawn(spawnPoint.position);
            }
            
            Debug.Log($"Игрок {player.name} респаунен в позиции {spawnPoint.name}");
        }
        
        /// <summary>
        /// Создание арены с препятствиями
        /// </summary>
        [Server]
        public void CreateArena()
        {
            // Создаем границы арены
            CreateArenaBoundaries();
            
            // Создаем препятствия для рикошетов
            CreateRicochetObstacles();
            
            Debug.Log("Арена создана успешно");
        }
        
        /// <summary>
        /// Создание границ арены
        /// </summary>
        [Server]
        private void CreateArenaBoundaries()
        {
            // Верхняя стена
            CreateWall(new Vector3(0, arenaSize.y / 2, 0), new Vector3(arenaSize.x, 1, 1), "Wall");
            
            // Нижняя стена
            CreateWall(new Vector3(0, -arenaSize.y / 2, 0), new Vector3(arenaSize.x, 1, 1), "Wall");
            
            // Левая стена
            CreateWall(new Vector3(-arenaSize.x / 2, 0, 0), new Vector3(1, arenaSize.y, 1), "Wall");
            
            // Правая стена
            CreateWall(new Vector3(arenaSize.x / 2, 0, 0), new Vector3(1, arenaSize.y, 1), "Wall");
        }
        
        /// <summary>
        /// Создание препятствий для рикошетов
        /// </summary>
        [Server]
        private void CreateRicochetObstacles()
        {
            // Центральные препятствия
            CreateWall(new Vector3(0, 0, 0), new Vector3(2, 2, 1), "Wall");
            CreateWall(new Vector3(3, 3, 0), new Vector3(1, 1, 1), "Wall");
            CreateWall(new Vector3(-3, -3, 0), new Vector3(1, 1, 1), "Wall");
            CreateWall(new Vector3(3, -3, 0), new Vector3(1, 1, 1), "Wall");
            CreateWall(new Vector3(-3, 3, 0), new Vector3(1, 1, 1), "Wall");
        }
        
        /// <summary>
        /// Создание стены
        /// </summary>
        [Server]
        private void CreateWall(Vector3 position, Vector3 scale, string tag)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = $"Wall_{position.x}_{position.y}";
            wall.transform.position = position;
            wall.transform.localScale = scale;
            wall.tag = tag;
            
            // Настройка материала
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.gray;
            }
            
            // Настройка коллайдера
            Collider collider = wall.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }
            
            // Спавним в сети
            NetworkServer.Spawn(wall);
        }
        
        /// <summary>
        /// Очистка арены
        /// </summary>
        [Server]
        public void ClearArena()
        {
            // Удаляем все стены
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            foreach (GameObject wall in walls)
            {
                NetworkServer.Destroy(wall);
            }
            
            Debug.Log("Арена очищена");
        }
        
        /// <summary>
        /// Получение размера арены
        /// </summary>
        public Vector2 GetArenaSize()
        {
            return arenaSize;
        }
        
        /// <summary>
        /// Проверка, находится ли позиция в пределах арены
        /// </summary>
        public bool IsPositionInArena(Vector3 position)
        {
            return Mathf.Abs(position.x) <= arenaSize.x / 2 && 
                   Mathf.Abs(position.y) <= arenaSize.y / 2;
        }
        
        /// <summary>
        /// Визуализация арены в редакторе
        /// </summary>
        private void OnDrawGizmos()
        {
            // Границы арены
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(arenaSize.x, arenaSize.y, 1));
            
            // Точки респауна
            if (spawnPointA != null)
            {
                Gizmos.color = playerAColor;
                Gizmos.DrawWireSphere(spawnPointA.position, 0.5f);
            }
            
            if (spawnPointB != null)
            {
                Gizmos.color = playerBColor;
                Gizmos.DrawWireSphere(spawnPointB.position, 0.5f);
            }
        }
    }
} 