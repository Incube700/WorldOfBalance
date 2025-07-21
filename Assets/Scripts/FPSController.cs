using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("FPS Settings")]
    public bool limitFPS = true;
    public int targetFPS = 60;
    public bool showFPS = false;
    public bool showFPSInGame = false;
    
    [Header("FPS Display Settings")]
    public Color fpsTextColor = Color.white;
    public int fontSize = 16;
    public Vector2 fpsDisplayPosition = new Vector2(10, 10);
    
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private float avgFPS = 0.0f;
    private int frameCount = 0;
    private float timeElapsed = 0.0f;
    
    void Awake()
    {
        // Настройка FPS
        if (limitFPS)
        {
            Application.targetFrameRate = targetFPS;
            QualitySettings.vSyncCount = 0; // Отключаем V-Sync для лучшего контроля
        }
        
        Debug.Log($"FPS контроллер инициализирован. Целевой FPS: {targetFPS}");
    }
    
    void Update()
    {
        UpdateFPS();
        
        // Проверяем нажатия клавиш для управления FPS
        CheckFPSControls();
    }
    
    void UpdateFPS()
    {
        // Обновляем текущий FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        
        // Обновляем средний FPS
        frameCount++;
        timeElapsed += Time.unscaledDeltaTime;
        
        if (timeElapsed >= 1.0f)
        {
            avgFPS = frameCount / timeElapsed;
            frameCount = 0;
            timeElapsed = 0.0f;
        }
        
        // Отладочная информация
        if (showFPS)
        {
            Debug.Log($"FPS: {fps:F1}, Средний FPS: {avgFPS:F1}");
        }
    }
    
    void CheckFPSControls()
    {
        // F1 - включить/выключить ограничение FPS
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleFPSLimit();
        }
        
        // F2 - включить/выключить отображение FPS
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleFPSDisplay();
        }
        
        // F3 - переключить отображение FPS в игре
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ToggleFPSInGameDisplay();
        }
        
        // F4 - установить FPS 30
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SetTargetFPS(30);
        }
        
        // F5 - установить FPS 60
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SetTargetFPS(60);
        }
        
        // F6 - установить FPS 120
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SetTargetFPS(120);
        }
    }
    
    void OnGUI()
    {
        if (showFPSInGame)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fontSize;
            style.normal.textColor = fpsTextColor;
            
            string fpsText = $"FPS: {fps:F1}\nСредний FPS: {avgFPS:F1}";
            
            GUI.Label(new Rect(fpsDisplayPosition.x, fpsDisplayPosition.y, 200, 50), fpsText, style);
        }
    }
    
    // Публичные методы для управления FPS
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
        Debug.Log($"Отображение FPS в консоли: {(showFPS ? "включено" : "отключено")}");
    }
    
    public void ToggleFPSInGameDisplay()
    {
        showFPSInGame = !showFPSInGame;
        Debug.Log($"Отображение FPS в игре: {(showFPSInGame ? "включено" : "отключено")}");
    }
    
    public float GetCurrentFPS()
    {
        return fps;
    }
    
    public float GetAverageFPS()
    {
        return avgFPS;
    }
    
    public void SetFPSDisplayColor(Color color)
    {
        fpsTextColor = color;
    }
    
    public void SetFPSDisplayPosition(Vector2 position)
    {
        fpsDisplayPosition = position;
    }
} 