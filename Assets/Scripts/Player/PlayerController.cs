using UnityEngine;

/// <summary>
/// Контроллер игрока с управлением движением и стрельбой
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    
    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    public float projectileSpeed = 10f;
    
    [Header("Input")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string fireButton = "Fire1";
    
    private float nextFireTime;
    private Camera mainCamera;
    private Rigidbody2D rb;
    private DamageSystem damageSystem;
    private ArmorSystem armorSystem;
    
    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        damageSystem = GetComponent<DamageSystem>();
        armorSystem = GetComponent<ArmorSystem>();
        
        if (firePoint == null)
        {
            firePoint = transform;
        }
        
        // Проверяем наличие необходимых компонентов
        if (damageSystem == null)
        {
            Debug.LogWarning($"DamageSystem not found on {gameObject.name}");
        }
        
        if (armorSystem == null)
        {
            Debug.LogWarning($"ArmorSystem not found on {gameObject.name}");
        }
    }
    
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleShooting();
    }
    
    /// <summary>
    /// Обрабатывает движение игрока
    /// </summary>
    void HandleMovement()
    {
        float horizontal = Input.GetAxis(horizontalAxis);
        float vertical = Input.GetAxis(verticalAxis);
        
        Vector2 movement = new Vector2(horizontal, vertical).normalized * moveSpeed;
        rb.velocity = movement;
    }
    
    /// <summary>
    /// Обрабатывает поворот игрока к курсору мыши
    /// </summary>
    void HandleRotation()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Обрабатывает стрельбу
    /// </summary>
    void HandleShooting()
    {
        if (Input.GetButton(fireButton) && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    /// <summary>
    /// Создает и запускает снаряд
    /// </summary>
    void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            
            if (projectileScript != null)
            {
                projectileScript.Initialize(projectileSpeed, transform.right);
            }
        }
    }
    
    /// <summary>
    /// Возвращает true если игрок жив
    /// </summary>
    /// <returns>true если игрок жив</returns>
    public bool IsAlive()
    {
        return damageSystem != null && damageSystem.IsAlive();
    }
    
    /// <summary>
    /// Возвращает процент здоровья игрока
    /// </summary>
    /// <returns>Процент здоровья (0-1)</returns>
    public float GetHealthPercentage()
    {
        return damageSystem != null ? damageSystem.GetHealthPercentage() : 0f;
    }
    
    /// <summary>
    /// Возвращает текущее здоровье игрока
    /// </summary>
    /// <returns>Текущее здоровье</returns>
    public float GetCurrentHealth()
    {
        return damageSystem != null ? damageSystem.currentHealth : 0f;
    }
    
    /// <summary>
    /// Возвращает максимальное здоровье игрока
    /// </summary>
    /// <returns>Максимальное здоровье</returns>
    public float GetMaxHealth()
    {
        return damageSystem != null ? damageSystem.maxHealth : 0f;
    }
} 