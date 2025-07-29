using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveForce = 800f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float rotationSpeed = 180f;
    
    [Header("Combat")]
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform target;
    
    private float lastFireTime;
    private Rigidbody2D rb;
    private Transform firePoint;
    private EnemyTurretController turret;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        turret = GetComponentInChildren<EnemyTurretController>();
        
        GameObject firePointObj = new GameObject("FirePoint");
        firePointObj.transform.SetParent(transform);
        firePointObj.transform.localPosition = new Vector3(-0.6f, 0, 0);
        firePoint = firePointObj.transform;
        
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }
    
    private void Update()
    {
        if (target == null) return;
        
        HandleMovement();
        HandleShooting();
    }
    
    private void HandleMovement()
    {
        if (target == null) return;
        
        Vector2 direction = (target.position - transform.position).normalized;
        
        // Применяем силу для движения
        rb.AddForce(direction * moveForce * Time.deltaTime);
        
        // Ограничиваем скорость
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        
        // Поворачиваем в направлении движения
        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.Euler(0, 0, angle), 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    private void HandleShooting()
    {
        if (target == null) return;
        
        if (Time.time - lastFireTime >= fireRate && CanFire())
        {
            FireProjectile();
        }
    }
    
    private bool CanFire()
    {
        return true; // Упрощенная проверка
    }
    
    private void FireProjectile()
    {
        if (projectilePrefab == null) return;
        
        lastFireTime = Time.time;
        
        // Получаем направление к цели
        Vector2 fireDirection = (target.position - transform.position).normalized;
        
        // Создаем снаряд
        Vector3 spawnPosition = transform.position + (Vector3)(fireDirection * 0.5f);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(fireDirection, gameObject);
        }
        
        Debug.Log("Enemy fired projectile!");
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