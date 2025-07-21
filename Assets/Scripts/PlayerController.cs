using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions; // Перетащить сюда Input Action Asset
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // единиц в секунду
    public float acceleration = 13f; // ускорение в единицах в секунду²
    public float deceleration = 15f; // замедление в единицах в секунду²
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    
    [Header("Performance Settings")]
    public bool limitFPS = true;
    public int targetFPS = 60;
    public bool showFPS = false;
    
    private InputAction moveAction;
    private InputAction dashAction;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;
    
    // FPS контроллер
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    
    void Awake()
    {
        // Настройка FPS
        if (limitFPS)
        {
            Application.targetFrameRate = targetFPS;
            QualitySettings.vSyncCount = 0; // Отключаем V-Sync для лучшего контроля
        }
        
        // Получаем Actions из Asset
        moveAction = inputActions.FindAction("Player/Move");
        
        // Создаем действие dash программно, так как его нет в Asset
        dashAction = new InputAction("dash", InputActionType.Button);
        dashAction.AddBinding("<Keyboard>/space");
        
        // Отладочная информация
        if (moveAction == null)
        {
            Debug.LogError("Move action not found! Check Input Action Asset.");
        }
        else
        {
            Debug.Log("Move action found successfully!");
        }
        
        Debug.Log("Dash action created successfully!");
        Debug.Log($"FPS ограничен до: {targetFPS}");
    }
    
    void OnEnable()
    {
        moveAction?.Enable();
        dashAction?.Enable();
    }
    
    void OnDisable()
    {
        moveAction?.Disable();
        dashAction?.Disable();
    }
    
    void Update()
    {
        // Обновление FPS
        UpdateFPS();
        
        HandleMovement();
        HandleDash();
    }
    
    void UpdateFPS()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        
        if (showFPS)
        {
            Debug.Log($"FPS: {fps:F1}");
        }
    }
    
    void HandleMovement()
    {
        if (!isDashing)
        {
            moveInput = moveAction.ReadValue<Vector2>();
            
            // Плавное ускорение и замедление
            Vector2 targetVelocity = moveInput * moveSpeed;
            
            if (moveInput.magnitude > 0.1f)
            {
                // Ускорение
                currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            }
            else
            {
                // Замедление
                currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
            }
            
            Vector3 movement = new Vector3(currentVelocity.x, currentVelocity.y, 0f) * Time.deltaTime;
            transform.Translate(movement);
            
            // Отладочная информация
            if (moveInput.magnitude > 0.1f)
            {
                Debug.Log($"Moving: {moveInput}, Speed: {currentVelocity.magnitude:F1} u/s");
            }
        }
    }
    
    void HandleDash()
    {
        // Обновляем таймеры
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
            
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                Debug.Log("Dash ended");
            }
            else
            {
                // Выполняем даш
                Vector3 dashMovement = new Vector3(dashDirection.x, dashDirection.y, 0f) * dashSpeed * Time.deltaTime;
                transform.Translate(dashMovement);
            }
        }
        else
        {
            // Проверяем нажатие даша
            if (dashAction.WasPressedThisFrame() && dashCooldownTimer <= 0 && moveInput.magnitude > 0.1f)
            {
                StartDash();
            }
        }
    }
    
    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = moveInput.normalized;
        Debug.Log($"Dash started! Direction: {dashDirection}, Speed: {dashSpeed}");
    }
    
    // Методы для изменения настроек FPS во время игры
    public void SetTargetFPS(int newTargetFPS)
    {
        targetFPS = newTargetFPS;
        if (limitFPS)
        {
            Application.targetFrameRate = targetFPS;
            Debug.Log($"FPS изменен на: {targetFPS}");
        }
    }
    
    public void ToggleFPSLimit()
    {
        limitFPS = !limitFPS;
        if (limitFPS)
        {
            Application.targetFrameRate = targetFPS;
            Debug.Log("Ограничение FPS включено");
        }
        else
        {
            Application.targetFrameRate = -1; // Без ограничений
            Debug.Log("Ограничение FPS отключено");
        }
    }
    
    public void ToggleFPSDisplay()
    {
        showFPS = !showFPS;
        Debug.Log($"Отображение FPS: {(showFPS ? "включено" : "отключено")}");
    }
    
    // Получить текущий FPS
    public float GetCurrentFPS()
    {
        return fps;
    }
}
