using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TankController : MonoBehaviour
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
    
    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    
    private float lastFireTime;
    private Camera mainCamera;
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction lookAction;
    
    // Platform detection and input switching
    private bool useTouchInput;
    private bool isMobilePlatform;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool attackPressed;
    
    // Mobile input references
    private MobileInputController mobileInput;
    private Joystick mobileJoystick;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (healthSystem == null) healthSystem = GetComponent<HealthSystem>();
        if (armorSystem == null) armorSystem = GetComponent<ArmorSystem>();
        if (projectileSpawner == null) projectileSpawner = GetComponent<ProjectileSpawner>();
        
        // Setup camera
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(transform);
            mainCamera.transform.localPosition = new Vector3(0, 0, -10);
        }
        
        // Detect platform
        isMobilePlatform = Application.isMobilePlatform;
        Debug.Log($"Platform detected: {Application.platform}, Mobile: {isMobilePlatform}");
        
        // Platform-specific input setup
        useTouchInput = isMobilePlatform;
        
        if (useTouchInput)
        {
            SetupMobileInput();
        }
        else
        {
            SetupDesktopInput();
        }
        
        Debug.Log($"Local player input setup completed. Touch: {useTouchInput}");
    }
    
    void SetupDesktopInput()
    {
        // Setup input actions for desktop
        if (inputActions != null)
        {
            var playerActionMap = inputActions.FindActionMap("Player");
            if (playerActionMap != null)
            {
                moveAction = playerActionMap.FindAction("Move");
                attackAction = playerActionMap.FindAction("Attack");
                lookAction = playerActionMap.FindAction("Look");
                
                if (moveAction != null) moveAction.Enable();
                if (attackAction != null) attackAction.Enable();
                if (lookAction != null) lookAction.Enable();
                
                // Subscribe to input events
                if (attackAction != null)
                {
                    attackAction.performed += OnAttackPerformed;
                    attackAction.canceled += OnAttackCanceled;
                }
            }
        }
    }
    
    void SetupMobileInput()
    {
        // Find mobile input components
        mobileInput = FindObjectOfType<MobileInputController>();
        if (mobileInput != null)
        {
            mobileJoystick = mobileInput.GetComponentInChildren<Joystick>();
        }
        
        // Enable mobile UI
        if (mobileInput != null)
        {
            mobileInput.gameObject.SetActive(true);
        }
    }
    
    void Update()
    {
        if (IsDead()) return;
        
        HandleInput();
        HandleMovement();
        HandleShooting();
    }
    
    void HandleInput()
    {
        if (useTouchInput)
        {
            HandleMobileInput();
        }
        else
        {
            HandleDesktopInput();
        }
    }
    
    void HandleDesktopInput()
    {
        // Get input from Input System
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
        
        if (lookAction != null)
        {
            lookInput = lookAction.ReadValue<Vector2>();
        }
        
        // Fallback to legacy input for testing
        if (moveInput.magnitude < 0.1f)
        {
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        
        // Handle attack input
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            attackPressed = true;
        }
    }
    
    void HandleMobileInput()
    {
        // Get input from mobile joystick
        if (mobileJoystick != null)
        {
            moveInput = mobileJoystick.Direction;
        }
        
        // Handle touch input for shooting
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                attackPressed = true;
            }
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
    
    void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (CanFire())
        {
            Vector2 fireDirection = GetFireDirection();
            Fire(fireDirection);
            lastFireTime = Time.time;
        }
    }
    
    void OnAttackCanceled(InputAction.CallbackContext context)
    {
        // Handle attack release if needed
    }
    
    bool CanFire()
    {
        return Time.time - lastFireTime >= fireRate && !IsDead();
    }
    
    Vector2 GetFireDirection()
    {
        if (mainCamera == null) return transform.right;
        
        if (useTouchInput)
        {
            // For mobile, fire in movement direction or forward
            if (moveInput.magnitude > 0.1f)
            {
                return moveInput.normalized;
            }
            return transform.right;
        }
        else
        {
            // For desktop, fire towards mouse
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            
            Vector2 direction = (mouseWorldPos - transform.position).normalized;
            return direction;
        }
    }
    
    void Fire(Vector2 direction)
    {
        if (projectileSpawner != null)
        {
            projectileSpawner.SpawnProjectile(direction, gameObject);
        }
        
        // Play fire effects, sounds, etc.
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
    
    void OnDestroy()
    {
        // Clean up input actions
        if (moveAction != null) moveAction.Disable();
        if (attackAction != null) attackAction.Disable();
        if (lookAction != null) lookAction.Disable();
        
        if (attackAction != null)
        {
            attackAction.performed -= OnAttackPerformed;
            attackAction.canceled -= OnAttackCanceled;
        }
    }
}