using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float penetrationPower = 50f;
    [SerializeField] private float lifetime = 10f;
    
    [Header("Ricochet Settings")]
    [SerializeField] private float ricochetThreshold = 70f;
    [SerializeField] private int maxBounces = 3;
    
    private Vector2 direction;
    private GameObject owner;
    private Rigidbody2D rb;
    private int bounceCount = 0;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }
    
    public void Initialize(Vector2 dir, float spd, float dmg, float penetration, GameObject ownerObj)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        penetrationPower = penetration;
        owner = ownerObj;
        
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Не сталкиваемся с владельцем снаряда
        if (collision.gameObject == owner) return;
        
        Vector2 normal = collision.contacts[0].normal;
        float collisionAngle = Vector2.Angle(-direction, normal);
        
        // Проверяем, нужно ли рикошетить
        if (collisionAngle > ricochetThreshold && bounceCount < maxBounces)
        {
            Ricochet(normal);
            return;
        }
        
        // Проверяем, попали ли в игрока или врага
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            HandlePlayerHit(collision.gameObject, normal);
        }
        else
        {
            // Попадание в стену или землю
            HandleWallHit(normal);
        }
    }
    
    private void Ricochet(Vector2 normal)
    {
        Vector2 reflectedDirection = Vector2.Reflect(direction, normal).normalized;
        direction = reflectedDirection;
        
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        
        bounceCount++;
        
        // Уменьшаем пробивную способность после рикошета
        penetrationPower *= 0.8f;
        
        Debug.Log($"Projectile ricocheted! Bounce count: {bounceCount}");
        
        // Если превышен лимит отскоков, уничтожаем снаряд
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
    }
    
    private void HandlePlayerHit(GameObject target, Vector2 normal)
    {
        // Получаем компонент здоровья цели
        var healthSystem = target.GetComponent<MonoBehaviour>();
        if (healthSystem != null && healthSystem.GetType().Name == "HealthSystem")
        {
            // Проверяем броню
            var armorSystem = target.GetComponent<MonoBehaviour>();
            if (armorSystem != null && armorSystem.GetType().Name == "ArmorSystem")
            {
                Vector2 playerForward = target.transform.right;
                
                // Используем reflection для вызова метода
                var method = armorSystem.GetType().GetMethod("GetEffectiveArmor");
                if (method != null)
                {
                    float effectiveArmor = (float)method.Invoke(armorSystem, new object[] { -direction, playerForward });
                    
                    if (penetrationPower >= effectiveArmor)
                    {
                        // Пробиваем броню
                        var takeDamageMethod = healthSystem.GetType().GetMethod("TakeDamage");
                        if (takeDamageMethod != null)
                        {
                            takeDamageMethod.Invoke(healthSystem, new object[] { damage });
                        }
                        Debug.Log($"Hit {target.name} for {damage} damage! Armor: {effectiveArmor}, Penetration: {penetrationPower}");
                    }
                    else
                    {
                        // Не пробиваем - рикошет
                        Debug.Log($"Projectile deflected by armor! Armor: {effectiveArmor}, Penetration: {penetrationPower}");
                        Ricochet(normal);
                        return;
                    }
                }
            }
            else
            {
                // Нет брони - наносим урон
                var takeDamageMethod = healthSystem.GetType().GetMethod("TakeDamage");
                if (takeDamageMethod != null)
                {
                    takeDamageMethod.Invoke(healthSystem, new object[] { damage });
                }
            }
        }
        
        Destroy(gameObject);
    }
    
    private void HandleWallHit(Vector2 normal)
    {
        // Простое отражение от стен
        Ricochet(normal);
    }
}