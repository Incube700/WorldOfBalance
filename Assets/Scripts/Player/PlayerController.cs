using UnityEngine;

namespace WorldOfBalance.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        
        [Header("Combat")]
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        
        private float lastFireTime;
        private Rigidbody2D rb;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            if (firePoint == null)
            {
                // Создаем точку стрельбы если её нет
                GameObject firePointObj = new GameObject("FirePoint");
                firePointObj.transform.SetParent(transform);
                firePointObj.transform.localPosition = new Vector3(0.6f, 0, 0);
                firePoint = firePointObj.transform;
            }
        }
        
        private void Update()
        {
            HandleMovement();
            HandleShooting();
        }
        
        private void HandleMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            
            Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * moveSpeed;
            rb.velocity = movement;
        }
        
        private void HandleShooting()
        {
            if (Input.GetKeyDown(KeyCode.Space) && CanFire())
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
            if (projectilePrefab == null || firePoint == null) return;
            
            lastFireTime = Time.time;
            
            // Создаем снаряд в точке стрельбы
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            // Получаем компонент снаряда и инициализируем его
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                Vector2 direction = transform.right; // Стреляем вправо (вперед)
                projectileComponent.Initialize(direction, 10f, 10f, 50f, gameObject);
            }
            
            Debug.Log("Player fired projectile!");
        }
    }
}