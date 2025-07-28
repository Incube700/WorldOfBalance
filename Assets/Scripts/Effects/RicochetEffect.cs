using UnityEngine;

namespace WorldOfBalance.Effects
{
    /// <summary>
    /// Эффект рикошета снаряда от поверхности
    /// </summary>
    public class RicochetEffect : MonoBehaviour
    {
        [Header("Effect Settings")]
        [SerializeField] private float lifetime = 1f;
        [SerializeField] private float sparkCount = 8;
        [SerializeField] private float sparkSpeed = 5f;
        [SerializeField] private float sparkLifetime = 0.5f;
        
        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer mainSprite;
        [SerializeField] private GameObject sparkPrefab;
        [SerializeField] private ParticleSystem sparkParticles;
        
        private float currentLifetime;
        private Vector3 originalScale;
        private Color originalColor;
        
        private void Start()
        {
            // Получаем компоненты, если они не назначены
            if (mainSprite == null)
                mainSprite = GetComponent<SpriteRenderer>();
                
            if (sparkParticles == null)
                sparkParticles = GetComponent<ParticleSystem>();
            
            // Сохраняем исходные значения
            originalScale = transform.localScale;
            if (mainSprite != null)
                originalColor = mainSprite.color;
            
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
            if (sparkParticles != null)
            {
                sparkParticles.Play();
            }
            
            // Создаем искры
            CreateSparks();
            
            // Устанавливаем начальный размер
            transform.localScale = Vector3.zero;
        }
        
        /// <summary>
        /// Создание искр
        /// </summary>
        private void CreateSparks()
        {
            if (sparkPrefab == null) return;
            
            for (int i = 0; i < sparkCount; i++)
            {
                // Случайное направление
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                
                // Создаем искру
                GameObject spark = Instantiate(sparkPrefab, transform.position, Quaternion.identity);
                
                // Добавляем движение
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
        /// Обновление эффекта
        /// </summary>
        private void UpdateEffect()
        {
            float progress = currentLifetime / lifetime;
            
            // Масштабирование
            float scale = Mathf.Lerp(0f, 1f, progress * 2f);
            transform.localScale = originalScale * scale;
            
            // Затухание
            if (mainSprite != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, progress * 3f);
                Color newColor = originalColor;
                newColor.a = alpha;
                mainSprite.color = newColor;
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
            if (mainSprite != null)
            {
                originalColor = color;
                mainSprite.color = color;
            }
        }
        
        /// <summary>
        /// Создание эффекта рикошета в указанной позиции
        /// </summary>
        public static void CreateRicochetEffect(Vector3 position, Vector2 direction, Color color)
        {
            // Создаем эффект
            GameObject effectPrefab = Resources.Load<GameObject>("Effects/RicochetEffect");
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
                RicochetEffect ricochetEffect = effect.GetComponent<RicochetEffect>();
                
                if (ricochetEffect != null)
                {
                    ricochetEffect.SetDirection(direction);
                    ricochetEffect.SetColor(color);
                }
            }
        }
    }
} 