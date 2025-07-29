using UnityEngine;
using Mirror;
using WorldOfBalance.Systems;

namespace WorldOfBalance.Player
{
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
        
        // Network synchronization
        [SyncVar] private Vector3 networkPosition;
        [SyncVar] private Quaternion networkRotation;
        [SyncVar] private bool isDead;
        
        // Local variables
        private Vector2 moveInput;
        private Vector2 aimDirection;
        private float lastFireTime;
        private CameraFollow playerCamera;
        
        private void Awake()
        {
            // Получаем компоненты с типизацией
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (armorSystem == null)
                armorSystem = GetComponent<ArmorSystem>();
            if (healthSystem == null)
                healthSystem = GetComponent<HealthSystem>();
            
            // Настраиваем Rigidbody2D для 2D игры
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.drag = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        
        private void Start()
        {
            if (isLocalPlayer)
            {
                // Находим камеру с типизацией
                playerCamera = FindObjectOfType<CameraFollow>();
                if (playerCamera != null)
                {
                    playerCamera.SetTarget(transform);
                }
                else
                {
                    Debug.LogWarning("CameraFollow not found in scene!");
                }
            }
        }
        
        private void Update()
        {
            if (!isLocalPlayer || isDead) return;
            
            HandleInput();
        }
        
        private void FixedUpdate()
        {
            if (!isLocalPlayer || isDead) return;
            
            Move();
            RotateTowardsMouse();
        }
        
        private void HandleInput()
        {
            // Движение
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput = moveInput.normalized;
            
            // Прицеливание
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = (mousePosition - transform.position).normalized;
            
            // Стрельба
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                CmdFire();
            }
        }
        
        private void Move()
        {
            if (rb != null)
            {
                Vector2 velocity = moveInput * moveSpeed;
                rb.velocity = velocity;
                
                // Синхронизируем позицию
                if (isServer)
                {
                    networkPosition = transform.position;
                }
            }
        }
        
        private void RotateTowardsMouse()
        {
            if (aimDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                
                // Синхронизируем поворот
                if (isServer)
                {
                    networkRotation = transform.rotation;
                }
            }
        }
        
        private bool CanFire()
        {
            return Time.time - lastFireTime >= fireRate;
        }
        
        [Command]
        private void CmdFire()
        {
            if (!CanFire()) return;
            
            lastFireTime = Time.time;
            
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                NetworkServer.Spawn(projectile);
                
                // Получаем компонент с типизацией
                NetworkProjectile projectileComponent = projectile.GetComponent<NetworkProjectile>();
                if (projectileComponent != null)
                {
                    projectileComponent.Initialize(aimDirection, projectileSpeed, projectileDamage, projectilePenetration, gameObject);
                }
                else
                {
                    Debug.LogError("NetworkProjectile component not found on projectile prefab!");
                }
            }
        }
        
        [ClientRpc]
        private void RpcTakeDamage()
        {
            if (spriteRenderer != null)
            {
                StartCoroutine(DamageFlash());
            }
        }
        
        private System.Collections.IEnumerator DamageFlash()
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
        
        [ClientRpc]
        private void RpcDie()
        {
            isDead = true;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.gray;
            }
            Debug.Log("Player died!");
        }
        
        [ClientRpc]
        private void RpcRespawn()
        {
            isDead = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
            Debug.Log("Player respawned!");
        }
        
        public bool IsDead => isDead;
        
        // Network synchronization
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.WriteVector3(transform.position);
            writer.WriteQuaternion(transform.rotation);
            writer.WriteBool(isDead);
        }
        
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            networkPosition = reader.ReadVector3();
            networkRotation = reader.ReadQuaternion();
            isDead = reader.ReadBool();
        }
        
        private void Update()
        {
            if (!isLocalPlayer)
            {
                // Интерполяция для удаленных игроков
                transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
                transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
            }
        }
    }
} 