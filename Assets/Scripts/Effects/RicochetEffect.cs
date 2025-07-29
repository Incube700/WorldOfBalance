using UnityEngine;

namespace WorldOfBalance.Effects
{
    public class RicochetEffect : MonoBehaviour
    {
        [Header("Spark Settings")]
        [SerializeField] private GameObject sparkPrefab;
        [SerializeField] private int sparkCount = 8;
        [SerializeField] private float sparkSpeed = 5f;
        [SerializeField] private float sparkLifetime = 1f;
        
        [Header("Effect Settings")]
        [SerializeField] private float effectLifetime = 0.3f;
        [SerializeField] private float maxScale = 1.5f;
        [SerializeField] private float scaleSpeed = 3f;
        [SerializeField] private float fadeSpeed = 2f;
        
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
        /// Инициализирует эффект рикошета
        /// </summary>
        /// <param name="direction">Направление рикошета</param>
        public void Initialize(Vector2 direction)
        {
            currentLifetime = effectLifetime;
            
            // Поворачиваем эффект в направлении рикошета
            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            // Создаем искры
            CreateSparks();
            
            // Запускаем частицы, если есть
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
            
            Debug.Log($"RicochetEffect initialized in direction: {direction}");
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
        /// Создает искры для эффекта рикошета
        /// </summary>
        private void CreateSparks()
        {
            if (sparkPrefab == null) return;
            
            for (int i = 0; i < sparkCount; i++)
            {
                // Случайное направление для искры
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                
                // Создаем искру
                GameObject spark = Instantiate(sparkPrefab, transform.position, Quaternion.identity);
                
                // Настраиваем физику искры
                Rigidbody2D sparkRb = spark.GetComponent<Rigidbody2D>();
                if (sparkRb != null)
                {
                    sparkRb.velocity = direction * sparkSpeed;
                }
                
                // Уничтожаем искру через время
                Destroy(spark, sparkLifetime);
            }
        }
        
        /// <summary>
        /// Обновляет визуальный эффект
        /// </summary>
        private void UpdateEffect()
        {
            float progress = currentLifetime / effectLifetime;
            
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
        /// Создает эффект рикошета в указанной позиции
        /// </summary>
        /// <param name="position">Позиция эффекта</param>
        /// <param name="direction">Направление рикошета</param>
        public static void CreateRicochetEffect(Vector2 position, Vector2 direction)
        {
            GameObject effectPrefab = Resources.Load<GameObject>("Effects/RicochetEffect");
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
                RicochetEffect ricochetEffect = effect.GetComponent<RicochetEffect>();
                if (ricochetEffect != null)
                {
                    ricochetEffect.Initialize(direction);
                }
            }
            else
            {
                Debug.LogWarning("RicochetEffect prefab not found in Resources/Effects/");
            }
        }
        
        /// <summary>
        /// Получает информацию об эффекте
        /// </summary>
        /// <returns>Кортеж с информацией об эффекте</returns>
        public (float lifetime, float currentLifetime, float progress) GetEffectInfo()
        {
            float progress = effectLifetime > 0 ? currentLifetime / effectLifetime : 0f;
            return (effectLifetime, currentLifetime, progress);
        }
        
        /// <summary>
        /// Получает информацию о настройках искр
        /// </summary>
        /// <returns>Кортеж с информацией об искрах</returns>
        public (int sparkCount, float sparkSpeed, float sparkLifetime) GetSparkInfo()
        {
            return (sparkCount, sparkSpeed, sparkLifetime);
        }
    }
} 