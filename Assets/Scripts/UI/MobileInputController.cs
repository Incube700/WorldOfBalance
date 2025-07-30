using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileInputController : MonoBehaviour
{
    [Header("Mobile UI Elements")]
    [SerializeField] private GameObject mobileUI;
    [SerializeField] private Joystick moveJoystick;
    [SerializeField] private Button fireButton;
    [SerializeField] private Button secondaryButton;
    
    [Header("Input References")]
    [SerializeField] private TankController tankController;
    
    private bool isMobilePlatform;
    
    void Start()
    {
        // Detect platform
        isMobilePlatform = Application.platform == RuntimePlatform.Android || 
                          Application.platform == RuntimePlatform.IPhonePlayer;
        
        // Show/hide mobile UI based on platform
        if (mobileUI != null)
        {
            mobileUI.SetActive(isMobilePlatform);
        }
        
        // Setup button listeners
        if (fireButton != null)
        {
            fireButton.onClick.AddListener(OnFireButtonPressed);
        }
        
        if (secondaryButton != null)
        {
            secondaryButton.onClick.AddListener(OnSecondaryButtonPressed);
        }
        
        Debug.Log($"Mobile UI initialized. Platform: {Application.platform}, Mobile: {isMobilePlatform}");
    }
    
    void OnFireButtonPressed()
    {
        if (tankController != null)
        {
            // Trigger fire action
            Debug.Log("Fire button pressed");
        }
    }
    
    void OnSecondaryButtonPressed()
    {
        if (tankController != null)
        {
            // Trigger secondary action (if any)
            Debug.Log("Secondary button pressed");
        }
    }
    
    public Vector2 GetMoveInput()
    {
        if (moveJoystick != null)
        {
            return moveJoystick.Direction;
        }
        return Vector2.zero;
    }
    
    public bool IsFireButtonPressed()
    {
        // This would be handled by the button's onClick event
        return false;
    }
    
    void OnDestroy()
    {
        // Clean up button listeners
        if (fireButton != null)
        {
            fireButton.onClick.RemoveListener(OnFireButtonPressed);
        }
        
        if (secondaryButton != null)
        {
            secondaryButton.onClick.RemoveListener(OnSecondaryButtonPressed);
        }
    }
} 