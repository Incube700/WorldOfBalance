using UnityEngine;
using Mirror;

namespace WorldOfBalance
{
    public class CameraFollow : NetworkBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] private float minDistance = 0.1f;
        
        private Transform target;
        private Camera playerCamera;
        private Vector3 desiredPosition;
        private Vector3 smoothedPosition;
        
        private void Awake()
        {
            playerCamera = GetComponent<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("CameraFollow: Camera component not found!");
                enabled = false;
                return;
            }
        }
        
        private void Start()
        {
            // Отключаем камеру на удаленных клиентах
            if (!isLocalPlayer && !isServer)
            {
                if (playerCamera != null)
                {
                    playerCamera.enabled = false;
                }
                enabled = false;
                return;
            }
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                Debug.Log($"CameraFollow: Target set to {target.name}");
            }
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            // Вычисляем желаемую позицию
            desiredPosition = target.position + offset;
            
            // Плавно перемещаем камеру
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            
            // Проверяем минимальное расстояние для оптимизации
            if (Vector3.Distance(transform.position, smoothedPosition) > minDistance)
            {
                transform.position = smoothedPosition;
            }
        }
        
        // Метод для сброса камеры в начальную позицию
        public void ResetCamera()
        {
            if (target != null)
            {
                transform.position = target.position + offset;
            }
        }
        
        // Метод для изменения настроек камеры
        public void SetCameraSettings(float newSmoothSpeed, Vector3 newOffset)
        {
            smoothSpeed = newSmoothSpeed;
            offset = newOffset;
        }
    }
}