using UnityEngine;
using Mirror;
using WorldOfBalance.Systems;
using WorldOfBalance.Player;

namespace WorldOfBalance.Projectile
{
    /// <summary>
    /// Сетевой снаряд для PvP-игры "Мир Баланса"
    /// Управляет физикой снаряда, пробитием брони и рикошетами
    /// </summary>
    public class NetworkProjectile : NetworkBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float damage = 25f;
        [SerializeField] private float penetrationPower = 51f;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private float ricochetThreshold = 30f; // Угол для рикошета
        
        [Header("Physics")]
        [SerializeField] private float bounceForce = 0.8f; // Сила отскока при рикошете
        [SerializeField] private LayerMask collisionLayers = -1;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject ricochetEffectPrefab;
        
        // Сетевые переменные
        [SyncVar] private Vector2 networkDirection;
        [SyncVar] private float networkSpeed;
        [SyncVar] private float networkDamage;
        [SyncVar] private float networkPenetrationPower;
        
        // Локальные переменные
        private Vector2 direction;
        private Rigidbody2D rb;
        private bool isInitialized = false;
        private GameObject owner;
        private float currentDamage;
        private float currentPenetrationPower;
        
        // Свойства
        public Vector2 Direction => direction;
        public float Speed => speed;
        public float Damage => currentDamage;
        public float PenetrationPower => currentPenetrationPower;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            
            rb.gravityScale = 0f;
            rb.drag = 0f;
            
