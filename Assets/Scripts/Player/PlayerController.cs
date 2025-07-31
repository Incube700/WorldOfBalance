using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180f;
    
    [Header("Combat")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TankController tankController;
    
    private float lastFireTime;
    private Camera mainCamera;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (tankController == null) tankController = GetComponent<TankController>();
        
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(transform);
            mainCamera.transform.localPosition = new Vector3(0, 0, -10);
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector2 movement = new Vector2(horizontal, vertical);
        if (movement.magnitude > 0)
        {
            // Move
            rb.AddForce(movement * moveSpeed);
            
            // Rotate towards movement direction
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.AngleAxis(angle, Vector3.forward), 
                rotationSpeed * Time.deltaTime
            );
        }
        
        // Shooting
        if (Input.GetMouseButton(0) && CanFire())
        {
            Vector2 fireDirection = GetFireDirection();
            Fire(fireDirection);
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
        
        Debug.Log($"Fired projectile in direction: {direction}");
    }
} 