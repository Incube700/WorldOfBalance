using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    
    [Header("Combat")]
    public float maxArmor = 50f;
    public float hp = 100f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    public float projectileSpeed = 10f;
    
    [Header("Armor Zones")]
    public float frontArmor = 50f;
    public float sideArmor = 30f;
    public float backArmor = 20f;
    
    private float nextFireTime;
    private Camera mainCamera;
    private Rigidbody2D rb;
    
    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }
    
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleShooting();
    }
    
    void HandleMovement()
    {
        // Альтернативное управление для второго игрока
        float horizontal = 0f;
        float vertical = 0f;
        
        if (Input.GetKey(KeyCode.I)) vertical = 1f;
        if (Input.GetKey(KeyCode.K)) vertical = -1f;
        if (Input.GetKey(KeyCode.J)) horizontal = -1f;
        if (Input.GetKey(KeyCode.L)) horizontal = 1f;
        
        Vector2 movement = new Vector2(horizontal, vertical).normalized * moveSpeed;
        rb.velocity = movement;
    }
    
    void HandleRotation()
    {
        // Для второго игрока можно использовать второй курсор или автоматическое наведение
        // Пока используем простую логику - игрок поворачивается в направлении движения
        if (rb.velocity.magnitude > 0.1f)
        {
            Vector2 direction = rb.velocity.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    void HandleShooting()
    {
        // Второй игрок стреляет правой кнопкой мыши
        if (Input.GetButton("Fire2") && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            
            if (projectileScript != null)
            {
                projectileScript.Initialize(projectileSpeed, transform.right);
            }
        }
    }
    
    public void TakeDamage(float damage, float hitAngle)
    {
        float armorValue = GetArmorValue(hitAngle);
        float finalDamage = Mathf.Max(0, damage - armorValue);
        
        hp -= finalDamage;
        
        Debug.Log($"Player 2 hit! Angle: {hitAngle}°, Armor: {armorValue}, Damage: {finalDamage}, HP: {hp}");
        
        if (hp <= 0)
        {
            Die();
        }
    }
    
    float GetArmorValue(float hitAngle)
    {
        // Normalize angle to 0-360
        hitAngle = (hitAngle + 360) % 360;
        
        // Front armor (0-45° and 315-360°)
        if (hitAngle <= 45 || hitAngle >= 315)
        {
            return frontArmor;
        }
        // Side armor (45-135° and 225-315°)
        else if ((hitAngle > 45 && hitAngle <= 135) || (hitAngle >= 225 && hitAngle < 315))
        {
            return sideArmor;
        }
        // Back armor (135-225°)
        else
        {
            return backArmor;
        }
    }
    
    void Die()
    {
        Debug.Log("Player 2 died!");
        // Add death logic here
        gameObject.SetActive(false);
    }
} 