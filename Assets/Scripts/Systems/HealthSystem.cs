using UnityEngine;
using System;

namespace WorldOfBalance.Systems
{
    public class HealthSystem : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        
        public event Action<float> OnHealthChanged;
        public event Action OnPlayerDied;
        
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float HealthPercentage => currentHealth / maxHealth;
        public bool IsDead => currentHealth <= 0f;
        
        private void Start()
        {
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Max(0f, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth);
            
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}");
            
            if (IsDead)
            {
                OnPlayerDied?.Invoke();
                Debug.Log($"{gameObject.name} died!");
                
                // Визуальный эффект смерти
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.gray;
                }
                
                // Отключаем компоненты
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = false;
                
                Collider2D collider = GetComponent<Collider2D>();
                if (collider != null) collider.enabled = false;
            }
        }
        
        public void Heal(float amount)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth);
        }
        
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth);
            
            // Восстанавливаем визуал
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                if (gameObject.CompareTag("Player"))
                    spriteRenderer.color = Color.blue;
                else if (gameObject.CompareTag("Enemy"))
                    spriteRenderer.color = Color.red;
            }
            
            // Включаем компоненты
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = true;
            
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = true;
        }
    }
} 