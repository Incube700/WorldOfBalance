using UnityEngine;

public class EnemyAI : MonoBehaviour
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
    [SerializeField] private PlayerController playerController;
    
    private float lastFireTime;
    private Transform player;
    private Vector2 lastKnownPlayerPosition;
    private bool playerInRange;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (playerController == null) playerController = GetComponent<PlayerController>();
        
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
        if (player == null) return;
        
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
            // Simple forward movement when player not in range
            Vector2 forwardDirection = transform.right;
            rb.AddForce(forwardDirection * moveSpeed * 0.3f);
            
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
        
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        spawnPosition += (Vector3)(direction * 0.5f);
        
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Bullet bullet = projectileObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            // Передаем GameObject owner (this.gameObject) как второй параметр
            bullet.Initialize(direction, gameObject);
        }
        
        Debug.Log($"Enemy fired bullet in direction: {direction}");
    }
} 