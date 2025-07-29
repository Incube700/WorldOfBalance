using UnityEngine;

namespace WorldOfBalance.Effects
{
    public class HitEffect : MonoBehaviour
    {
        [Header("Effect Settings")]
        [SerializeField] private float lifetime = 0.5f;
        [SerializeField] private float maxScale = 2f;
        [SerializeField] private float scaleSpeed = 2f;
        [SerializeField] private float fadeSpeed = 1f;
        
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem particleSystem;
        
        // Local variables
        private float currentLifetime;
        private Vector3 originalScale;
        private Color originalColor;
        
        private void Awake()
        {
            // Получаем компоненты с типизацией
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (particleSystem == null)
                particleSystem = GetComponent<ParticleSystem>();
            
            // Сохраняем исходные значения
            originalScale = transform.localScale;
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
        }
        
        /// <summary>
        /// Инициализирует эффект попадания
        /// </summary>
        /// <param name="direction">Направление попадания</param>
        public void Initialize(Vector2 direction)
        {
            currentLifetime = lifetime;
            
            // Поворачиваем эффект в направлении попадания
            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            // Запускаем частицы, если есть
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
            
            Debug.Log($"HitEffect initialized in direction: {direction}");
        }
        
        private void Update()
        {
            UpdateEffect();
            
            // Уничтожаем эффект по истечении времени жизни
            if (currentLifetime <= 0)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Обновляет визуальный эффект
        /// </summary>
        private void UpdateEffect()
        {
            float progress = currentLifetime / lifetime;
            
            // Масштабирование
            float scale = Mathf.Lerp(0f, maxScale, progress * scaleSpeed);
            transform.localScale = originalScale * scale;
            
            // Прозрачность для спрайта
            if (spriteRenderer != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, progress * fadeSpeed);
                Color newColor = originalColor;
                newColor.a = alpha;
                spriteRenderer.color = newColor;
            }
            
            currentLifetime -= Time.deltaTime;
        }
        
        /// <summary>
        /// Создает эффект попадания в указанной позиции
        /// </summary>
        /// <param name="position">Позиция эффекта</param>
        /// <param name="direction">Направление попадания</param>
        public static void CreateHitEffect(Vector2 position, Vector2 direction)
        {
            GameObject effectPrefab = Resources.Load<GameObject>("Effects/HitEffect");
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
                HitEffect hitEffect = effect.GetComponent<HitEffect>();
                if (hitEffect != null)
                {
                    hitEffect.Initialize(direction);
                }
            }
            else
            {
                Debug.LogWarning("HitEffect prefab not found in Resources/Effects/");
            }
        }
        
        /// <summary>
        /// Получает информацию об эффекте
        /// </summary>
        /// <returns>Кортеж с информацией об эффекте</returns>
        public (float lifetime, float currentLifetime, float progress) GetEffectInfo()
        {
            float progress = lifetime > 0 ? currentLifetime / lifetime : 0f;
            return (lifetime, currentLifetime, progress);
        }
    }
} 