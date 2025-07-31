using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private bool useMobileInput = false;
    [SerializeField] private GameObject joystick;
    
    private Vector2 movementInput;
    private bool fireInput;
    private bool isMobilePlatform;
    
    public Vector2 MovementInput => movementInput;
    public bool FireInput => fireInput;
    public bool IsMobilePlatform => isMobilePlatform;
    
    void Start()
    {
        // Detect platform
        isMobilePlatform = Application.isMobilePlatform;
        
        // Auto-detect mobile input if on mobile platform
        if (isMobilePlatform)
        {
            useMobileInput = true;
        }
        
        // Find joystick if not assigned
        if (joystick == null)
        {
            joystick = GameObject.Find("Joystick");
        }
        
        Debug.Log($"InputManager initialized - Mobile: {isMobilePlatform}, UseMobileInput: {useMobileInput}");
    }
    
    void Update()
    {
        if (useMobileInput)
        {
            HandleMobileInput();
        }
        else
        {
            HandleKeyboardInput();
        }
    }
    
    void HandleKeyboardInput()
    {
        // Movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movementInput = new Vector2(horizontal, vertical).normalized;
        
        // Fire input
        fireInput = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
    }
    
    void HandleMobileInput()
    {
        // Simplified mobile input - fallback to keyboard
        HandleKeyboardInput();
    }
    
    public Vector2 GetMovementDirection()
    {
        return movementInput;
    }
    
    public bool IsFiring()
    {
        return fireInput;
    }
    
    public Vector2 GetMouseWorldPosition()
    {
        if (Camera.main != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            return mouseWorldPos;
        }
        return Vector2.zero;
    }
    
    public Vector2 GetFireDirection(Vector2 fromPosition)
    {
        // Use mouse direction for keyboard
        Vector2 mouseWorldPos = GetMouseWorldPosition();
        return (mouseWorldPos - fromPosition).normalized;
    }
} 