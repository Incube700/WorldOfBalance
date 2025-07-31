using UnityEngine;
using UnityEngine.UI;

public class MobileUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Joystick joystick;
    [SerializeField] private Button fireButton;
    
    [Header("Settings")]
    [SerializeField] private bool showOnMobileOnly = true;
    
    void Start()
    {
        // Only show on mobile platforms
        if (showOnMobileOnly && !Application.isMobilePlatform)
        {
            gameObject.SetActive(false);
            return;
        }
        
        SetupUI();
    }
    
    void SetupUI()
    {
        // Create Canvas if not assigned
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("MobileCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            // Add required components
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create Joystick if not assigned
        if (joystick == null)
        {
            CreateJoystick();
        }
        
        // Create Fire Button if not assigned
        if (fireButton == null)
        {
            CreateFireButton();
        }
        
        Debug.Log("Mobile UI setup complete");
    }
    
    void CreateJoystick()
    {
        // Create Joystick background
        GameObject joystickObj = new GameObject("Joystick");
        joystickObj.transform.SetParent(canvas.transform, false);
        
        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(joystickObj.transform, false);
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.5f);
        
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(0, 0);
        bgRect.anchoredPosition = new Vector2(150, 150);
        bgRect.sizeDelta = new Vector2(100, 100);
        
        // Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(joystickObj.transform, false);
        
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0, 0);
        handleRect.anchorMax = new Vector2(0, 0);
        handleRect.anchoredPosition = Vector2.zero;
        handleRect.sizeDelta = new Vector2(40, 40);
        
        // Add Joystick component
        joystick = joystickObj.AddComponent<Joystick>();
        joystick.background = bgRect;
        joystick.handle = handleRect;
    }
    
    void CreateFireButton()
    {
        // Create Fire Button
        GameObject buttonObj = new GameObject("FireButton");
        buttonObj.transform.SetParent(canvas.transform, false);
        
        fireButton = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(1, 0, 0, 0.8f);
        
        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = "FIRE";
        buttonText.color = Color.white;
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = 24;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Position button
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1, 0);
        buttonRect.anchorMax = new Vector2(1, 0);
        buttonRect.anchoredPosition = new Vector2(-150, 150);
        buttonRect.sizeDelta = new Vector2(120, 60);
        
        // Add click handler
        fireButton.onClick.AddListener(OnFireButtonClicked);
    }
    
    void OnFireButtonClicked()
    {
        // This will be handled by InputManager
        Debug.Log("Fire button clicked");
    }
} 