using UnityEngine;

namespace WorldOfBalance.Effects
{
    /// <summary>
    /// Эффект попадания снаряда в цель
    /// </summary>
    public class HitEffect : MonoBehaviour
    {
        [Header("Effect Settings")]
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private float fadeSpeed = 2f;
        [SerializeField] private float scaleSpeed = 3f;
        [SerializeField] private float maxScale = 2f;
        
        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem particleSystem;
        
        private float currentLifetime;
        private Vector3 originalScale;
        private Color originalColor;
        
        private void Start()
        {
            // Получаем компоненты, если они не назначены
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
                
            if (particleSystem == null)
                particleSystem = GetComponent<ParticleSystem>();
            
            // Сохраняем исходные значения
            originalScale = transform.localScale;
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
            
            // Запускаем эффект
            StartEffect();
        }
        
        private void Update()
        {
            currentLifetime += Time.deltaTime;
            
            // Обновляем эффект
            UpdateEffect();
            
            // Уничтожаем объект по истечении времени
            if (currentLifetime >= lifetime)
            {
                Destroy(gameObject);
            }
    }
    
    /// <summary>
        /// Запуск эффекта
    /// </summary>
        private void StartEffect()
        {
            // Запускаем систему частиц
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
            
            // Устанавливаем начальный размер
            transform.localScale = Vector3.zero;
    }
    
    /// <summary>
        /// Обновление эффекта
    /// </summary>
        private void UpdateEffect()
        {
            float progress = currentLifetime / lifetime;
            
            // Масштабирование
            float scale = Mathf.Lerp(0f, maxScale, progress * scaleSpeed);
            transform.localScale = originalScale * scale;
            
            // Затухание
            if (spriteRenderer != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, progress * fadeSpeed);
                Color newColor = originalColor;
                newColor.a = alpha;
                spriteRenderer.color = newColor;
            }
        }
        
        /// <summary>
        /// Установка направления эффекта
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
    }
    
    /// <summary>
        /// Установка цвета эффекта
    /// </summary>
        public void SetColor(Color color)
        {
            if (spriteRenderer != null)
            {
                originalColor = color;
                spriteRenderer.color = color;
            }
        }
    }
} 