            if (isServer)
            {
                Invoke(nameof(DestroyProjectile), lifetime);
            }
        }
        
        /// <summary>
        /// Инициализация снаряда на сервере
        /// </summary>
        [Server]
        public void Initialize(Vector2 projectileDirection, float projectileSpeed, float projectileDamage, float projectilePenetration, GameObject projectileOwner)
        {
            direction = projectileDirection.normalized;
            speed = projectileSpeed;
            currentDamage = projectileDamage;
            currentPenetrationPower = projectilePenetration;
            owner = projectileOwner;
            
            // Синхронизация с клиентами
            networkDirection = direction;
            networkSpeed = speed;
            networkDamage = currentDamage;
            networkPenetrationPower = currentPenetrationPower;
            
            isInitialized = true;
            
            // Установка начальной скорости
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
            
            // Уведомление клиентов об инициализации
            RpcInitialize(direction, speed, currentDamage, currentPenetrationPower);
        }
        
        /// <summary>
        /// Инициализация снаряда на клиентах
        /// </summary>
        [ClientRpc]
        private void RpcInitialize(Vector2 projectileDirection, float projectileSpeed, float projectileDamage, float projectilePenetration)
        {
            direction = projectileDirection;
            speed = projectileSpeed;
            currentDamage = projectileDamage;
            currentPenetrationPower = projectilePenetration;
            isInitialized = true;
            
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isServer) return;
            
            HandleCollision(collision);
        }
        
        /// <summary>
        /// Обработка столкновения снаряда
        /// </summary>
        [Server]
        private void HandleCollision(Collision2D collision)
        {
            // Получаем угол столкновения
            Vector2 normal = collision.contacts[0].normal;
            float collisionAngle = Vector2.Angle(direction, normal);
            
            Debug.Log($"Projectile collision with {collision.gameObject.name} at angle: {collisionAngle}°");
            
            // Проверяем, является ли объект игроком
            NetworkPlayerController player = collision.gameObject.GetComponent<NetworkPlayerController>();
            if (player != null && player.gameObject != owner)
            {
                HandlePlayerHit(player, collisionAngle, normal);
                return;
            }
            
            // Проверяем, является ли объект стеной
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                HandleWallHit(collisionAngle, normal);
                return;
            }
            
            // Для других объектов - просто уничтожаем снаряд
            DestroyProjectile();
        }
        
        /// <summary>
        /// Обработка попадания в игрока
        /// </summary>
        [Server]
        private void HandlePlayerHit(NetworkPlayerController player, float collisionAngle, Vector2 normal)
        {
            // Получаем систему брони игрока
            ArmorSystem armorSystem = player.GetComponent<ArmorSystem>();
            if (armorSystem == null)
            {
                // Если нет системы брони, наносим полный урон
                player.RpcTakeDamage(currentDamage, transform.position, direction);
                DestroyProjectile();
                return;
            }
            
            // Вычисляем эффективную броню
            Vector2 playerForward = player.transform.right; // Направление игрока
            float effectiveArmor = armorSystem.GetEffectiveArmor(-direction, playerForward);
            
            // Проверяем пробитие
            bool canPenetrate = currentPenetrationPower >= effectiveArmor;
            
            Debug.Log($"Hit angle: {collisionAngle}°, Effective armor: {effectiveArmor}, Penetration: {currentPenetrationPower}, Can penetrate: {canPenetrate}");
            
            if (canPenetrate)
            {
                // Снаряд пробивает броню
                player.RpcTakeDamage(currentDamage, transform.position, direction);
                RpcSpawnHitEffect(transform.position, direction);
                Debug.Log("Projectile penetrated armor!");
            }
            else
            {
                // Снаряд рикошетит от брони
                Ricochet(normal);
                RpcSpawnRicochetEffect(transform.position, direction);
                Debug.Log("Projectile ricocheted from armor!");
            }
        }
        
        /// <summary>
        /// Обработка попадания в стену
        /// </summary>
        [Server]
        private void HandleWallHit(float collisionAngle, Vector2 normal)
        {
            // Если угол меньше порога рикошета - снаряд рикошетит
            if (collisionAngle < ricochetThreshold)
            {
                Ricochet(normal);
                RpcSpawnRicochetEffect(transform.position, direction);
                Debug.Log($"Projectile ricocheted from wall at angle: {collisionAngle}°");
            }
            else
            {
                // Снаряд застревает в стене
                RpcSpawnHitEffect(transform.position, direction);
                Debug.Log($"Projectile stuck in wall at angle: {collisionAngle}°");
                DestroyProjectile();
            }
        }
        
        /// <summary>
        /// Рикошет снаряда
        /// </summary>
        [Server]
        private void Ricochet(Vector2 normal)
        {
            // Вычисляем направление отскока
            Vector2 reflection = Vector2.Reflect(direction, normal);
            
            // Применяем силу отскока
            Vector2 newVelocity = reflection * speed * bounceForce;
            
            // Обновляем направление и скорость
            direction = reflection.normalized;
            networkDirection = direction;
            
            if (rb != null)
            {
                rb.velocity = newVelocity;
            }
            
            // Уменьшаем урон и пробитие после рикошета
            currentDamage *= 0.7f;
            currentPenetrationPower *= 0.8f;
            networkDamage = currentDamage;
            networkPenetrationPower = currentPenetrationPower;
            
            Debug.Log($"Projectile ricocheted! New direction: {direction}, New damage: {currentDamage}");
            
            // Если скорость стала слишком низкой, уничтожаем снаряд
            if (rb.velocity.magnitude < speed * 0.3f)
            {
                Debug.Log("Projectile lost too much energy, destroying");
                DestroyProjectile();
            }
        }
        
        /// <summary>
        /// Создание эффекта попадания на клиентах
        /// </summary>
        [ClientRpc]
        private void RpcSpawnHitEffect(Vector3 position, Vector2 direction)
        {
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
        
        /// <summary>
        /// Создание эффекта рикошета на клиентах
        /// </summary>
        [ClientRpc]
        private void RpcSpawnRicochetEffect(Vector3 position, Vector2 direction)
        {
            if (ricochetEffectPrefab != null)
            {
                GameObject effect = Instantiate(ricochetEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 1f);
            }
        }
        
        /// <summary>
        /// Уничтожение снаряда
        /// </summary>
        [Server]
        private void DestroyProjectile()
        {
            NetworkServer.Destroy(gameObject);
        }
        
        private void OnDrawGizmos()
        {
            // Визуализация направления снаряда в редакторе
            if (isInitialized)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, direction * 2f);
            }
        }
    }
} 