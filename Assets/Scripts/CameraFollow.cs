using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Follow Settings")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float lookAheadDistance = 2f;
    
    [Header("Bounds")]
    public bool useBounds = false;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;
    
    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    
    void Start()
    {
        if (target == null)
        {
            // Автоматически находим игрока
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        // Устанавливаем начальную позицию камеры
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Вычисляем желаемую позицию с учетом направления движения игрока
        Vector3 lookAhead = CalculateLookAhead();
        desiredPosition = target.position + offset + lookAhead;
        
        // Плавно перемещаем камеру
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Применяем границы, если они включены
        if (useBounds)
        {
            smoothedPosition = ClampToBounds(smoothedPosition);
        }
        
        transform.position = smoothedPosition;
    }
    
    Vector3 CalculateLookAhead()
    {
        // Получаем компонент Rigidbody2D игрока для определения направления движения
        Rigidbody2D playerRb = target.GetComponent<Rigidbody2D>();
        if (playerRb != null && playerRb.velocity.magnitude > 0.1f)
        {
            // Вычисляем опережение на основе направления движения
            Vector2 velocity = playerRb.velocity.normalized;
            return new Vector3(velocity.x, velocity.y, 0) * lookAheadDistance;
        }
        
        return Vector3.zero;
    }
    
    Vector3 ClampToBounds(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, minX, maxX),
            Mathf.Clamp(position.y, minY, maxY),
            position.z
        );
    }
    
    // Метод для установки новой цели
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    // Метод для изменения границ камеры
    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        useBounds = true;
    }
} 