using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Joystick Settings")]
    [SerializeField] private float joystickRange = 50f;
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    
    private Vector2 inputVector = Vector2.zero;
    private bool isPressed = false;
    
    public Vector2 Direction => inputVector;
    public bool IsPressed => isPressed;
    
    void Start()
    {
        // Auto-setup if components not assigned
        if (background == null) background = GetComponent<RectTransform>();
        if (handle == null)
        {
            Transform handleTransform = transform.Find("Handle");
            if (handleTransform != null)
                handle = handleTransform.GetComponent<RectTransform>();
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        OnDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out position))
        {
            // Normalize position relative to joystick range
            position.x = (position.x / background.sizeDelta.x);
            position.y = (position.y / background.sizeDelta.y);
            
            // Clamp to circular area
            inputVector = new Vector2(position.x * 2, position.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            
            // Move handle
            if (handle != null)
            {
                handle.anchoredPosition = new Vector2(
                    inputVector.x * (background.sizeDelta.x / 2),
                    inputVector.y * (background.sizeDelta.y / 2)
                );
            }
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        inputVector = Vector2.zero;
        
        // Reset handle position
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }
}