using UnityEngine;
using Mirror;
using WorldOfBalance.Player;
using WorldOfBalance.Systems;

namespace WorldOfBalance.Map
{
    public class SpawnManager : NetworkBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform spawnPointA;
        [SerializeField] private Transform spawnPointB;
        
        [Header("Arena Settings")]
        [SerializeField] private float arenaSize = 20f;
        [SerializeField] private float wallThickness = 1f;
        [SerializeField] private Material wallMaterial;
        
        [Header("Components")]
        [SerializeField] private NetworkManagerLobby networkManager;
        
        private int currentSpawnIndex = 0;
        
        private void Awake()
        {
            // Получаем NetworkManager с типизацией
            if (networkManager == null)
                networkManager = FindObjectOfType<NetworkManagerLobby>();
        }
        
        /// <summary>
        /// Получает следующую точку спавна
        /// </summary>
        /// <returns>Точка спавна</returns>
        [Server]
        public Transform GetNextSpawnPoint()
        {
            Transform spawnPoint = currentSpawnIndex == 0 ? spawnPointA : spawnPointB;
            currentSpawnIndex = (currentSpawnIndex + 1) % 2;
            return spawnPoint;
        }
        
        /// <summary>
        /// Спавнит игрока в указанной позиции
        /// </summary>
        /// <param name="playerPrefab">Префаб игрока</param>
        /// <param name="spawnPoint">Точка спавна</param>
        /// <returns>Созданный игрок</returns>
        [Server]
        public GameObject SpawnPlayer(GameObject playerPrefab, Transform spawnPoint)
        {
            if (playerPrefab == null || spawnPoint == null)
            {
                Debug.LogError("SpawnPlayer: Invalid prefab or spawn point!");
                return null;
            }
            
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(player);
            
            SetupPlayerColor(player);
            
            Debug.Log($"Player spawned at {spawnPoint.name}");
            return player;
        }
        
        /// <summary>
        /// Настраивает цвет игрока в зависимости от того, локальный он или нет
        /// </summary>
        /// <param name="player">Игрок</param>
        [Server]
        private void SetupPlayerColor(GameObject player)
        {
            NetworkPlayerController playerController = player.GetComponent<NetworkPlayerController>();
            if (playerController != null)
            {
                SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    // Устанавливаем цвет в зависимости от того, локальный игрок или нет
                    Color playerColor = playerController.IsLocalPlayer ? Color.blue : Color.red;
                    spriteRenderer.color = playerColor;
                }
            }
        }
        
        /// <summary>
        /// Возрождает игрока
        /// </summary>
        /// <param name="player">Игрок для возрождения</param>
        [Server]
        public void RespawnPlayer(GameObject player)
        {
            if (player == null) return;
            
            Transform spawnPoint = GetNextSpawnPoint();
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
            
            // Получаем компоненты с типизацией
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            NetworkPlayerController playerController = player.GetComponent<NetworkPlayerController>();
            
            if (healthSystem != null)
            {
                healthSystem.Respawn();
            }
            
            if (playerController != null)
            {
                // Сбрасываем состояние игрока
                // (это будет обработано в NetworkPlayerController)
            }
            
            Debug.Log($"Player {player.name} respawned at {spawnPoint.name}");
        }
        
        /// <summary>
        /// Создает арену для игры
        /// </summary>
        [Server]
        public void CreateArena()
        {
            CreateArenaBoundaries();
            CreateRicochetObstacles();
            Debug.Log("Arena created successfully");
        }
        
        /// <summary>
        /// Создает границы арены
        /// </summary>
        [Server]
        private void CreateArenaBoundaries()
        {
            float halfSize = arenaSize / 2f;
            
            // Создаем стены по периметру
            CreateWall(new Vector3(-halfSize, 0, 0), new Vector3(wallThickness, arenaSize, 1f), "Wall_Left");
            CreateWall(new Vector3(halfSize, 0, 0), new Vector3(wallThickness, arenaSize, 1f), "Wall_Right");
            CreateWall(new Vector3(0, -halfSize, 0), new Vector3(arenaSize, wallThickness, 1f), "Wall_Bottom");
            CreateWall(new Vector3(0, halfSize, 0), new Vector3(arenaSize, wallThickness, 1f), "Wall_Top");
        }
        
        /// <summary>
        /// Создает препятствия для рикошетов
        /// </summary>
        [Server]
        private void CreateRicochetObstacles()
        {
            // Создаем несколько препятствий в центре арены
            CreateWall(new Vector3(0, 0, 0), new Vector3(3f, 1f, 1f), "Obstacle_Center");
            CreateWall(new Vector3(-5f, 2f, 0), new Vector3(2f, 2f, 1f), "Obstacle_Left");
            CreateWall(new Vector3(5f, -2f, 0), new Vector3(2f, 2f, 1f), "Obstacle_Right");
        }
        
        /// <summary>
        /// Создает стену
        /// </summary>
        /// <param name="position">Позиция стены</param>
        /// <param name="scale">Размер стены</param>
        /// <param name="name">Имя стены</param>
        [Server]
        private void CreateWall(Vector3 position, Vector3 scale, string name)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = position;
            wall.transform.localScale = scale;
            
            // Настраиваем компоненты с типизацией
            Renderer renderer = wall.GetComponent<Renderer>();
            Collider collider = wall.GetComponent<Collider>();
            
            if (renderer != null && wallMaterial != null)
            {
                renderer.material = wallMaterial;
            }
            
            if (collider != null)
            {
                // Настраиваем коллайдер для 2D физики
                collider.isTrigger = false;
            }
            
            // Устанавливаем тег для идентификации
            wall.tag = "Wall";
            
            Debug.Log($"Wall created: {name} at {position}");
        }
        
        /// <summary>
        /// Очищает арену
        /// </summary>
        [Server]
        public void ClearArena()
        {
            // Удаляем все стены и препятствия
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            foreach (GameObject wall in walls)
            {
                NetworkServer.Destroy(wall);
            }
            
            Debug.Log("Arena cleared");
        }
        
        /// <summary>
        /// Получает размер арены
        /// </summary>
        /// <returns>Размер арены</returns>
        public float GetArenaSize()
        {
            return arenaSize;
        }
        
        /// <summary>
        /// Проверяет, находится ли позиция в пределах арены
        /// </summary>
        /// <param name="position">Позиция для проверки</param>
        /// <returns>true, если позиция в пределах арены</returns>
        public bool IsPositionInArena(Vector3 position)
        {
            float halfSize = arenaSize / 2f;
            return Mathf.Abs(position.x) <= halfSize && Mathf.Abs(position.y) <= halfSize;
        }
    }
} 