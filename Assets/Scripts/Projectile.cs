using UnityEngine;

/// <summary>
/// Simple projectile that moves forward using transform.up * speed with Rigidbody2D
/// Perfect for minimalist tank arcade game
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int maxBounces = 4;
    
    private Rigidbody2D rb;
    private GameObject owner;
    private int bounceCount = 0;
    private float spawnTime;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnTime = Time.time;
        
        // Configure rigidbody
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        
        // Move in direction opposite from turret (backward from turret direction)
        Vector2 moveDirection = -transform.up; // Opposite to turret direction  
        rb.linearVelocity = moveDirection * speed;
        
        Debug.Log($"Projectile moving in direction: {moveDirection}, velocity: {rb.linearVelocity}, rotation: {transform.eulerAngles.z}°");
    }
    
    void Update()
    {
        // Destroy projectile after lifetime
        if (Time.time - spawnTime > lifetime)
        {
            DestroyProjectile();
        }
    }
    
    public void Initialize(GameObject projectileOwner)
    {
        owner = projectileOwner;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore collision with owner
        if (collision.gameObject == owner) return;
        
        // Check what we hit
        if (IsTarget(collision.gameObject))
        {
            HandleTargetHit(collision);
        }
        else
        {
            HandleWallBounce(collision);
        }
    }
    
    bool IsTarget(GameObject hitObject)
    {
        return hitObject.GetComponent<PlayerController>() != null || 
               hitObject.GetComponent<TankController>() != null || 
               hitObject.GetComponent<EnemyAI>() != null;
    }
    
    void HandleTargetHit(Collision2D collision)
    {
        Vector2 hitPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position;
        
        // Deal damage to different tank types
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        TankController tank = collision.gameObject.GetComponent<TankController>();
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        
        if (player != null && !player.IsDead())
        {
            player.TakeDamage(damage, hitPoint, owner);
        }
        else if (tank != null && !tank.IsDead)
        {
            tank.TakeDamage(damage, hitPoint, rb.linearVelocity.normalized);
        }
        else if (enemy != null)
        {
            HealthSystem enemyHealth = enemy.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, hitPoint, owner);
            }
        }
        
        DestroyProjectile();
    }
    
    void HandleWallBounce(Collision2D collision)
    {
        if (bounceCount >= maxBounces)
        {
            DestroyProjectile();
            return;
        }
        
        if (collision.contacts.Length == 0)
        {
            DestroyProjectile();
            return;
        }
        
        // Calculate reflection
        Vector2 hitNormal = collision.contacts[0].normal;
        Vector2 incomingDirection = rb.linearVelocity.normalized;
        Vector2 reflectedDirection = Vector2.Reflect(incomingDirection, hitNormal);
        
        // Apply reflected velocity (maintain opposite direction concept)  
        rb.linearVelocity = reflectedDirection * speed;
        
        // Update rotation to match new direction
        float angle = Mathf.Atan2(reflectedDirection.y, reflectedDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        bounceCount++;
    }
    
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}