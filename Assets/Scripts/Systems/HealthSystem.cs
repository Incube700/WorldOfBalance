using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // Визуальный эффект получения урона
        if (spriteRenderer != null)
        {
            StartCoroutine(DamageFlash());
        }
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"{gameObject.name} healed {amount}. Health: {currentHealth}");
    }
    
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        Debug.Log($"{gameObject.name} health reset to {maxHealth}");
    }
    
    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        
        // Визуальный эффект смерти
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.gray;
        }
        
        // Отключаем компоненты
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
        
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        
        // Отключаем скрипты
        var playerController = GetComponent<PlayerController>();
        if (playerController != null) playerController.enabled = false;
        
        var enemyAI = GetComponent<EnemyAIController>();
        if (enemyAI != null) enemyAI.enabled = false;
    }
    
    private System.Collections.IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
    
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public float GetMaxHealth()
    {
        return maxHealth;
    }
} 