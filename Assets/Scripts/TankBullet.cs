using UnityEngine;

/// <summary>
/// Minimalistic bullet for tank duel game.
/// Handles movement, ricochet, and precise angle-based damage.
/// </summary>
public class TankBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float damage = 26f;
    [SerializeField] private float penetration = 51f;
    [SerializeField] private float lifetime = 8f;
    [SerializeField] private int maxBounces = 4;
    
    // Private variables
    private Rigidbody2D rb;
    private GameObject owner;
    private int bounceCount = 0;
    private float spawnTime;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnTime = Time.time;
        
        // Configure rigidbody for precise bullet physics
        rb.gravityScale = 0f;
        rb.linearDamping = 0f; // No drag for bullets
        rb.angularDamping = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    void Update()
    {
        // Destroy bullet after lifetime
        if (Time.time - spawnTime > lifetime)
        {
            DestroyBullet();
        }
    }
    
    public void Initialize(Vector2 direction, GameObject bulletOwner)
    {
        owner = bulletOwner;
        
        // Move in direction opposite from turret (backward from turret direction)
        Vector2 moveDirection = -direction.normalized; // Opposite to turret direction
        
        // Set bullet velocity
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed;
        }
        
        // Rotate bullet to face movement direction (for visual)
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        Debug.Log($"TankBullet initialized by {bulletOwner.name}, original direction: {direction}, move direction: {moveDirection}, speed: {speed}");
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore collision with owner
        if (collision.gameObject == owner)
            return;
        
        // Get collision data
        ContactPoint2D contact = collision.contacts[0];
        Vector2 hitPoint = contact.point;
        Vector2 hitNormal = contact.normal;
        Vector2 bulletDirection = rb.linearVelocity.normalized;
        
        // Check what we hit
        TankController tank = collision.gameObject.GetComponent<TankController>();
        
        if (tank != null)
        {
            HandleTankHit(tank, hitPoint, hitNormal, bulletDirection);
        }
        else
        {
            // Hit wall or obstacle - bounce
            HandleWallBounce(hitNormal, bulletDirection);
        }
    }
    
    void HandleTankHit(TankController tank, Vector2 hitPoint, Vector2 hitNormal, Vector2 bulletDirection)
    {
        // Calculate impact angle relative to tank surface
        float impactAngle = Vector2.Angle(-bulletDirection, hitNormal);
        
        Debug.Log($"Bullet hit {tank.name}. Impact angle: {impactAngle:F1}°");
        
        // Check penetration based on angle (30-degree rule)
        if (impactAngle <= 30f)
        {
            // Direct hit - penetrate and deal damage
            if (penetration >= tank.Armor)
            {
                tank.TakeDamage(damage, hitPoint, bulletDirection);
                Debug.Log($"Bullet penetrated {tank.name}'s armor! Damage: {damage}");
                DestroyBullet();
            }
            else
            {
                Debug.Log($"Bullet failed to penetrate {tank.name}'s armor. Penetration: {penetration} < Armor: {tank.Armor}");
                HandleWallBounce(hitNormal, bulletDirection);
            }
        }
        else
        {
            // Glancing hit - ricochet
            Debug.Log($"Bullet ricocheted off {tank.name}. Impact angle: {impactAngle:F1}° > 30°");
            HandleWallBounce(hitNormal, bulletDirection);
        }
    }
    
    void HandleWallBounce(Vector2 hitNormal, Vector2 bulletDirection)
    {
        // Check bounce limit
        if (bounceCount >= maxBounces)
        {
            Debug.Log("Bullet reached maximum bounces. Destroying.");
            DestroyBullet();
            return;
        }
        
        // Calculate reflection direction
        Vector2 reflectedDirection = Vector2.Reflect(bulletDirection, hitNormal);
        
        // Apply new velocity (maintain opposite direction concept)
        rb.linearVelocity = reflectedDirection * speed;
        
        // Update bullet rotation
        float angle = Mathf.Atan2(reflectedDirection.y, reflectedDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        bounceCount++;
        
        Debug.Log($"Bullet bounced! New direction: {reflectedDirection}, Bounces: {bounceCount}/{maxBounces}");
        
        // Visual feedback for ricochet
        CreateRicochetEffect();
    }
    
    void CreateRicochetEffect()
    {
        // Simple visual effect for ricochet
        GameObject effect = new GameObject("RicochetEffect");
        effect.transform.position = transform.position;
        
        SpriteRenderer sr = effect.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = Color.yellow;
        
        // Destroy effect after short time
        Destroy(effect, 0.3f);
    }
    
    void DestroyBullet()
    {
        Debug.Log("Bullet destroyed");
        Destroy(gameObject);
    }
    
    Sprite CreateCircleSprite()
    {
        int size = 16;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 1;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                pixels[y * size + x] = distance <= radius ? Color.white : Color.clear;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}