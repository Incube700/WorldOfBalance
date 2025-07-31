using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 1000f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float rotationSpeed = 180f;
    
    [Header("Combat Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Weapon weapon;
    
    private float lastFireTime;
    private Camera mainCamera;
    private Vector2 moveInput;
    private bool attackPressed;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (healthSystem == null) healthSystem = GetComponent<HealthSystem>();
        if (weapon == null) weapon = GetComponent<Weapon>();
        
        // Setup camera (не привязываем к игроку, чтобы она не вращалась)
        mainCamera = Camera.main;
        
        Debug.Log("PlayerController initialized");
    }
    

    
    void Update()
    {
        if (IsDead()) return;
        
        HandleInput();
        HandleMovement();
        HandleShooting();
        UpdateCamera();
    }
    
    void UpdateCamera()
    {
        // Камера следует за игроком, но не вращается
        if (mainCamera != null)
        {
            Vector3 targetPosition = transform.position + new Vector3(0, 0, -10);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * 5f);
        }
    }
    
    void HandleInput()
    {
        // Get movement input
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        // Handle attack input
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            attackPressed = true;
        }
    }
    
    void HandleMovement()
    {
        if (moveInput.magnitude > 0.1f)
        {
            // Apply force for movement
            rb.AddForce(moveInput.normalized * moveForce * Time.deltaTime);
            
            // Limit speed
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
            
            // Rotate towards movement direction
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.Euler(0, 0, angle), 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    void HandleShooting()
    {
        if (attackPressed && CanFire())
        {
            Vector2 fireDirection = GetFireDirection();
            Fire(fireDirection);
            lastFireTime = Time.time;
            attackPressed = false;
        }
    }
    
    bool CanFire()
    {
        return Time.time - lastFireTime >= fireRate && !IsDead();
    }
    
    Vector2 GetFireDirection()
    {
        if (mainCamera == null) return transform.right;
        
        // Fire towards mouse
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        return direction;
    }
    
    void Fire(Vector2 direction)
    {
        if (weapon != null)
        {
            weapon.SpawnProjectile(direction, gameObject);
        }
        
        Debug.Log($"Fire effect played for {gameObject.name} in direction {direction}");
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