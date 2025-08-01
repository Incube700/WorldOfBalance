using UnityEngine;

/// <summary>
/// Helper script to automatically set up FirePoint for tanks
/// </summary>
public class TankSetupHelper : MonoBehaviour
{
    [Header("FirePoint Setup")]
    [SerializeField] private float firePointDistance = 1.2f; // Расстояние до края турели/пушки
    [SerializeField] private bool createFirePointOnStart = true;
    [SerializeField] private bool createTurretIfMissing = true;
    
    void Start()
    {
        if (createFirePointOnStart)
        {
            SetupFirePoint();
        }
    }
    
    [ContextMenu("Setup FirePoint")]
    public void SetupFirePoint()
    {
        // Check if FirePoint already exists
        Transform existingFirePoint = transform.Find("FirePoint");
        if (existingFirePoint != null)
        {
            Debug.Log($"FirePoint already exists on {gameObject.name}");
            return;
        }
        
        // Create FirePoint GameObject with visual representation
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(transform);
        
        // Setup turret system
        Transform turret = SetupTurret();
        
        if (turret != null)
        {
            // Attach FirePoint to turret so it moves with turret rotation
            firePoint.transform.SetParent(turret);
            Debug.Log($"FirePoint attached to turret for {gameObject.name}");
        }
        
        // Position it in front of the tank/turret (assuming faces "up" in local space)
        firePoint.transform.localPosition = new Vector3(0, firePointDistance, 0);
        firePoint.transform.localRotation = Quaternion.identity;
        
        Debug.Log($"FirePoint created at local pos: {firePoint.transform.localPosition}, world pos: {firePoint.transform.position}");
        
        // Add visual representation - small cannon barrel
        CreateCannonBarrel(firePoint);
        
        // Update Weapon component if it exists
        Weapon weapon = GetComponent<Weapon>();
        if (weapon != null)
        {
            // Use reflection to set the firePoint field
            System.Reflection.FieldInfo firePointField = typeof(Weapon).GetField("firePoint", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (firePointField != null)
            {
                firePointField.SetValue(weapon, firePoint.transform);
            }
        }
        
        // Update TankController if it exists
        TankController tankController = GetComponent<TankController>();
        if (tankController != null)
        {
            // Use reflection to set the firePoint field
            System.Reflection.FieldInfo firePointField = typeof(TankController).GetField("firePoint", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (firePointField != null)
            {
                firePointField.SetValue(tankController, firePoint.transform);
            }
        }
        
        Debug.Log($"FirePoint with cannon barrel created for {gameObject.name}");
    }
    
    void CreateCannonBarrel(GameObject firePoint)
    {
        // Create cannon barrel visual that extends from tank center to FirePoint
        GameObject barrel = new GameObject("Cannon");
        barrel.transform.SetParent(transform); // Родитель - сам танк, не FirePoint
        barrel.transform.localPosition = Vector3.zero;
        barrel.transform.localRotation = Quaternion.identity;
        
        // Add SpriteRenderer for visual representation
        SpriteRenderer barrelRenderer = barrel.AddComponent<SpriteRenderer>();
        
        // Create longer cannon barrel that reaches to FirePoint
        int barrelWidth = 6;
        int barrelLength = Mathf.RoundToInt(firePointDistance * 100); // Длина до FirePoint
        Texture2D barrelTexture = new Texture2D(barrelWidth, barrelLength);
        Color[] pixels = new Color[barrelWidth * barrelLength];
        
        // Fill with dark gray color for cannon
        Color cannonColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = cannonColor;
        }
        
        barrelTexture.SetPixels(pixels);
        barrelTexture.Apply();
        
        // Create sprite from texture with pivot at bottom center
        Sprite barrelSprite = Sprite.Create(barrelTexture, new Rect(0, 0, barrelWidth, barrelLength), new Vector2(0.5f, 0f), 100f);
        barrelRenderer.sprite = barrelSprite;
        barrelRenderer.sortingOrder = 1; // Render on top of tank
        
        // Position barrel to start from tank center and extend to FirePoint
        barrel.transform.localPosition = new Vector3(0, firePointDistance * 0.5f, 0);
        
        Debug.Log($"Cannon barrel created: length={barrelLength}, positioned at {barrel.transform.localPosition}");
    }
    
    Transform SetupTurret()
    {
        // First, try to find existing turret
        Transform turret = transform.Find("Turret");
        TankController tankController = GetComponent<TankController>();
        
        // Try to get turret from TankController component
        if (turret == null && tankController != null)
        {
            System.Reflection.FieldInfo turretField = typeof(TankController).GetField("turret", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (turretField != null)
            {
                turret = (Transform)turretField.GetValue(tankController);
            }
        }
        
        // Create turret if missing and requested
        if (turret == null && createTurretIfMissing)
        {
            GameObject turretObj = new GameObject("Turret");
            turretObj.transform.SetParent(transform);
            turretObj.transform.localPosition = Vector3.zero;
            turretObj.transform.localRotation = Quaternion.identity;
            turret = turretObj.transform;
            
            // Assign turret to TankController if it exists
            if (tankController != null)
            {
                System.Reflection.FieldInfo turretField = typeof(TankController).GetField("turret", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (turretField != null)
                {
                    turretField.SetValue(tankController, turret);
                    Debug.Log($"Created and assigned turret to {gameObject.name}");
                }
            }
        }
        
        return turret;
    }
}