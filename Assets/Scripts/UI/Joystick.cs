using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Joystick Settings")]
    [SerializeField] public RectTransform background;
    [SerializeField] public RectTransform handle;
    [SerializeField] private float maxRadius = 50f;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = Color.yellow;
    
    private Vector2 input;
    private bool isDragging;
    private Image backgroundImage;
    private Image handleImage;
    
    public Vector2 Direction => input;
    public bool IsPressed => isDragging;
    
    // Public properties for external access
    public float MaxRadius => maxRadius;
    public Color NormalColor => normalColor;
    public Color ActiveColor => activeColor;
    
    void Start()
    {
        if (backgroundImage == null) backgroundImage = background?.GetComponent<Image>();
        if (handleImage == null) handleImage = handle?.GetComponent<Image>();
        
        // Set initial colors
        if (backgroundImage != null) backgroundImage.color = normalColor;
        if (handleImage != null) handleImage.color = normalColor;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        OnDrag(eventData);
        
        // Change colors
        if (backgroundImage != null) backgroundImage.color = activeColor;
        if (handleImage != null) handleImage.color = activeColor;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        
        if (success)
        {
            // Calculate input
            Vector2 clampedInput = Vector2.ClampMagnitude(localPoint, maxRadius);
            input = clampedInput / maxRadius;
            
            // Move handle
            if (handle != null)
            {
                handle.anchoredPosition = input * maxRadius;
            }
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        input = Vector2.zero;
        
        // Reset handle position
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
        
        // Reset colors
        if (backgroundImage != null) backgroundImage.color = normalColor;
        if (handleImage != null) handleImage.color = normalColor;
    }
    
    public void ResetJoystick()
    {
        isDragging = false;
        input = Vector2.zero;
        
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
        
        if (backgroundImage != null) backgroundImage.color = normalColor;
        if (handleImage != null) handleImage.color = normalColor;
    }
    
    // Public methods for external control
    public void SetColors(Color normal, Color active)
    {
        normalColor = normal;
        activeColor = active;
        
        if (backgroundImage != null) backgroundImage.color = normalColor;
        if (handleImage != null) handleImage.color = normalColor;
    }
    
    public void SetMaxRadius(float radius)
    {
        maxRadius = radius;
    }
    
    public void SetBackground(RectTransform bg)
    {
        background = bg;
        if (backgroundImage == null) backgroundImage = background?.GetComponent<Image>();
    }
    
    public void SetHandle(RectTransform hdl)
    {
        handle = hdl;
        if (handleImage == null) handleImage = handle?.GetComponent<Image>();
    }
} 