using UnityEngine;
using Mirror;

public class ProjectileSpawner : NetworkBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float spawnOffset = 1f;
    
    void Start()
    {
        if (firePoint == null)
        {
            // Create fire point if not assigned
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(spawnOffset, 0, 0);
            firePoint = firePointObj.transform;
        }
    }
    
    public void SpawnProjectile(Vector2 direction, GameObject owner)
    {
        if (!isServer) return;
        
        // Calculate spawn position
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        spawnPosition += (Vector3)(direction * 0.5f); // Small offset from tank
        
        // Spawn projectile on server
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        // Set up projectile
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction, owner);
        }
        
        // Spawn on all clients
        NetworkServer.Spawn(projectileObj);
        
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
        if (!isServer) return;
        
        // Calculate spawn position
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        spawnPosition += (Vector3)(direction * 0.5f);
        
        // Spawn projectile
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        // Set up projectile with custom settings
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction, owner);
            
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
        
        // Spawn on all clients
        NetworkServer.Spawn(projectileObj);
    }
}