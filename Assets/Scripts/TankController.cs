using UnityEngine;

/// <summary>
/// Minimalistic tank controller for precise 2D tank duel game.
/// Handles movement, turret rotation with 30-degree limit, and shooting.
/// </summary>
public class TankController : MonoBehaviour
{
    [Header("Tank Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 90f; // degrees per second
    [SerializeField] private float fireRate = 1f; // shots per second
    
    [Header("Turret Settings")]
    [SerializeField] private Transform turret;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float maxTurretAngle = 40f; // Maximum turret rotation from body
    [SerializeField] private float turretRotationSpeed = 180f; // degrees per second
    
    [Header("Combat Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float armor = 50f;
    [SerializeField] private float health = 50f;
    
    // Private variables
    private Rigidbody2D rb;
    private Camera mainCamera;
    private float lastFireTime;
    private float maxHealth;
    private bool isPlayer;
    private Transform target; // For AI targeting
    
    // Input
    private Vector2 moveInput;
    private Vector2 aimDirection;
    private bool fireInput;
    
    public float Health => health;
    public float MaxHealth => maxHealth;
    public float Armor => armor;
    public bool IsDead => health <= 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        maxHealth = health;
        
        // Configure rigidbody for precise 2D physics
        rb.gravityScale = 0f;
        rb.linearDamping = 5f; // Smooth stopping
        rb.angularDamping = 10f; // Prevent unwanted rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Tank rotates via script only
        
        // Determine if this is player or AI
        isPlayer = gameObject.CompareTag("Player");
        
        // Find target for AI
        if (!isPlayer)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }
        
        // Ensure turret exists
        if (turret == null)
        {
            turret = transform.Find("Turret");
            if (turret == null)
            {
                Debug.LogWarning($"No turret found for {gameObject.name}. Creating default turret.");
                CreateDefaultTurret();
            }
        }
        
        // Ensure fire point exists
        if (firePoint == null && turret != null)
        {
            firePoint = turret.Find("FirePoint");
            if (firePoint == null)
            {
                GameObject firePointObj = new GameObject("FirePoint");
                firePointObj.transform.SetParent(turret);
                firePointObj.transform.localPosition = new Vector3(0.6f, 0, 0); // At turret tip
                firePoint = firePointObj.transform;
            }
        }
        
        Debug.Log($"{gameObject.name} initialized. IsPlayer: {isPlayer}");
    }
    
    void Update()
    {
        if (IsDead) return;
        
        HandleInput();
        HandleMovement();
        HandleTurretRotation();
        HandleShooting();
    }
    
    void HandleInput()
    {
        if (isPlayer)
        {
            // Player input
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            
            // Mouse aiming
            if (mainCamera != null)
            {
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f;
                aimDirection = (mouseWorldPos - transform.position).normalized;
            }
            
            fireInput = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
        }
        else
        {
            // AI input
            if (target != null)
            {
                // Move towards target
                Vector2 directionToTarget = (target.position - transform.position).normalized;
                moveInput = directionToTarget;
                
                // Aim at target
                aimDirection = directionToTarget;
                
                // Fire if close enough
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                fireInput = distanceToTarget < 10f; // Fire within 10 units
            }
        }
    }
    
    void HandleMovement()
    {
        if (moveInput.magnitude > 0.1f)
        {
            // Move tank
            Vector2 movement = moveInput.normalized * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }
    
    void HandleTurretRotation()
    {
        if (turret == null || aimDirection == Vector2.zero) return;
        
        // Calculate desired turret angle relative to tank body
        float bodyAngle = transform.eulerAngles.z;
        float desiredAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float relativeAngle = Mathf.DeltaAngle(bodyAngle, desiredAngle);
        
        // Check if turret can rotate to desired angle
        if (Mathf.Abs(relativeAngle) <= maxTurretAngle)
        {
            // Turret can reach target - rotate turret
            float targetTurretAngle = desiredAngle;
            float currentTurretAngle = turret.eulerAngles.z;
            float newTurretAngle = Mathf.MoveTowardsAngle(currentTurretAngle, targetTurretAngle, turretRotationSpeed * Time.deltaTime);
            turret.rotation = Quaternion.Euler(0, 0, newTurretAngle);
        }
        else
        {
            // Turret can't reach target - rotate tank body
            float targetBodyAngle = desiredAngle - Mathf.Sign(relativeAngle) * maxTurretAngle;
            float currentBodyAngle = transform.eulerAngles.z;
            float newBodyAngle = Mathf.MoveTowardsAngle(currentBodyAngle, targetBodyAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newBodyAngle);
            
            // Set turret to maximum angle
            float turretAngle = transform.eulerAngles.z + Mathf.Sign(relativeAngle) * maxTurretAngle;
            turret.rotation = Quaternion.Euler(0, 0, turretAngle);
        }
    }
    
    void HandleShooting()
    {
        if (fireInput && CanFire())
        {
            Fire();
            lastFireTime = Time.time;
        }
    }
    
    bool CanFire()
    {
        return Time.time - lastFireTime >= (1f / fireRate);
    }
    
    void Fire()
    {
        if (bulletPrefab == null || firePoint == null) return;
        
        // Calculate fire direction from FirePoint
        Vector2 fireDirection = firePoint.up;
        
        // Use command pattern for firing
        FireCommand(fireDirection);
    }
    
    // Command method for firing (similar to [Command] in Mirror)
    void FireCommand(Vector2 direction)
    {
        // Spawn bullet at FirePoint position and rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        Debug.Log($"{gameObject.name} spawning bullet at pos: {firePoint.position}, rotation: {firePoint.rotation.eulerAngles}");
        
        // Initialize bullet - check for both bullet types
        Bullet simpleBullet = bullet.GetComponent<Bullet>();
        TankBullet tankBullet = bullet.GetComponent<TankBullet>();
        Projectile projectile = bullet.GetComponent<Projectile>();
        
        if (simpleBullet != null)
        {
            simpleBullet.Initialize(direction, gameObject);
        }
        else if (tankBullet != null)
        {
            tankBullet.Initialize(direction, gameObject);
        }
        else if (projectile != null)
        {
            projectile.Initialize(direction, gameObject);
        }
        
        Debug.Log($"{gameObject.name} fired projectile in direction: {direction}");
    }
    
    public void TakeDamage(float damage, Vector2 hitPoint, Vector2 hitDirection)
    {
        if (IsDead) return;
        
        // Calculate impact angle for armor effectiveness
        Vector2 tankForward = transform.right;
        float impactAngle = Vector2.Angle(-hitDirection, tankForward);
        
        // Only take damage if impact angle is <= 30 degrees (direct hit)
        if (impactAngle <= 30f)
        {
            health -= damage;
            health = Mathf.Max(0, health);
            
            Debug.Log($"{gameObject.name} took {damage} damage! Impact angle: {impactAngle:F1}°. Health: {health}");
            
            // Visual feedback
            StartCoroutine(DamageFlash());
            
            if (IsDead)
            {
                OnDeath();
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} deflected shot! Impact angle: {impactAngle:F1}° > 30°");
        }
    }
    
    void OnDeath()
    {
        Debug.Log($"{gameObject.name} destroyed!");
        
        // Disable components
        GetComponent<Collider2D>().enabled = false;
        rb.simulated = false;
        
        // Visual death effect
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.gray;
        
        // Disable this script
        enabled = false;
    }
    
    System.Collections.IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }
    }
    
    void CreateDefaultTurret()
    {
        GameObject turretObj = new GameObject("Turret");
        turretObj.transform.SetParent(transform);
        turretObj.transform.localPosition = Vector3.zero;
        
        // Add visual component (simple rectangle)
        SpriteRenderer turretRenderer = turretObj.AddComponent<SpriteRenderer>();
        turretRenderer.sprite = CreateRectangleSprite(0.8f, 0.2f);
        turretRenderer.color = Color.gray;
        
        turret = turretObj.transform;
    }
    
    Sprite CreateRectangleSprite(float width, float height)
    {
        // Create a simple rectangle sprite
        Texture2D texture = new Texture2D(Mathf.RoundToInt(width * 100), Mathf.RoundToInt(height * 100));
        Color[] pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }
}