using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [Header("Health System")]
    [SerializeField] private HealthSystem healthSystem;
    
    void Start()
    {
        if (healthSystem == null)
            healthSystem = GetComponent<HealthSystem>();
    }
    
    void Update()
    {
        if (healthSystem == null) return;
        
        // Просто логируем здоровье в консоль
        if (healthSystem.IsDead())
        {
            Debug.Log($"{gameObject.name} мертв!");
        }
    }
} 