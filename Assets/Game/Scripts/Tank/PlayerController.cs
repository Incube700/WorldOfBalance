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
        // Получаем вертикальный и горизонтальный ввод
        float vertical = moveInput.y;     // W/S - движение вперед/назад
        float horizontal = moveInput.x;   // A/D - поворот влево/вправо
        
        // Движение относительно локальной ориентации танка
        if (Mathf.Abs(vertical) > 0.1f)
        {
            // В 2D танк "смотрит" вправо (transform.right = forward)
            Vector2 forward = transform.right;
            rb.linearVelocity = forward * vertical * maxSpeed;
        }
        else
        {
            // Останавливаем танк если нет ввода движения
            rb.linearVelocity = Vector2.zero;
        }
        
        // Поворот вокруг собственной оси
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            // Поворот в 2D вокруг Z-оси
            transform.Rotate(0, 0, -horizontal * rotationSpeed * Time.deltaTime);
        }
    }
    
    void HandleShooting()
    {
        if (attackPressed && CanFire())
        {
            Fire();
            lastFireTime = Time.time;
            attackPressed = false;
        }
    }
    
    bool CanFire()
    {
        return Time.time - lastFireTime >= fireRate && !IsDead();
    }
    
    void Fire()
    {
        if (weapon != null)
        {
            weapon.SpawnProjectile(gameObject);
        }
        
        Debug.Log($"Fire effect played for {gameObject.name}");
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