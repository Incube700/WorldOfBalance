using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 1000f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float rotationSpeed = 180f;
    
    [Header("Combat Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("References")]
    [SerializeField] private TurretController turret;
    
    private Rigidbody2D rb;
    private float lastFireTime;
    private Camera mainCamera;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Находим пушку, если не назначена
        if (turret == null)
            turret = GetComponentInChildren<TurretController>();
            
        // Находим камеру
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(transform);
            mainCamera.transform.localPosition = new Vector3(0, 0, -10);
        }
    }
    
    private void Update()
    {
        HandleMovement();
        HandleShooting();
    }
    
    private void HandleMovement()
    {
        // Получаем ввод для движения
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        if (movement.magnitude > 0.1f)
        {
            // Применяем силу для движения
            rb.AddForce(movement * moveForce * Time.deltaTime);
            
            // Ограничиваем скорость
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
            
            // Поворачиваем в направлении движения
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.Euler(0, 0, angle), 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    private void HandleShooting()
    {
        // Стреляем по клику мыши
        if (Input.GetMouseButtonDown(0) && CanFire())
        {
            FireProjectile();
        }
    }
    
    private bool CanFire()
    {
        return Time.time - lastFireTime >= fireRate;
    }
    
    private void FireProjectile()
    {
        if (projectilePrefab == null) return;
        
        lastFireTime = Time.time;
        
        // Получаем направление выстрела от мыши
        Vector2 fireDirection = GetFireDirection();
        
        // Создаем снаряд
        Vector3 spawnPosition = transform.position + (Vector3)(fireDirection * 0.5f);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(fireDirection, gameObject);
        }
        
        Debug.Log($"Player fired projectile! Direction: {fireDirection}");
    }
    
    private Vector2 GetFireDirection()
    {
        if (mainCamera == null) return transform.right;
        
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        return direction;
    }
    
    public void TakeDamage(float damage, Vector2 hitPoint, GameObject attacker)
    {
        Debug.Log($"{gameObject.name} took {damage} damage from {attacker.name}");
    }
    
    public bool IsDead()
    {
        return false; // Пока нет системы здоровья
    }
}