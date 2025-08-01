using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Health Bar References")]
    public Image playerHealthFill;
    public Image enemyHealthFill;
    public Text playerHealthText;
    public Text enemyHealthText;
    
    [Header("Auto-Find References")]
    public bool autoFindReferences = true;
    
    private HealthSystem playerHealth;
    private HealthSystem enemyHealth;
    
    void Start()
    {
        if (autoFindReferences)
        {
            FindHealthBars();
            FindHealthSystems();
        }
    }
    
    void FindHealthBars()
    {
        // Find player health bar
        GameObject playerHealthBar = GameObject.Find("PlayerHealthBar");
        if (playerHealthBar != null)
        {
            playerHealthFill = playerHealthBar.transform.Find("Fill")?.GetComponent<Image>();
            playerHealthText = playerHealthBar.transform.Find("Label")?.GetComponent<Text>();
        }
        
        // Find enemy health bar
        GameObject enemyHealthBar = GameObject.Find("EnemyHealthBar");
        if (enemyHealthBar != null)
        {
            enemyHealthFill = enemyHealthBar.transform.Find("Fill")?.GetComponent<Image>();
            enemyHealthText = enemyHealthBar.transform.Find("Label")?.GetComponent<Text>();
        }
        
        Debug.Log($"HealthUI: Found player fill: {playerHealthFill != null}, enemy fill: {enemyHealthFill != null}");
    }
    
    void FindHealthSystems()
    {
        // Find player and enemy health systems
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthSystem>();
        }
        
        GameObject enemy = GameObject.Find("Enemy");
        if (enemy != null)
        {
            enemyHealth = enemy.GetComponent<HealthSystem>();
        }
        
        Debug.Log($"HealthUI: Found player health: {playerHealth != null}, enemy health: {enemyHealth != null}");
    }
    
    void Update()
    {
        UpdateHealthBars();
    }
    
    void UpdateHealthBars()
    {
        // Update player health bar
        if (playerHealth != null && playerHealthFill != null)
        {
            float healthPercent = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
            playerHealthFill.fillAmount = healthPercent;
            
            if (playerHealthText != null)
            {
                playerHealthText.text = $"Player HP: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
            }
        }
        
        // Update enemy health bar
        if (enemyHealth != null && enemyHealthFill != null)
        {
            float healthPercent = (float)enemyHealth.CurrentHealth / enemyHealth.MaxHealth;
            enemyHealthFill.fillAmount = healthPercent;
            
            if (enemyHealthText != null)
            {
                enemyHealthText.text = $"Enemy HP: {enemyHealth.CurrentHealth}/{enemyHealth.MaxHealth}";
            }
        }
    }
    
    // Public method to manually refresh references if needed
    public void RefreshReferences()
    {
        FindHealthBars();
        FindHealthSystems();
    }
}