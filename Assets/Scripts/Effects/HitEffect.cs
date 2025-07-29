using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private float lifetime = 0.5f;
    [SerializeField] private Color hitColor = Color.red;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = hitColor;
        }
        
        Destroy(gameObject, lifetime);
    }
    
    private void OnDestroy()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
} 