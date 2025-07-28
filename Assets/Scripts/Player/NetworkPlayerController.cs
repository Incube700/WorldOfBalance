using UnityEngine;
using Mirror;
using WorldOfBalance.Systems;

namespace WorldOfBalance.Player
{
    /// <summary>
    /// Сетевой контроллер игрока для PvP-игры "Мир Баланса"
    /// Управляет движением, стрельбой и синхронизацией по сети
    /// </summary>
    public class NetworkPlayerController : NetworkBehaviour
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
        [SyncVar] private Vector2 networkPosition;
        [SyncVar] private float networkRotation;
        [SyncVar] private bool isDead = false;
        
        // Локальные переменные
        private float lastFireTime;
        private Vector2 moveInput;
        private Vector2 aimDirection;
        private Camera playerCamera;
        
        // Свойства
        public bool IsDead => isDead;
        public bool IsLocalPlayer => isLocalPlayer;
        
        private void Start()
        {
            if (isLocalPlayer)
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
            if (!isLocalPlayer || isDead) return;
            
            // Обработка ввода
            HandleInput();
            
            // Стрельба
            if (Input.GetMouseButton(0) && CanFire())
            {
                CmdFire();
            }
        }
        
        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            
            // Движение
            Move();
            
            // Поворот к мыши
            RotateTowardsMouse();
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
        /// Проверка возможности стрельбы
        /// </summary>
        private bool CanFire()
        {
            return Time.time - lastFireTime >= fireRate;
        }
        
        /// <summary>
        /// Команда стрельбы (вызывается на клиенте, выполняется на сервере)
        /// </summary>
        [Command]
        private void CmdFire()
        {
            if (!CanFire()) return;
            
            lastFireTime = Time.time;
            
            // Создание снаряда на сервере
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                NetworkServer.Spawn(projectile);
                
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
        public void RpcTakeDamage(float damage, Vector2 hitPoint, Vector2 hitDirection)
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
        public void RpcDie()
        {
            isDead = true;
            
            // Отключение компонентов
            if (rb != null) rb.simulated = false;
            if (spriteRenderer != null) spriteRenderer.color = Color.gray;
            
            Debug.Log($"Игрок {gameObject.name} погиб");
        }
        
        /// <summary>
        /// Возрождение игрока
        /// </summary>
        [ClientRpc]
        public void RpcRespawn(Vector3 spawnPosition)
        {
            isDead = false;
            transform.position = spawnPosition;
            
            // Восстановление компонентов
            if (rb != null) rb.simulated = true;
            if (healthSystem != null) healthSystem.ResetHealth();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isLocalPlayer ? Color.blue : Color.red;
            }
        }
        
        /// <summary>
        /// Синхронизация позиции и поворота
        /// </summary>
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.WriteVector2(transform.position);
            writer.WriteFloat(transform.rotation.eulerAngles.z);
        }
        
        /// <summary>
        /// Десериализация позиции и поворота
        /// </summary>
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            networkPosition = reader.ReadVector2();
            networkRotation = reader.ReadFloat();
        }
        
        /// <summary>
        /// Обновление позиции удаленного игрока
        /// </summary>
        private void UpdateRemotePlayer()
        {
            if (!isLocalPlayer)
            {
                transform.position = Vector2.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, networkRotation), Time.deltaTime * 10f);
            }
        }
    }
} 