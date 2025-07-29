using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 90f; // градусов в секунду
    
    [Header("Combat")]
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform target;
    
    private float lastFireTime;
    private Rigidbody2D rb;
    private Transform firePoint;
    private EnemyTurretController turret;
    private float targetRotation;
    private bool isRotating = false;
    
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
        HandleRotation();
    }
    
    private void HandleMovement()
    {
        if (target == null) return;
        
        Vector2 direction = (target.position - transform.position).normalized;
        Vector2 movement = direction * moveSpeed;
        rb.linearVelocity = movement;
    }
    
    private void HandleRotation()
    {
        if (!isRotating) return;
        
        float currentRotation = transform.eulerAngles.z;
        float newRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
        
        if (Mathf.Abs(Mathf.DeltaAngle(currentRotation, targetRotation)) < 1f)
        {
            isRotating = false;
        }
        
        Debug.Log($"Enemy: Rotating to {targetRotation:F1}°, Current: {newRotation:F1}°");
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
        return turret != null && turret.IsAimedAtTarget();
    }
    
    private void FireProjectile()
    {
        if (projectilePrefab == null || turret == null) return;
        
        lastFireTime = Time.time;
        
        Transform firePoint = turret.GetFirePoint();
        Vector2 fireDirection = turret.GetFireDirection();
        
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(fireDirection, 8f, 8f, 40f, gameObject);
        }
        
        Debug.Log("Enemy fired projectile!");
    }
    
    public void RotateTowardsDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetRotation = angle;
        isRotating = true;
        
        Debug.Log($"Enemy: Starting rotation to {targetRotation:F1}°");
    }
}