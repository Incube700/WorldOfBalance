using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 90f; // градусов в секунду
    
    [Header("Combat Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("References")]
    [SerializeField] private TurretController turret;
    
    private Rigidbody2D rb;
    private float lastFireTime;
    private float targetRotation;
    private bool isRotating = false;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Находим пушку, если не назначена
        if (turret == null)
            turret = GetComponentInChildren<TurretController>();
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
        
        Vector2 movement = new Vector2(horizontal, vertical).normalized * moveSpeed;
        rb.linearVelocity = movement;
        
        // Если нет движения, обрабатываем поворот корпуса
        if (movement.magnitude < 0.1f && isRotating)
        {
            HandleRotation();
        }
    }
    
    private void HandleRotation()
    {
        if (!isRotating) return;
        
        float currentRotation = transform.eulerAngles.z;
        float newRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
        
        // Если достигли целевого угла, прекращаем поворот
        if (Mathf.Abs(Mathf.DeltaAngle(currentRotation, targetRotation)) < 1f)
        {
            isRotating = false;
        }
        
        Debug.Log($"Player: Rotating to {targetRotation:F1}°, Current: {newRotation:F1}°");
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
        return Time.time - lastFireTime >= fireRate && turret != null && turret.IsAimedAtTarget();
    }
    
    private void FireProjectile()
    {
        if (projectilePrefab == null || turret == null) return;
        
        lastFireTime = Time.time;
        
        // Получаем точку выстрела и направление от пушки
        Transform firePoint = turret.GetFirePoint();
        Vector2 fireDirection = turret.GetFireDirection();
        
        // Создаем снаряд
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(fireDirection, 10f, 10f, 50f, gameObject);
        }
        
        Debug.Log($"Player fired projectile! Direction: {fireDirection}");
    }
    
    public void RotateTowardsDirection(Vector2 direction)
    {
        // Вычисляем целевой угол поворота
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetRotation = angle;
        isRotating = true;
        
        Debug.Log($"Player: Starting rotation to {targetRotation:F1}°");
    }
}