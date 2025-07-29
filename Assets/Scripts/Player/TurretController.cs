using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Turret Settings")]
    [SerializeField] private float maxRotationAngle = 70f;
    [SerializeField] private float rotationSpeed = 180f; // градусов в секунду
    
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform firePoint;
    
    private Camera mainCamera;
    private PlayerController playerController;
    
    private void Start()
    {
        mainCamera = Camera.main;
        playerController = GetComponentInParent<PlayerController>();
        
        if (playerTransform == null)
            playerTransform = transform.parent; // Set to parent transform if not assigned
    }
    
    private void Update()
    {
        HandleTurretRotation();
    }
    
    private void HandleTurretRotation()
    {
        if (mainCamera == null || playerTransform == null) return;
        
        // Получаем позицию мыши в мировых координатах
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        
        // Вектор от пушки к мыши
        Vector2 directionToMouse = (mouseWorldPosition - transform.position).normalized;
        
        // Направление корпуса игрока (вправо)
        Vector2 playerForward = playerTransform.right;
        
        // Угол между направлением игрока и курсором
        float angleToMouse = Vector2.SignedAngle(playerForward, directionToMouse);
        
        // Ограничиваем угол поворота пушки
        float clampedAngle = Mathf.Clamp(angleToMouse, -maxRotationAngle, maxRotationAngle);
        
        // Если угол превышает лимит, просто ограничиваем поворот пушки
        if (Mathf.Abs(angleToMouse) > maxRotationAngle)
        {
            clampedAngle = Mathf.Sign(angleToMouse) * maxRotationAngle;
        }
        
        // Поворачиваем пушку
        float targetRotation = clampedAngle;
        float currentRotation = transform.eulerAngles.z;
        
        // Интерполируем поворот для плавности
        float newRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
        
        Debug.Log($"Turret: Angle to mouse: {angleToMouse:F1}°, Clamped: {clampedAngle:F1}°, Rotation: {newRotation:F1}°");
    }
    
    public bool IsAimedAtTarget()
    {
        if (mainCamera == null || playerTransform == null) return false;
        
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        
        Vector2 directionToMouse = (mouseWorldPosition - transform.position).normalized;
        Vector2 playerForward = playerTransform.right;
        float angleToMouse = Vector2.SignedAngle(playerForward, directionToMouse);
        
        return Mathf.Abs(angleToMouse) <= maxRotationAngle;
    }
    
    public Vector2 GetFireDirection()
    {
        return transform.right;
    }
    
    public Transform GetFirePoint()
    {
        return firePoint != null ? firePoint : transform;
    }
}