using UnityEngine;

/// <summary>
/// Снаряд с улучшенной физикой пробития и рикошета
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float damage = 25f;
    public float penetrationPower = 30f; // Сила пробития
    public float lifetime = 5f;
    public float ricochetThreshold = 30f; // Угол для рикошета
    
    [Header("Physics")]
    public float bounceForce = 0.8f; // Сила отскока при рикошете
    public float minSpeedAfterRicochet = 3f; // Минимальная скорость после рикошета
    public LayerMask collisionLayers = -1;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    private bool isInitialized = false;
    private int ricochetCount = 0;
    private int maxRicochets = 3; // Максимальное количество рикошетов
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        rb.gravityScale = 0f;
        rb.drag = 0f;
        
        Destroy(gameObject, lifetime);
    }
    
    /// <summary>
    /// Инициализирует снаряд
    /// </summary>
    /// <param name="projectileSpeed">Скорость снаряда</param>
    /// <param name="projectileDirection">Направление движения</param>
    public void Initialize(float projectileSpeed, Vector2 projectileDirection)
    {
        speed = projectileSpeed;
        direction = projectileDirection.normalized;
        isInitialized = true;
        
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }
    
    /// <summary>
    /// Обрабатывает столкновение снаряда
    /// </summary>
    /// <param name="collision">Информация о столкновении</param>
    void HandleCollision(Collision2D collision)
    {
        // Получаем нормаль поверхности
        Vector2 normal = collision.contacts[0].normal;
        
        // Вычисляем угол между направлением снаряда и нормалью поверхности
        float angle = Vector2.Angle(direction, normal);
        
        Debug.Log($"Projectile collision with {collision.gameObject.name} at angle: {angle}°");
        
        // Проверяем, является ли объект игроком
        DamageSystem damageSystem = collision.gameObject.GetComponent<DamageSystem>();
        if (damageSystem != null)
        {
            HandlePlayerHit(damageSystem, collision, normal);
            return;
        }
        
        // Проверяем, является ли объект стеной
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            HandleWallHit(collision, normal);
            return;
        }
        
        // Для других объектов - просто уничтожаем снаряд
        DestroyProjectile();
    }
    
    /// <summary>
    /// Обрабатывает попадание в игрока
    /// </summary>
    /// <param name="damageSystem">Система урона игрока</param>
    /// <param name="collision">Информация о столкновении</param>
    /// <param name="normal">Нормаль поверхности</param>
    void HandlePlayerHit(DamageSystem damageSystem, Collision2D collision, Vector2 normal)
    {
        // Вычисляем направление удара (от снаряда к игроку)
        Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
        
        // Пытаемся нанести урон игроку
        bool damageDealt = damageSystem.TakeDamage(damage, penetrationPower, hitDirection);
        
        if (damageDealt)
        {
            // Снаряд пробил броню - уничтожаем его
            Debug.Log($"Projectile penetrated player! Damage: {damage}");
            DestroyProjectile();
        }
        else
        {
            // Снаряд не пробил броню - он отскакивает
            Debug.Log($"Projectile deflected by player armor!");
            Ricochet(normal);
        }
    }
    
    /// <summary>
    /// Обрабатывает попадание в стену
    /// </summary>
    /// <param name="collision">Информация о столкновении</param>
    /// <param name="normal">Нормаль поверхности</param>
    void HandleWallHit(Collision2D collision, Vector2 normal)
    {
        // Вычисляем угол между направлением снаряда и нормалью стены
        float angle = Vector2.Angle(direction, normal);
        
        // Если угол меньше порога рикошета - снаряд рикошетит
        if (angle < ricochetThreshold && ricochetCount < maxRicochets)
        {
            Ricochet(normal);
        }
        else
        {
            // Снаряд застревает в стене
            Debug.Log($"Projectile stuck in wall at angle: {angle}°");
            
            // Показываем эффект уничтожения
            ShowDestroyEffect();
            DestroyProjectile();
        }
    }
    
    /// <summary>
    /// Обрабатывает рикошет снаряда
    /// </summary>
    /// <param name="normal">Нормаль поверхности</param>
    void Ricochet(Vector2 normal)
    {
        ricochetCount++;
        
        // Вычисляем направление отскока по формуле отражения
        Vector2 reflection = Vector2.Reflect(direction, normal);
        
        // Применяем силу отскока
        Vector2 newVelocity = reflection * speed * bounceForce;
        
        // Обновляем направление и скорость
        direction = reflection.normalized;
        rb.velocity = newVelocity;
        
        // Уменьшаем урон после рикошета
        damage *= 0.8f;
        
        // Уменьшаем силу пробития после рикошета
        penetrationPower *= 0.9f;
        
        Debug.Log($"Projectile ricocheted! Count: {ricochetCount}, New direction: {direction}, New velocity: {newVelocity}");
        
        // Показываем эффект рикошета
        ShowRicochetEffect();
        
        // Если скорость стала слишком низкой или превышено количество рикошетов, уничтожаем снаряд
        if (rb.velocity.magnitude < minSpeedAfterRicochet || ricochetCount >= maxRicochets)
        {
            Debug.Log($"Projectile lost too much energy or exceeded ricochet limit, destroying");
            ShowDestroyEffect();
            DestroyProjectile();
        }
    }
    
    /// <summary>
    /// Показывает эффект рикошета
    /// </summary>
    void ShowRicochetEffect()
    {
        // Ищем HitEffect в сцене для показа эффекта рикошета
        HitEffect[] hitEffects = FindObjectsOfType<HitEffect>();
        foreach (HitEffect effect in hitEffects)
        {
            if (effect != null)
            {
                effect.ShowRicochetEffect(transform.position, -direction);
                break;
            }
        }
    }
    
    /// <summary>
    /// Показывает эффект уничтожения
    /// </summary>
    void ShowDestroyEffect()
    {
        // Ищем HitEffect в сцене для показа эффекта уничтожения
        HitEffect[] hitEffects = FindObjectsOfType<HitEffect>();
        foreach (HitEffect effect in hitEffects)
        {
            if (effect != null)
            {
                effect.ShowDestroyEffect(transform.position);
                break;
            }
        }
    }
    
    /// <summary>
    /// Уничтожает снаряд
    /// </summary>
    void DestroyProjectile()
    {
        Debug.Log("Projectile destroyed");
        Destroy(gameObject);
    }
    
    void OnDrawGizmos()
    {
        // Визуализация направления снаряда в редакторе
        if (isInitialized)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, direction * 2f);
        }
    }
} 