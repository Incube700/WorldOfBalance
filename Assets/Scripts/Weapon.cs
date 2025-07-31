using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float spawnOffset = 1f;
    
    void Start()
    {
        if (firePoint == null)
        {
            // Try to find existing FirePoint first
            Transform existingFirePoint = transform.Find("FirePoint");
            if (existingFirePoint != null)
            {
                firePoint = existingFirePoint;
            }
            else
            {
                // Create fire point if not assigned and not found
                GameObject firePointObj = new GameObject("FirePoint");
                firePointObj.transform.SetParent(transform);
                firePointObj.transform.localPosition = new Vector3(spawnOffset, 0, 0);
                firePoint = firePointObj.transform;
            }
        }
        
        Debug.Log($"Weapon initialized with FirePoint at: {firePoint.localPosition}");
    }
    
    public void SpawnProjectile(GameObject owner)
    {
        if (projectilePrefab == null || firePoint == null) return;
        
        // Spawn projectile at FirePoint position and rotation
        GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        Debug.Log($"Weapon spawning projectile at pos: {firePoint.position}, rotation: {firePoint.rotation.eulerAngles}");
        
        // Initialize bullet with owner and direction
        Bullet bullet = projectileObj.GetComponent<Bullet>();
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        
        if (bullet != null)
        {
            // Use FirePoint direction for precise control
            Vector2 fireDirection = firePoint.up;
            bullet.Initialize(fireDirection, owner);
        }
        else if (projectile != null)
        {
            // Use FirePoint direction for precise control
            Vector2 fireDirection = firePoint.up;
            projectile.Initialize(fireDirection, owner);
        }
        
        Debug.Log($"Projectile spawned by {owner.name} from FirePoint");
    }
    
    // Helper method to get fire direction from mouse position
    public Vector2 GetFireDirectionFromMouse()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return transform.right;
        
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        return direction;
    }
    
    // Helper method to get fire direction from transform
    public Vector2 GetFireDirectionFromTransform(Transform target)
    {
        if (target == null) return transform.right;
        
        Vector2 direction = (target.position - transform.position).normalized;
        return direction;
    }
    
    // Method to spawn projectile with custom settings - now simplified
    public void SpawnProjectileWithSettings(GameObject owner)
    {
        SpawnProjectile(owner);
    }
}