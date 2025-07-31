using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float detectionRange = 10f;
    
    [Header("Combat")]
    [SerializeField] private float fireRate = 0.8f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TankController tankController;
    
    private float lastFireTime;
    private Transform player;
    private Vector2 lastKnownPlayerPosition;
    private bool playerInRange;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (tankController == null) tankController = GetComponent<TankController>();
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        HandleAI();
    }
    
    void HandleAI()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= detectionRange;
        
        if (playerInRange)
        {
            lastKnownPlayerPosition = player.position;
            
            // Move towards player
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            rb.AddForce(directionToPlayer * moveSpeed);
            
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
            // Patrol behavior - move to last known position
            Vector2 directionToLastKnown = (lastKnownPlayerPosition - (Vector2)transform.position).normalized;
            if (Vector2.Distance(transform.position, lastKnownPlayerPosition) > 1f)
            {
                rb.AddForce(directionToLastKnown * moveSpeed * 0.5f);
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
        
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        spawnPosition += (Vector3)(direction * 0.5f);
        
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction, gameObject);
        }
        
        Debug.Log($"Enemy fired projectile in direction: {direction}");
    }
} 