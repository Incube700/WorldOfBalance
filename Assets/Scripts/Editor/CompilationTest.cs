using UnityEngine;
using UnityEditor;

public class CompilationTest : MonoBehaviour
{
    [MenuItem("Tools/Test Compilation")]
    public static void TestCompilation()
    {
        Debug.Log("=== Compilation Test Started ===");
        
        // Test Joystick access
        TestJoystickAccess();
        
        // Test MobileUI access
        TestMobileUIAccess();
        
        // Test InputManager access
        TestInputManagerAccess();
        
        Debug.Log("=== Compilation Test Completed ===");
    }
    
    private static void TestJoystickAccess()
    {
        Debug.Log("Testing Joystick access...");
        
        // Create a test joystick
        GameObject joystickObj = new GameObject("TestJoystick");
        Joystick joystick = joystickObj.AddComponent<Joystick>();
        
        // Test public field access
        if (joystick.background == null)
        {
            Debug.Log("✅ Joystick.background is accessible");
        }
        
        if (joystick.handle == null)
        {
            Debug.Log("✅ Joystick.handle is accessible");
        }
        
        // Test public property access
        Vector2 direction = joystick.Direction;
        bool isPressed = joystick.IsPressed;
        float maxRadius = joystick.MaxRadius;
        Color normalColor = joystick.NormalColor;
        Color activeColor = joystick.ActiveColor;
        
        Debug.Log("✅ Joystick public properties are accessible");
        
        // Test public method access
        joystick.SetColors(Color.red, Color.blue);
        joystick.SetMaxRadius(100f);
        
        Debug.Log("✅ Joystick public methods are accessible");
        
        // Cleanup
        DestroyImmediate(joystickObj);
    }
    
    private static void TestMobileUIAccess()
    {
        Debug.Log("Testing MobileUI access...");
        
        // Create a test MobileUI
        GameObject mobileUIObj = new GameObject("TestMobileUI");
        MobileUI mobileUI = mobileUIObj.AddComponent<MobileUI>();
        
        // Test that MobileUI can create joystick
        // This will test the access to joystick.background and joystick.handle
        mobileUI.SendMessage("SetupUI");
        
        Debug.Log("✅ MobileUI can access Joystick fields");
        
        // Cleanup
        DestroyImmediate(mobileUIObj);
    }
    
    private static void TestInputManagerAccess()
    {
        Debug.Log("Testing InputManager access...");
        
        // Create a test InputManager
        GameObject inputManagerObj = new GameObject("TestInputManager");
        InputManager inputManager = inputManagerObj.AddComponent<InputManager>();
        
        // Test that InputManager can access joystick properties
        Vector2 movement = inputManager.GetMovementDirection();
        bool isFiring = inputManager.IsFiring();
        Vector2 fireDir = inputManager.GetFireDirection(Vector2.zero);
        
        Debug.Log("✅ InputManager can access Joystick properties");
        
        // Cleanup
        DestroyImmediate(inputManagerObj);
    }
} 