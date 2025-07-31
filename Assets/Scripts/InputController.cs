using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private bool useMobileInput = false;
    [SerializeField] private MobileJoystick mobileJoystick;
    
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
        
        // Find mobile joystick if not assigned
        if (mobileJoystick == null)
        {
            mobileJoystick = FindObjectOfType<MobileJoystick>();
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
        if (mobileJoystick != null)
        {
            // Get movement from mobile joystick
            movementInput = mobileJoystick.Direction;
            
            // Fire input from joystick press or screen tap
            fireInput = mobileJoystick.IsPressed || Input.touchCount > 0;
        }
        else
        {
            // Fallback to keyboard if no joystick found
            HandleKeyboardInput();
        }
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