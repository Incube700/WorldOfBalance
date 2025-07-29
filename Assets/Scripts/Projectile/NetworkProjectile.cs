using UnityEngine;
using Mirror;
using WorldOfBalance.Systems;
using WorldOfBalance.Effects;

namespace WorldOfBalance.Projectile
{
    public class NetworkProjectile : NetworkBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float damage = 25f;
        [SerializeField] private float penetrationPower = 51f;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private float ricochetThreshold = 70f;
        [SerializeField] private float bounceForce = 0.8f;
        
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject ricochetEffectPrefab;
        
        // Network synchronization
        [SyncVar] private Vector2 networkDirection;
        [SyncVar] private float networkSpeed;
        [SyncVar] private float networkDamage;
        [SyncVar] private float networkPenetrationPower;
        
        // Local variables
        private Vector2 direction;
        private float currentDamage;
        private float currentPenetrationPower;
        private float currentSpeed;
        private float currentLifetime;
        private int bounceCount = 0;
        private GameObject owner;
        
        private void Awake()
        {
            // Получаем компоненты с типизацией
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (circleCollider == null)
                circleCollider = GetComponent<CircleCollider2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Настраиваем Rigidbody2D для 2D игры
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.drag = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        
        [Server]
        public void Initialize(Vector2 direction, float speed, float damage, float penetrationPower, GameObject owner)
        {
            this.direction = direction.normalized;
            this.currentSpeed = speed;
            this.currentDamage = damage;
            this.currentPenetrationPower = penetrationPower;
            this.owner = owner;
            this.currentLifetime = lifetime;
            
            // Синхронизируем с сетью
            networkDirection = this.direction;
            networkSpeed = this.currentSpeed;
            networkDamage = this.currentDamage;
            networkPenetrationPower = this.currentPenetrationPower;
            
            // Устанавливаем начальную скорость
            if (rb != null)
            {
                rb.velocity = this.direction * this.currentSpeed;
            }
            
            Debug.Log($"Projectile initialized: Direction={direction}, Speed={speed}, Damage={damage}, Penetration={penetrationPower}");
        }
        
        private void Update()
        {
            if (!isServer) return;
            
            // Обновляем время жизни
            currentLifetime -= Time.deltaTime;
            if (currentLifetime <= 0)
            {
                DestroyProjectile();
                return;
            }
            
            // Обновляем направление из сетевых данных
            if (direction != networkDirection)
            {
                direction = networkDirection;
                if (rb != null)
                {
                    rb.velocity = direction * currentSpeed;
                }
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer) return;
            
            // Игнорируем столкновения с владельцем
            if (other.gameObject == owner) return;
            
            // Проверяем, является ли объект игроком
            NetworkPlayerController player = other.GetComponent<NetworkPlayerController>();
            if (player != null)
            {
                HandlePlayerHit(other);
            }
            else
            {
                // Столкновение со стеной или другим объектом
                HandleWallHit(other);
            }
        }
        
        [Server]
        private void HandlePlayerHit(Collider2D collision)
        {
            Vector2 normal = (transform.position - collision.transform.position).normalized;
            
            // Проверяем угол рикошета
            float ricochetAngle = Vector2.Angle(-direction, normal);
            if (ricochetAngle > ricochetThreshold)
            {
                Ricochet(normal);
                RpcSpawnRicochetEffect(transform.position, direction);
                Debug.Log("Projectile ricocheted due to steep angle!");
                return;
            }
            
            // Получаем компоненты с типизацией
            ArmorSystem armorSystem = collision.GetComponent<ArmorSystem>();
            HealthSystem healthSystem = collision.GetComponent<HealthSystem>();
            
            if (armorSystem != null && healthSystem != null)
            {
                // Вычисляем эффективную броню
                Vector2 playerForward = collision.transform.right; // Предполагаем, что игрок смотрит вправо
                float effectiveArmor = armorSystem.GetEffectiveArmor(-direction, playerForward);
                
                // Проверяем пробитие
                if (currentPenetrationPower >= effectiveArmor)
                {
                    // Пробитие - наносим урон
                    healthSystem.TakeDamage(currentDamage);
                    Debug.Log($"Projectile penetrated! Damage: {currentDamage}, Effective Armor: {effectiveArmor}");
                    
                    RpcSpawnHitEffect(transform.position, direction);
                }
                else
                {
                    // Не пробивает - рикошет
                    Ricochet(normal);
                    RpcSpawnRicochetEffect(transform.position, direction);
                    Debug.Log($"Projectile ricocheted! Penetration: {currentPenetrationPower}, Effective Armor: {effectiveArmor}");
                }
            }
            else
            {
                Debug.LogError("ArmorSystem or HealthSystem not found on player!");
                RpcSpawnHitEffect(transform.position, direction);
            }
            
            DestroyProjectile();
        }
        
        [Server]
        private void HandleWallHit(Collider2D collision)
        {
            Vector2 normal = (transform.position - collision.transform.position).normalized;
            
            // Проверяем угол рикошета
            float ricochetAngle = Vector2.Angle(-direction, normal);
            if (ricochetAngle > ricochetThreshold)
            {
                Ricochet(normal);
                RpcSpawnRicochetEffect(transform.position, direction);
                return;
            }
            else
            {
                RpcSpawnHitEffect(transform.position, direction);
                NetworkServer.Destroy(gameObject);
            }
        }
        
        [Server]
        private void Ricochet(Vector2 normal)
        {
            Vector2 reflectedDirection = Vector2.Reflect(direction, normal).normalized;
            Vector2 newVelocity = reflectedDirection * currentSpeed * bounceForce;
            
            direction = reflectedDirection;
            networkDirection = direction;
            
            if (rb != null)
            {
                rb.velocity = newVelocity;
            }
            
            // Уменьшаем урон и силу пробития после рикошета
            currentDamage *= 0.7f;
            currentPenetrationPower *= 0.8f;
            networkDamage = currentDamage;
            networkPenetrationPower = currentPenetrationPower;
            
            bounceCount++;
            if (bounceCount >= 3)
            {
                currentPenetrationPower = 0f;
                networkPenetrationPower = currentPenetrationPower;
                Debug.Log("Projectile lost penetration after 3 bounces");
            }
            
            Debug.Log($"Projectile ricocheted! New direction: {direction}, New damage: {currentDamage}");
            
            // Уничтожаем снаряд, если скорость слишком низкая
            if (rb.velocity.magnitude < currentSpeed * 0.3f)
            {
                DestroyProjectile();
            }
        }
        
        [Server]
        private void DestroyProjectile()
        {
            NetworkServer.Destroy(gameObject);
        }
        
        [ClientRpc]
        private void RpcSpawnHitEffect(Vector2 position, Vector2 direction)
        {
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
                HitEffect hitEffect = effect.GetComponent<HitEffect>();
                if (hitEffect != null)
                {
                    hitEffect.Initialize(direction);
                }
            }
        }
        
        [ClientRpc]
        private void RpcSpawnRicochetEffect(Vector2 position, Vector2 direction)
        {
            if (ricochetEffectPrefab != null)
            {
                GameObject effect = Instantiate(ricochetEffectPrefab, position, Quaternion.identity);
                RicochetEffect ricochetEffect = effect.GetComponent<RicochetEffect>();
                if (ricochetEffect != null)
                {
                    ricochetEffect.Initialize(direction);
                }
            }
        }
        
        // Статический метод для создания эффекта рикошета
        public static void CreateRicochetEffect(Vector2 position, Vector2 direction)
        {
            GameObject effectPrefab = Resources.Load<GameObject>("Effects/RicochetEffect");
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
                RicochetEffect ricochetEffect = effect.GetComponent<RicochetEffect>();
                if (ricochetEffect != null)
                {
                    ricochetEffect.Initialize(direction);
                }
            }
        }
    }
} 