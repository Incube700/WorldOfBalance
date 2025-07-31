using UnityEngine;

/// <summary>
/// Новая система снарядов без физики - простое перемещение по направлению
/// Надежное движение с постоянной скоростью и правильными рикошетами
/// </summary>
public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 6f;
    public float lifeTime = 5f;
    public float damage = 25f;
    public int maxBounces = 4;
    
    [Header("Debug")]
    public bool showDebugRays = true;
    
    private Vector3 direction;
    private GameObject owner;
    private int bounceCount = 0;
    private float spawnTime;
    
    // Компоненты для столкновений
    private Collider2D col2D;
    private Collider col3D;
    
    void Start()
    {
        spawnTime = Time.time;
        
        // Получаем коллайдеры для проверки столкновений
        col2D = GetComponent<Collider2D>();
        col3D = GetComponent<Collider>();
        
        // Автоматически уничтожаем через время жизни
        Invoke(nameof(DestroySelf), lifeTime);
    }
    
    /// <summary>
    /// Инициализация снаряда с направлением и владельцем
    /// </summary>
    public void Init(Vector3 dir, GameObject bulletOwner)
    {
        direction = dir.normalized;
        owner = bulletOwner;
        
        // Поворачиваем снаряд в направлении полета для визуала
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
        
        Debug.Log($"BulletController initialized: direction={direction}, speed={speed}, owner={bulletOwner.name}");
    }
    
    void Update()
    {
        // Простое равномерное движение без физики
        if (direction != Vector3.zero)
        {
            transform.position += direction * speed * Time.deltaTime;
            
            if (showDebugRays)
            {
                Debug.DrawRay(transform.position, direction * 0.5f, Color.red);
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject, collision.bounds.center);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 hitPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position;
        Vector2 hitNormal = collision.contacts.Length > 0 ? collision.contacts[0].normal : Vector2.up;
        
        HandleCollisionWithNormal(collision.gameObject, hitPoint, hitNormal);
    }
    
    void OnTriggerEnter(Collider collision)
    {
        HandleCollision(collision.gameObject, collision.bounds.center);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position;
        Vector3 hitNormal = collision.contacts.Length > 0 ? collision.contacts[0].normal : Vector3.up;
        
        HandleCollisionWithNormal(collision.gameObject, hitPoint, hitNormal);
    }
    
    void HandleCollision(GameObject hitObject, Vector3 hitPoint)
    {
        // Игнорируем столкновение с владельцем
        if (hitObject == owner) return;
        
        // Проверяем, что это за объект
        if (IsTarget(hitObject))
        {
            // Попадание в танк - наносим урон
            DealDamage(hitObject, hitPoint);
            DestroySelf();
        }
        else if (IsWall(hitObject))
        {
            // Попадание в стену - пытаемся сделать рикошет
            // Без нормали используем простое отражение от центра объекта
            Vector3 wallCenter = hitObject.transform.position;
            Vector3 toWall = (wallCenter - transform.position).normalized;
            Vector3 reflectedDir = Vector3.Reflect(direction, -toWall);
            
            HandleBounce(reflectedDir);
        }
    }
    
    void HandleCollisionWithNormal(GameObject hitObject, Vector3 hitPoint, Vector3 hitNormal)
    {
        // Игнорируем столкновение с владельцем
        if (hitObject == owner) return;
        
        if (IsTarget(hitObject))
        {
            // Попадание в танк
            DealDamage(hitObject, hitPoint);
            DestroySelf();
        }
        else if (IsWall(hitObject))
        {
            // Рикошет от стены с правильной нормалью
            Vector3 reflectedDir = Vector3.Reflect(direction, hitNormal);
            HandleBounce(reflectedDir);
        }
    }
    
    void HandleBounce(Vector3 reflectedDirection)
    {
        if (bounceCount >= maxBounces)
        {
            Debug.Log($"Bullet reached max bounces ({maxBounces}), destroying");
            DestroySelf();
            return;
        }
        
        // Обновляем направление
        direction = reflectedDirection.normalized;
        bounceCount++;
        
        // Поворачиваем визуал в новом направлении
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
        
        Debug.Log($"Bullet bounced! New direction: {direction}, bounce #{bounceCount}/{maxBounces}");
    }
    
    bool IsTarget(GameObject obj)
    {
        // Проверяем, является ли объект целью (танк)
        return obj.GetComponent<TankController>() != null || 
               obj.GetComponent<PlayerController>() != null || 
               obj.GetComponent<EnemyAI>() != null;
    }
    
    bool IsWall(GameObject obj)
    {
        // Проверяем, является ли объект стеной
        return obj.CompareTag("Wall") || 
               obj.name.ToLower().Contains("wall") || 
               obj.name.ToLower().Contains("obstacle") ||
               obj.layer == LayerMask.NameToLayer("Wall");
    }
    
    void DealDamage(GameObject target, Vector3 hitPoint)
    {
        Debug.Log($"Bullet hit {target.name} for {damage} damage at {hitPoint}");
        
        // Попытка нанести урон разным типам танков
        TankController tank = target.GetComponent<TankController>();
        PlayerController player = target.GetComponent<PlayerController>();
        EnemyAI enemy = target.GetComponent<EnemyAI>();
        
        if (tank != null)
        {
            tank.TakeDamage(damage, hitPoint, direction);
        }
        else if (player != null)
        {
            player.TakeDamage(damage, hitPoint, owner);
        }
        else if (enemy != null)
        {
            HealthSystem enemyHealth = enemy.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, hitPoint, owner);
            }
        }
    }
    
    void DestroySelf()
    {
        Debug.Log($"Destroying bullet after {Time.time - spawnTime:F1}s, {bounceCount} bounces");
        Destroy(gameObject);
    }
    
    // Визуализация в редакторе
    void OnDrawGizmos()
    {
        if (direction != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, direction * 2f);
        }
    }
}