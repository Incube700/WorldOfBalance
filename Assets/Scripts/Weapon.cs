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
    
    public void SpawnProjectile(Vector2 direction, GameObject owner)
    {
        // Spawn bullet directly from FirePoint (no additional offset needed)
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        
        // Spawn bullet at barrel tip
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        // Set up bullet
        Bullet bullet = projectileObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(direction, owner);
        }
        
        Debug.Log($"Projectile spawned by {owner.name} in direction {direction}");
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
    
    // Method to spawn projectile with custom settings
    public void SpawnProjectileWithSettings(Vector2 direction, GameObject owner, float customSpeed = -1f, float customDamage = -1f)
    {
        // Calculate spawn position
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        spawnPosition += (Vector3)(direction * 0.5f);
        
        // Spawn projectile
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        // Set up bullet with custom settings
        Bullet bullet = projectileObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(direction, owner);
            
            // Apply custom settings if provided
            if (customSpeed > 0)
            {
                Rigidbody2D rb = projectileObj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * customSpeed;
                }
            }
        }
    }
}