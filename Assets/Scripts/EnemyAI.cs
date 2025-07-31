using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveForce = 600f; // Force applied for movement
    [SerializeField] private float maxSpeed = 5f; // Maximum enemy speed
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float detectionRange = 10f;
    
    [Header("Combat")]
    [SerializeField] private float fireRate = 0.8f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerController playerController;
    
    private float lastFireTime;
    private Transform player;
    private Vector2 lastKnownPlayerPosition;
    private bool playerInRange;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (playerController == null) playerController = GetComponent<PlayerController>();
        
        // Setup rigidbody for floaty enemy physics
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearDamping = 2f; // Same drag as player for consistent feel
        rb.angularDamping = 5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        Debug.Log("EnemyAI initialized with floaty physics");
    }
    
    void Update()
    {
        if (player == null) return;
        
        HandleAI();
    }
    
    void HandleAI()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= detectionRange;
        
        if (playerInRange)
        {
            lastKnownPlayerPosition = player.position;
            
            // Move towards player with force-based physics
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            rb.AddForce(directionToPlayer * moveForce);
            
            // Limit maximum speed to keep enemy responsive
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
            
            // Rotate towards player
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.AngleAxis(angle, Vector3.forward), 
                rotationSpeed * Time.deltaTime
            );
            
            // Shoot at player
            if (CanFire())
            {
                Vector2 fireDirection = (player.position - transform.position).normalized;
                Fire(fireDirection);
                lastFireTime = Time.time;
            }
        }
        else
        {
            // Simple forward movement when player not in range
            Vector2 forwardDirection = transform.right;
            rb.AddForce(forwardDirection * moveForce * 0.3f);
            
            // Limit speed even when wandering
            if (rb.linearVelocity.magnitude > maxSpeed * 0.5f)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed * 0.5f;
            }
            
            // Shoot forward occasionally
            if (CanFire() && Random.Range(0f, 1f) < 0.1f)
            {
                Fire(forwardDirection);
                lastFireTime = Time.time;
            }
        }
    }
    
    bool CanFire()
    {
        return Time.time - lastFireTime >= fireRate;
    }
    
    void Fire(Vector2 direction)
    {
        if (projectilePrefab == null) return;
        
        Vector3 spawnPosition;
        Quaternion spawnRotation;
        
        if (firePoint != null)
        {
            // Use FirePoint position and rotation (direction from turret)
            spawnPosition = firePoint.position;
            spawnRotation = firePoint.rotation;
        }
        else
        {
            // Fallback: calculate rotation from direction if no FirePoint
            spawnPosition = transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            spawnRotation = Quaternion.Euler(0, 0, angle);
        }
        
        // Spawn projectile with correct position and rotation
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
        
        Debug.Log($"Enemy spawning projectile at pos: {spawnPosition}, rotation: {spawnRotation.eulerAngles}");
        
        // Initialize bullet
        Bullet bullet = projectileObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(gameObject);
        }
        
        Debug.Log($"Enemy fired bullet from FirePoint direction");
    }
} 