using UnityEngine;
using Mirror;

public class HealthSystem : NetworkBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SyncVar] private float currentHealth;
    
    [Header("Death Settings")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private Color deathColor = Color.gray;
    
    private SpriteRenderer spriteRenderer;
    private TankController tankController;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercentage => currentHealth / maxHealth;
    
    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (tankController == null) tankController = GetComponent<TankController>();
        
        if (isServer)
        {
            currentHealth = maxHealth;
        }
    }
    
    public void TakeDamage(float damage, Vector2 hitPoint, GameObject attacker)
    {
        if (!isServer || IsDead()) return;
        
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
        
        // Disable tank controller
        if (tankController != null)
        {
            tankController.enabled = false;
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
        
        // Destroy after delay
        Invoke(nameof(DestroyTank), 3f);
    }
    
    void DestroyTank()
    {
        if (isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
    
    public void Heal(float amount)
    {
        if (!isServer || IsDead()) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
    }
    
    public void Respawn()
    {
        if (!isServer) return;
        
        currentHealth = maxHealth;
        
        // Reset visual
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        // Re-enable tank controller
        if (tankController != null)
        {
            tankController.enabled = true;
        }
        
        // Re-enable Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
        }
    }
}