using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 800f; // Force applied for movement
    [SerializeField] private float maxSpeed = 6f; // Maximum tank speed
    [SerializeField] private float linearDrag = 2f; // Drag for slight inertia feel
    [SerializeField] private float rotationSpeed = 180f;
    
    [Header("Combat Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Weapon weapon;
    [SerializeField] private InputController inputController;
    
    private float lastFireTime;
    private Camera mainCamera;
    private Vector2 moveInput;
    private bool attackPressed;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (healthSystem == null) healthSystem = GetComponent<HealthSystem>();
        if (weapon == null) weapon = GetComponent<Weapon>();
        if (inputController == null) inputController = FindObjectOfType<InputController>();
        
        // Setup rigidbody for floaty tank physics
        rb.bodyType = RigidbodyType2D.Dynamic; // Change from Kinematic to Dynamic
        rb.gravityScale = 0f; // No gravity in top-down
        rb.linearDamping = linearDrag; // Add drag for inertia feel
        rb.angularDamping = 5f; // Prevent unwanted rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Tank rotates via script only
        
        // Setup camera (не привязываем к игроку, чтобы она не вращалась)
        mainCamera = Camera.main;
        
        Debug.Log("PlayerController initialized with floaty physics");
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
        if (inputController != null)
        {
            // Use InputController for both mobile and desktop input
            moveInput = inputController.MovementInput;
            attackPressed = inputController.FireInput;
        }
        else
        {
            // Fallback to direct input
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                attackPressed = true;
            }
        }
    }
    
    void HandleMovement()
    {
        if (moveInput.magnitude > 0.1f)
        {
            // Apply force for floaty tank movement with slight inertia
            Vector2 force = moveInput.normalized * moveForce;
            rb.AddForce(force);
            
            // Limit maximum speed to keep control responsive
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
            
            // Rotate towards movement direction - tank faces the direction it's moving
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
        // Tank fires in the direction it's facing (forward direction)
        return transform.right; // In 2D, transform.right is the forward direction when rotation is 0
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