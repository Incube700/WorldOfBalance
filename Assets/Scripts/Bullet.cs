using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 2f; // 2 HP per tank as specified
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float directHitAngle = 45f; // Angle threshold for direct hits (degrees)
    [SerializeField] private int maxBounces = 5;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D circleCollider;
    
    private int bounceCount = 0;
    private GameObject owner;
    private float spawnTime;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (circleCollider == null) circleCollider = GetComponent<CircleCollider2D>();
        
        spawnTime = Time.time;
    }
    
    public void Initialize(Vector2 direction, GameObject projectileOwner)
    {
        owner = projectileOwner;
        
        // Set velocity for bullet movement
        rb.linearVelocity = direction * speed;
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
        // Safety check - ensure we have contact points
        if (collision.contacts.Length == 0)
        {
            Debug.LogWarning("No contact points found for tank hit - destroying bullet");
            DestroyBullet();
            return;
        }
        
        // Get hit point and normal
        Vector2 hitPoint = collision.contacts[0].point;
        Vector2 hitNormal = collision.contacts[0].normal;
        
        // Calculate angle of impact
        Vector2 bulletDirection = rb.linearVelocity.normalized;
        float impactAngle = Vector2.Angle(-bulletDirection, hitNormal);
        
        // Check if it's a direct hit (angle is close to perpendicular)
        if (impactAngle <= directHitAngle)
        {
            // Direct hit - deal damage
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
            
            if (player != null && !player.IsDead())
            {
                player.TakeDamage(damage, hitPoint, owner);
                // Add hit feedback - flash the tank
                StartCoroutine(FlashTank(player.GetComponent<SpriteRenderer>()));
            }
            else if (enemy != null)
            {
                // Enemy takes damage (assuming EnemyAI has health system)
                HealthSystem enemyHealth = enemy.GetComponent<HealthSystem>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage, hitPoint, owner);
                    // Add hit feedback - flash the enemy tank
                    StartCoroutine(FlashTank(enemy.GetComponent<SpriteRenderer>()));
                }
            }
            
            DestroyBullet();
        }
        else
        {
            // Glancing hit - bounce off
            HandleWallBounce(collision);
        }
    }
    
    void HandleWallBounce(Collision2D collision)
    {
        if (bounceCount >= maxBounces)
        {
            DestroyBullet();
            return;
        }
        
        // Safety check - ensure we have contact points
        if (collision.contacts.Length == 0)
        {
            Debug.LogWarning("No contact points found for bounce - destroying bullet");
            DestroyBullet();
            return;
        }
        
        // Get contact info
        ContactPoint2D contact = collision.contacts[0];
        Vector2 hitNormal = contact.normal;
        Vector2 incomingDirection = rb.linearVelocity.normalized;
        
        // Calculate perfect reflection: angle of incidence = angle of reflection
        Vector2 reflectedDirection = Vector2.Reflect(incomingDirection, hitNormal);
        
        // Move bullet slightly away from wall to prevent getting stuck
        Vector2 separationOffset = hitNormal * 0.1f;
        transform.position = (Vector2)transform.position + separationOffset;
        
        // Apply reflected velocity with original speed
        rb.linearVelocity = reflectedDirection * speed;
        
        bounceCount++;
        
        Debug.Log($"Bullet bounced off {collision.gameObject.name}! Direction: {incomingDirection} -> {reflectedDirection}, Count: {bounceCount}/{maxBounces}");
    }
    
    void DestroyBullet()
    {
        Debug.Log("Bullet destroyed");
        Destroy(gameObject);
    }
    
    // Simple hit feedback - flash the tank when hit
    System.Collections.IEnumerator FlashTank(SpriteRenderer tankRenderer)
    {
        if (tankRenderer == null) yield break;
        
        Color originalColor = tankRenderer.color;
        Color flashColor = Color.white;
        
        // Flash white briefly
        tankRenderer.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        
        // Return to original color
        tankRenderer.color = originalColor;
    }
}