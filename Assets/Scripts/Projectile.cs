using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 26f;
    [SerializeField] private float penetrationPower = 51f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float ricochetThreshold = 70f; // градусов
    [SerializeField] private int maxBounces = 3;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D circleCollider;
    
    private int bounceCount = 0;
    private float currentPenetrationPower;
    private GameObject owner;
    private float spawnTime;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (circleCollider == null) circleCollider = GetComponent<CircleCollider2D>();
        
        currentPenetrationPower = penetrationPower;
        spawnTime = Time.time;
    }
    
    public void Initialize(Vector2 direction, GameObject projectileOwner)
    {
        owner = projectileOwner;
        rb.linearVelocity = direction * speed;
    }
    
    void Update()
    {
        // Destroy if lifetime exceeded
        if (Time.time - spawnTime > lifetime)
        {
            DestroyProjectile();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit a tank
        TankController tank = collision.gameObject.GetComponent<TankController>();
        if (tank != null && tank.gameObject != owner)
        {
            HandleTankHit(collision);
        }
        else
        {
            // Check for ricochet
            HandleRicochet(collision);
        }
    }
    
    void HandleTankHit(Collision2D collision)
    {
        TankController tank = collision.gameObject.GetComponent<TankController>();
        if (tank == null || tank.IsDead()) return;
        
        // Get hit point and normal
        Vector2 hitPoint = collision.contacts[0].point;
        Vector2 hitNormal = collision.contacts[0].normal;
        
        // Calculate angle of impact
        Vector2 projectileDirection = rb.linearVelocity.normalized;
        float impactAngle = Vector2.Angle(projectileDirection, hitNormal);
        
        // Check if projectile can penetrate
        if (impactAngle < ricochetThreshold && currentPenetrationPower > 0)
        {
            // Deal damage
            float finalDamage = CalculateDamage(impactAngle);
            tank.TakeDamage(finalDamage, hitPoint, owner);
            
            // Reduce penetration power
            currentPenetrationPower -= 10f;
            
            // Destroy projectile if no penetration power left
            if (currentPenetrationPower <= 0)
            {
                DestroyProjectile();
                return;
            }
        }
        
        // Ricochet or destroy
        if (bounceCount < maxBounces)
        {
            HandleRicochet(collision);
        }
        else
        {
            DestroyProjectile();
        }
    }
    
    void HandleRicochet(Collision2D collision)
    {
        if (bounceCount >= maxBounces)
        {
            DestroyProjectile();
            return;
        }
        
        // Calculate ricochet direction
        Vector2 hitNormal = collision.contacts[0].normal;
        Vector2 incomingDirection = rb.linearVelocity.normalized;
        Vector2 ricochetDirection = Vector2.Reflect(incomingDirection, hitNormal);
        
        // Apply ricochet velocity
        rb.linearVelocity = ricochetDirection * speed;
        
        bounceCount++;
        
        // Reduce penetration power after ricochet
        currentPenetrationPower *= 0.7f;
        
        // Destroy if no penetration power left
        if (currentPenetrationPower <= 0)
        {
            DestroyProjectile();
        }
    }
    
    float CalculateDamage(float impactAngle)
    {
        // Calculate effective armor based on impact angle
        float effectiveArmor = 50f / Mathf.Cos(impactAngle * Mathf.Deg2Rad);
        
        // Calculate final damage
        float damageReduction = effectiveArmor / 100f;
        float finalDamage = damage * (1f - damageReduction);
        
        return Mathf.Max(0, finalDamage);
    }
    
    void DestroyProjectile()
    {
        if (isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}