using UnityEngine;

/// <summary>
/// Система урона и здоровья игрока
/// </summary>
public class DamageSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Damage Settings")]
    public float damageMultiplier = 1f;
    
    private ArmorSystem armorSystem;
    private HitEffect hitEffect;
    
    void Start()
    {
        currentHealth = maxHealth;
        armorSystem = GetComponent<ArmorSystem>();
        hitEffect = GetComponent<HitEffect>();
        
        if (armorSystem == null)
        {
            Debug.LogWarning($"ArmorSystem not found on {gameObject.name}");
        }
        
        if (hitEffect == null)
        {
            Debug.LogWarning($"HitEffect not found on {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Обрабатывает попадание снаряда в игрока
    /// </summary>
    /// <param name="damage">Базовый урон снаряда</param>
    /// <param name="penetrationPower">Сила пробития снаряда</param>
    /// <param name="hitDirection">Направление удара</param>
    /// <returns>true если снаряд пробил броню и нанес урон</returns>
    public bool TakeDamage(float damage, float penetrationPower, Vector2 hitDirection)
    {
        if (armorSystem == null)
        {
            // Если нет системы брони, наносим полный урон
            ApplyDamage(damage);
            return true;
        }
        
        // Получаем направление взгляда игрока
        Vector2 playerForward = transform.right; // Игрок смотрит вправо по умолчанию
        
        // Вычисляем эффективную броню
        float effectiveArmor = armorSystem.GetEffectiveArmor(hitDirection, playerForward);
        
        // Проверяем, пробивает ли снаряд броню
        bool canPenetrate = armorSystem.CanPenetrate(penetrationPower, effectiveArmor);
        
        if (canPenetrate)
        {
            // Снаряд пробил броню - наносим урон
            float finalDamage = damage * damageMultiplier;
            ApplyDamage(finalDamage);
            
            Debug.Log($"{gameObject.name} took {finalDamage} damage! HP: {currentHealth}");
            
            // Показываем эффект попадания
            if (hitEffect != null)
            {
                hitEffect.ShowHitEffect();
            }
            
            return true;
        }
        else
        {
            // Снаряд не пробил броню - отскакивает
            Debug.Log($"{gameObject.name} blocked the shot! Armor: {effectiveArmor}");
            
            // Показываем эффект отражения
            if (hitEffect != null)
            {
                hitEffect.ShowDeflectEffect();
            }
            
            return false;
        }
    }
    
    /// <summary>
    /// Применяет урон к игроку
    /// </summary>
    /// <param name="damage">Количество урона</param>
    private void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        // Проверяем смерть игрока
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Обрабатывает смерть игрока
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        
        // Уведомляем GameManager о смерти игрока
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.OnPlayerDeath(gameObject);
        }
        
        // Скрываем игрока
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Восстанавливает здоровье игрока
    /// </summary>
    /// <param name="amount">Количество здоровья для восстановления</param>
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        Debug.Log($"{gameObject.name} healed {amount} HP. Current HP: {currentHealth}");
    }
    
    /// <summary>
    /// Возвращает процент здоровья (0-1)
    /// </summary>
    /// <returns>Процент здоровья</returns>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    /// <summary>
    /// Возвращает true если игрок мертв
    /// </summary>
    /// <returns>true если игрок мертв</returns>
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    
    /// <summary>
    /// Возвращает true если игрок жив
    /// </summary>
    /// <returns>true если игрок жив</returns>
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
} 