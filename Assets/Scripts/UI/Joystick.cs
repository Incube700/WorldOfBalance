using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Joystick Settings")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
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
        
        Vector2 position = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        
        // Calculate input
        input = Vector2.ClampMagnitude(localPoint, maxRadius) / maxRadius;
        
        // Move handle
        if (handle != null)
        {
            handle.anchoredPosition = input * maxRadius;
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
} 