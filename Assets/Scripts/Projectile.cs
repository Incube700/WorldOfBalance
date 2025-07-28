using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float damage = 25f;
    public float lifetime = 5f;
    public float ricochetThreshold = 30f; // Угол для рикошета
    public float penetrationChance = 0.7f; // Базовый шанс пробития
    
    [Header("Physics")]
    public float bounceForce = 0.8f; // Сила отскока при рикошете
    public LayerMask collisionLayers = -1;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    private bool isInitialized = false;
    
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
    
    void HandleCollision(Collision2D collision)
    {
        // Получаем угол столкновения
        Vector2 normal = collision.contacts[0].normal;
        float collisionAngle = Vector2.Angle(direction, normal);
        
        Debug.Log($"Projectile collision with {collision.gameObject.name} at angle: {collisionAngle}°");
        
        // Проверяем, является ли объект игроком
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            HandlePlayerHit(player, collisionAngle);
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
    
    void HandlePlayerHit(PlayerController player, float collisionAngle)
    {
        // Вычисляем шанс пробития на основе угла
        float penetrationChance = CalculatePenetrationChance(collisionAngle);
        
        // Применяем урон игроку
        player.TakeDamage(damage, collisionAngle);
        
        // Проверяем пробитие
        if (Random.value <= penetrationChance)
        {
            Debug.Log($"Projectile penetrated player! Angle: {collisionAngle}°, Penetration chance: {penetrationChance}");
            // Снаряд проходит сквозь игрока
            return;
        }
        else
        {
            Debug.Log($"Projectile stopped by player armor. Angle: {collisionAngle}°, Penetration chance: {penetrationChance}");
            DestroyProjectile();
        }
    }
    
    void HandleWallHit(float collisionAngle, Vector2 normal)
    {
        // Если угол меньше порога рикошета - снаряд рикошетит
        if (collisionAngle < ricochetThreshold)
        {
            Ricochet(normal);
        }
        else
        {
            // Снаряд застревает в стене
            Debug.Log($"Projectile stuck in wall at angle: {collisionAngle}°");
            DestroyProjectile();
        }
    }
    
    float CalculatePenetrationChance(float collisionAngle)
    {
        // Нормализуем угол к 0-90 градусам
        float normalizedAngle = Mathf.Abs(collisionAngle - 90f);
        
        // Чем ближе угол к 90°, тем выше шанс пробития
        // При угле 90° шанс максимальный, при 0° - минимальный
        float angleFactor = normalizedAngle / 90f;
        float finalChance = penetrationChance * (1f - angleFactor * 0.5f);
        
        return Mathf.Clamp01(finalChance);
    }
    
    void Ricochet(Vector2 normal)
    {
        // Вычисляем направление отскока
        Vector2 reflection = Vector2.Reflect(direction, normal);
        
        // Применяем силу отскока
        Vector2 newVelocity = reflection * speed * bounceForce;
        
        // Обновляем направление и скорость
        direction = reflection.normalized;
        rb.velocity = newVelocity;
        
        Debug.Log($"Projectile ricocheted! New direction: {direction}, New velocity: {newVelocity}");
        
        // Уменьшаем урон после рикошета
        damage *= 0.7f;
        
        // Если скорость стала слишком низкой, уничтожаем снаряд
        if (rb.velocity.magnitude < speed * 0.3f)
        {
            Debug.Log("Projectile lost too much energy, destroying");
            DestroyProjectile();
        }
    }
    
    void DestroyProjectile()
    {
        // Здесь можно добавить эффекты уничтожения
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