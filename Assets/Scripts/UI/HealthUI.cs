using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;
    [SerializeField] private HealthSystem healthSystem;
    
    void Start()
    {
        if (healthSystem == null)
            healthSystem = GetComponent<HealthSystem>();
            
        if (healthSlider != null)
        {
            healthSlider.maxValue = healthSystem.MaxHealth;
            healthSlider.value = healthSystem.CurrentHealth;
        }
    }
    
    void Update()
    {
        if (healthSystem == null) return;
        
        // Update slider
        if (healthSlider != null)
        {
            healthSlider.value = healthSystem.CurrentHealth;
        }
        
        // Update text
        if (healthText != null)
        {
            healthText.text = $"HP: {healthSystem.CurrentHealth:F0}/{healthSystem.MaxHealth:F0}";
        }
    }
} 