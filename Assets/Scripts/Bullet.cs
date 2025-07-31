using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float damage = 2f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int maxBounces = 4;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    
    private int bounceCount = 0;
    private GameObject owner;
    private float spawnTime;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        spawnTime = Time.time;
        
        // Configure rigidbody for consistent velocity
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        
        // Move forward in the direction the projectile is facing (proper direction)
        Vector2 moveDirection = transform.up; // Forward direction from turret
        rb.linearVelocity = moveDirection * speed;
        
        Debug.Log($"Bullet moving in direction: {moveDirection}, velocity: {rb.linearVelocity}, rotation: {transform.eulerAngles.z}°");
    }
    
    public void Initialize(GameObject projectileOwner)
    {
        owner = projectileOwner;
    }
    
    public void Initialize(Vector2 direction, GameObject projectileOwner)
    {
        owner = projectileOwner;
        
        // Override velocity with specific direction (useful for manual control)
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
        
        Debug.Log($"Bullet initialized with custom direction: {direction}, by {projectileOwner.name}");
    }
    
    void Update()
    {
        // Destroy bullet after lifetime expires
        if (Time.time - spawnTime > lifetime)
        {
            DestroyBullet();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore collisions with the owner (tank that fired this bullet)
        if (collision.gameObject == owner) return;
        
        // Check what we hit - tanks first
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        
        if (player != null || enemy != null)
        {
            // Hit a tank - handle tank collision
            HandleTankHit(collision);
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.name.Contains("Wall"))
        {
            // Hit a wall - bounce off
            HandleWallBounce(collision);
        }
        else
        {
            // Hit something else (ground, etc.) - also bounce instead of destroying
            Debug.Log($"Bullet hit: {collision.gameObject.name} - bouncing");
            HandleWallBounce(collision);
        }
    }
    
    void HandleTankHit(Collision2D collision)
    {
        // Get hit point
        Vector2 hitPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position;
        
        // Deal damage to tank
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
        
        DestroyBullet();
    }
    
    void HandleWallBounce(Collision2D collision)
    {
        if (bounceCount >= maxBounces)
        {
            DestroyBullet();
            return;
        }
        
        if (collision.contacts.Length == 0)
        {
            DestroyBullet();
            return;
        }
        
        // Get contact info and calculate reflection
        Vector2 hitNormal = collision.contacts[0].normal;
        Vector2 incomingDirection = rb.linearVelocity.normalized;
        Vector2 reflectedDirection = Vector2.Reflect(incomingDirection, hitNormal);
        
        // Apply reflected velocity (maintain opposite direction concept)
        rb.linearVelocity = reflectedDirection * speed;
        
        // Update transform rotation to match new direction
        float angle = Mathf.Atan2(reflectedDirection.y, reflectedDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        bounceCount++;
    }
    
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}