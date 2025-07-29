using UnityEngine;

public class RicochetEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private Color ricochetColor = Color.yellow;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = ricochetColor;
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