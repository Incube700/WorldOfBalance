using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    [Header("Death Settings")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private Color deathColor = Color.gray;
    
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercentage => currentHealth / maxHealth;
    
    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (playerController == null) playerController = GetComponent<PlayerController>();
        
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage, Vector2 hitPoint, GameObject attacker)
    {
        if (IsDead()) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"{gameObject.name} took {damage} damage from {attacker.name}. Health: {currentHealth}");
        
        if (IsDead())
        {
            OnDeath();
        }
    }
    
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    
    void OnDeath()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        
        // Visual death effect
        if (spriteRenderer != null)
        {
            spriteRenderer.color = deathColor;
        }
        
        // Disable player controller
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Disable Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }
        
        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // НЕ удаляем объект, просто отключаем
        Debug.Log($"{gameObject.name} отключен (не удален)");
    }
    
    public void Heal(float amount)
    {
        if (IsDead()) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
    }
    
    public void Respawn()
    {
        currentHealth = maxHealth;
        
        // Reset visual
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        // Re-enable player controller
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Re-enable Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
        }
    }
}