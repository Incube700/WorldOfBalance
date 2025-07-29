using UnityEngine;
using Mirror;

public class TankController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 1000f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float rotationSpeed = 180f;
    
    [Header("Combat Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private ArmorSystem armorSystem;
    [SerializeField] private ProjectileSpawner projectileSpawner;
    
    private float lastFireTime;
    private Camera mainCamera;
    
    public override void OnStartLocalPlayer()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(transform);
            mainCamera.transform.localPosition = new Vector3(0, 0, -10);
        }
    }
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (healthSystem == null) healthSystem = GetComponent<HealthSystem>();
        if (armorSystem == null) armorSystem = GetComponent<ArmorSystem>();
        if (projectileSpawner == null) projectileSpawner = GetComponent<ProjectileSpawner>();
    }
    
    void Update()
    {
        if (!isLocalPlayer) return;
        
        HandleMovement();
        HandleShooting();
    }
    
    void HandleMovement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Movement
        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        if (movement.magnitude > 0.1f)
        {
            // Apply force for movement
            rb.AddForce(movement * moveForce * Time.deltaTime);
            
            // Limit speed
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
            
            // Rotate towards movement direction
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.Euler(0, 0, angle), 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && CanFire())
        {
            Vector2 fireDirection = GetFireDirection();
            CmdFire(fireDirection);
            lastFireTime = Time.time;
        }
    }
    
    bool CanFire()
    {
        return Time.time - lastFireTime >= fireRate;
    }
    
    Vector2 GetFireDirection()
    {
        if (mainCamera == null) return transform.right;
        
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        return direction;
    }
    
    [Command]
    void CmdFire(Vector2 direction)
    {
        if (projectileSpawner != null)
        {
            projectileSpawner.SpawnProjectile(direction, gameObject);
        }
    }
    
    public void TakeDamage(float damage, Vector2 hitPoint, GameObject attacker)
    {
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damage, hitPoint, attacker);
        }
    }
    
    public bool IsDead()
    {
        return healthSystem != null && healthSystem.IsDead();
    }
}