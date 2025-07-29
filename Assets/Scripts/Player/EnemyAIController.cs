using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    
    [Header("Combat")]
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform target;
    
    private float lastFireTime;
    private Rigidbody2D rb;
    private Transform firePoint;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Создаем точку стрельбы
        GameObject firePointObj = new GameObject("FirePoint");
        firePointObj.transform.SetParent(transform);
        firePointObj.transform.localPosition = new Vector3(-0.6f, 0, 0); // Стреляем влево
        firePoint = firePointObj.transform;
        
        // Ищем игрока если цель не назначена
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
        
        // Простое преследование игрока
        Vector2 direction = (target.position - transform.position).normalized;
        Vector2 movement = direction * moveSpeed;
        rb.linearVelocity = movement;
        
        // Поворачиваемся к игроку
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    private void HandleShooting()
    {
        if (target == null) return;
        
        if (Time.time - lastFireTime >= fireRate)
        {
            FireProjectile();
        }
    }
    
    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;
        
        lastFireTime = Time.time;
        
        // Создаем снаряд
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        // Получаем компонент снаряда и инициализируем его
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            Vector2 direction = -transform.right; // Стреляем влево (к игроку)
            projectileComponent.Initialize(direction, 8f, 8f, 40f, gameObject);
        }
        
        Debug.Log("Enemy fired projectile!");
    }
}