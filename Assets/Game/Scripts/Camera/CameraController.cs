using UnityEngine;

/// <summary>
/// Контроллер камеры для отслеживания танка в WorldOfBalance.
/// Обеспечивает плавное следование за игроком с ограничениями по границам уровня.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private bool findPlayerAutomatically = true;
    
    [Header("Camera Position")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float height = 10f;
    [SerializeField] private float angle = 0f; // Угол наклона камеры
    
    [Header("Movement Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private bool smoothFollow = true;
    
    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-20, -20);
    [SerializeField] private Vector2 maxBounds = new Vector2(20, 20);
    
    [Header("Camera Settings")]
    [SerializeField] private bool isOrthographic = true;
    [SerializeField] private float orthographicSize = 8f;
    [SerializeField] private float fieldOfView = 60f;
    
    private UnityEngine.Camera cam;
    private Vector3 targetPosition;
    private bool hasTarget;
    
    void Start()
    {
        InitializeCamera();
        FindTarget();
    }
    
    void InitializeCamera()
    {
        cam = GetComponent<UnityEngine.Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        if (cam != null)
        {
            // Set camera projection
            cam.orthographic = isOrthographic;
            if (isOrthographic)
            {
                cam.orthographicSize = orthographicSize;
            }
            else
            {
                cam.fieldOfView = fieldOfView;
            }
            
            Debug.Log("CameraController: Camera initialized");
        }
    }
    
    void FindTarget()
    {
        if (target != null)
        {
            hasTarget = true;
            return;
        }
        
        if (findPlayerAutomatically)
        {
            // Try to find player by tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                hasTarget = true;
                Debug.Log($"CameraController: Found player target: {player.name}");
                return;
            }
            
            // Try to find by component
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                target = playerController.transform;
                hasTarget = true;
                Debug.Log($"CameraController: Found player target by component: {playerController.name}");
                return;
            }
            
            Debug.LogWarning("CameraController: No player target found!");
        }
    }
    
    void LateUpdate()
    {
        if (!hasTarget && findPlayerAutomatically)
        {
            FindTarget();
        }
        
        if (hasTarget && target != null)
        {
            UpdateCameraPosition();
        }
    }
    
    void UpdateCameraPosition()
    {
        // Calculate target position
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -height; // Ensure proper Z distance for 2D
        
        // Apply angle rotation offset
        if (angle != 0)
        {
            Vector3 rotatedOffset = Quaternion.Euler(angle, 0, 0) * offset;
            desiredPosition = target.position + rotatedOffset;
        }
        
        // Apply boundaries if enabled
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        }
        
        // Move camera
        if (smoothFollow)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = desiredPosition;
        }
        
        // Update camera rotation for angle
        if (angle != 0)
        {
            Quaternion desiredRotation = Quaternion.Euler(angle, 0, 0);
            if (smoothFollow)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = desiredRotation;
            }
        }
    }
    
    /// <summary>
    /// Устанавливает новую цель для отслеживания
    /// </summary>
    /// <param name="newTarget">Новая цель</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        hasTarget = target != null;
        
        if (hasTarget)
        {
            Debug.Log($"CameraController: Target set to {target.name}");
        }
    }
    
    /// <summary>
    /// Устанавливает границы камеры
    /// </summary>
    /// <param name="min">Минимальные границы</param>
    /// <param name="max">Максимальные границы</param>
    public void SetBoundaries(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBoundaries = true;
        
        Debug.Log($"CameraController: Boundaries set to Min:{min}, Max:{max}");
    }
    
    /// <summary>
    /// Отключает ограничения границ
    /// </summary>
    public void DisableBoundaries()
    {
        useBoundaries = false;
    }
    
    /// <summary>
    /// Немедленно перемещает камеру к цели без анимации
    /// </summary>
    public void SnapToTarget()
    {
        if (hasTarget && target != null)
        {
            Vector3 snapPosition = target.position + offset;
            snapPosition.z = -height;
            
            if (useBoundaries)
            {
                snapPosition.x = Mathf.Clamp(snapPosition.x, minBounds.x, maxBounds.x);
                snapPosition.y = Mathf.Clamp(snapPosition.y, minBounds.y, maxBounds.y);
            }
            
            transform.position = snapPosition;
            
            if (angle != 0)
            {
                transform.rotation = Quaternion.Euler(angle, 0, 0);
            }
        }
    }
    
    /// <summary>
    /// Переключает режим камеры между ортографическим и перспективным
    /// </summary>
    public void ToggleProjectionMode()
    {
        if (cam != null)
        {
            isOrthographic = !isOrthographic;
            cam.orthographic = isOrthographic;
            
            if (isOrthographic)
            {
                cam.orthographicSize = orthographicSize;
            }
            else
            {
                cam.fieldOfView = fieldOfView;
            }
            
            Debug.Log($"CameraController: Switched to {(isOrthographic ? "Orthographic" : "Perspective")} mode");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (useBoundaries)
        {
            // Draw boundary rectangle
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, 0);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 1);
            Gizmos.DrawWireCube(center, size);
        }
        
        if (target != null)
        {
            // Draw line to target
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
            
            // Draw target position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, 0.5f);
        }
    }
}