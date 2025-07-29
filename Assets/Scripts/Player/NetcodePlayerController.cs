using UnityEngine;
using Unity.Netcode;
using WorldOfBalance.Systems;

namespace WorldOfBalance.Player
{
    /// <summary>
    /// Сетевой контроллер игрока для PvP-игры "Мир Баланса" с использованием Unity Netcode for GameObjects
    /// Управляет движением, стрельбой и синхронизацией по сети
    /// </summary>
    public class NetcodePlayerController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 180f;
        
        [Header("Combat Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float projectileDamage = 10f;
        [SerializeField] private float projectilePenetration = 51f;
        
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private ArmorSystem armorSystem;
        [SerializeField] private HealthSystem healthSystem;
        
        // Сетевые переменные
        private NetworkVariable<Vector2> networkPosition = new NetworkVariable<Vector2>();
        private NetworkVariable<float> networkRotation = new NetworkVariable<float>();
        private NetworkVariable<bool> isDead = new NetworkVariable<bool>();
        
        // Локальные переменные
        private float lastFireTime;
        private Vector2 moveInput;
        private Vector2 aimDirection;
        private Camera playerCamera;
        
        // Свойства
        public bool IsDead => isDead.Value;
        public bool IsLocalPlayer => IsOwner;
        
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                // Настройка для локального игрока
                playerCamera = Camera.main;
                if (playerCamera != null)
                {
                    playerCamera.GetComponent<CameraFollow>()?.SetTarget(transform);
                }
                
                // Установка цвета для локального игрока
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.blue;
                }
            }
            else
            {
                // Настройка для удаленного игрока
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.red;
                }
            }
        }
        
        private void Update()
        {
            if (!IsOwner || isDead.Value) return;
            
            // Обработка ввода
            HandleInput();
            
            // Стрельба
            if (Input.GetMouseButton(0) && CanFire())
            {
                FireServerRpc();
            }
        }
        
        private void FixedUpdate()
        {
            if (!IsOwner) return;
            
            // Движение
            Move();
            
            // Поворот к мыши
            RotateTowardsMouse();
            
            // Обновление сетевых переменных
            UpdateNetworkVariables();
        }
        
        /// <summary>
        /// Обработка ввода игрока
        /// </summary>
        private void HandleInput()
        {
            // Движение
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput = moveInput.normalized;
            
            // Прицеливание
            Vector3 mouseWorldPos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = (mouseWorldPos - transform.position).normalized;
        }
        
        /// <summary>
        /// Движение игрока
        /// </summary>
        private void Move()
        {
            if (rb != null)
            {
                rb.velocity = moveInput * moveSpeed;
            }
        }
        
        /// <summary>
        /// Поворот игрока к мыши
        /// </summary>
        private void RotateTowardsMouse()
        {
            if (aimDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.AngleAxis(angle, Vector3.forward),
                    rotationSpeed * Time.deltaTime
                );
            }
        }
        
        /// <summary>
        /// Обновление сетевых переменных
        /// </summary>
        private void UpdateNetworkVariables()
        {
            if (IsServer)
            {
                networkPosition.Value = transform.position;
                networkRotation.Value = transform.rotation.eulerAngles.z;
            }
        }
        
        /// <summary>
        /// Проверка возможности стрельбы
        /// </summary>
        private bool CanFire()
        {
            return Time.time - lastFireTime >= fireRate;
        }
        
        /// <summary>
        /// Серверный RPC для стрельбы
        /// </summary>
        [ServerRpc]
        private void FireServerRpc()
        {
            if (!CanFire()) return;
            
            lastFireTime = Time.time;
            
            // Создание снаряда на сервере
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                projectile.GetComponent<NetworkObject>()?.Spawn();
                
                // Настройка снаряда
                Projectile projectileComponent = projectile.GetComponent<Projectile>();
                if (projectileComponent != null)
                {
                    projectileComponent.Initialize(aimDirection, projectileSpeed, projectileDamage, projectilePenetration, gameObject);
                }
            }
        }
        
        /// <summary>
        /// Получение урона
        /// </summary>
        [ClientRpc]
        public void TakeDamageClientRpc(float damage, Vector2 hitPoint, Vector2 hitDirection)
        {
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
                
                // Визуальный эффект получения урона
                StartCoroutine(DamageFlash());
            }
        }
        
        /// <summary>
        /// Визуальный эффект получения урона
        /// </summary>
        private System.Collections.IEnumerator DamageFlash()
        {
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
            }
        }
        
        /// <summary>
        /// Смерть игрока
        /// </summary>
        [ClientRpc]
        public void DieClientRpc()
        {
            isDead.Value = true;
            
            // Отключение компонентов
            if (rb != null) rb.simulated = false;
            if (spriteRenderer != null) spriteRenderer.color = Color.gray;
            
            Debug.Log($"Игрок {gameObject.name} погиб");
        }
        
        /// <summary>
        /// Возрождение игрока
        /// </summary>
        [ClientRpc]
        public void RespawnClientRpc(Vector3 spawnPosition)
        {
            isDead.Value = false;
            transform.position = spawnPosition;
            
            // Восстановление компонентов
            if (rb != null) rb.simulated = true;
            if (healthSystem != null) healthSystem.ResetHealth();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = IsOwner ? Color.blue : Color.red;
            }
        }
        
        /// <summary>
        /// Обновление позиции удаленного игрока
        /// </summary>
        private void UpdateRemotePlayer()
        {
            if (!IsOwner)
            {
                transform.position = Vector2.Lerp(transform.position, networkPosition.Value, Time.deltaTime * 10f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, networkRotation.Value), Time.deltaTime * 10f);
            }
        }
    }
}