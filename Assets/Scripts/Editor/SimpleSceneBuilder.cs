using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SimpleSceneBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build Simple Local Scene")]
    public static void BuildSimpleLocalScene()
    {
        Debug.Log("=== СОЗДАЕМ ПРОСТУЮ СЦЕНУ ===");
        
        // Очищаем сцену
        ClearScene();
        
        // Создаем объекты
        CreateCamera();
        CreateGround();
        CreatePlayer();
        CreateEnemy();
        CreateProjectile();
        CreateGameManager();
        CreateUI();
        
        Debug.Log("=== СЦЕНА ГОТОВА! ===");
        Debug.Log("Нажмите Play для тестирования!");
    }
    
    static void ClearScene()
    {
        Debug.Log("Очищаем сцену...");
        
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name != "Main Camera")
            {
                DestroyImmediate(obj);
            }
        }
    }
    
    static void CreateCamera()
    {
        var camera = Camera.main;
        if (camera == null)
        {
            var cameraObj = new GameObject("Main Camera");
            camera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
        }
        
        camera.orthographic = true;
        camera.orthographicSize = 10f;
        camera.backgroundColor = new Color(0.2f, 0.3f, 0.5f, 1f);
        camera.transform.position = new Vector3(0, 0, -10);
    }
    
    static void CreateGround()
    {
        var ground = new GameObject("Ground");
        ground.transform.position = new Vector3(0, -5, 0);
        ground.transform.localScale = new Vector3(20, 1, 1);
        
        var spriteRenderer = ground.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(0.3f, 0.6f, 0.3f, 1f);
        
        var collider = ground.AddComponent<BoxCollider2D>();
        var rigidbody = ground.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;
    }
    
    static void CreatePlayer()
    {
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(-5, 0, 0);
        
        var spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(0, 0.5f, 1f, 1f);
        
        var collider = player.AddComponent<BoxCollider2D>();
        var rigidbody = player.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.drag = 0.5f;
        
        player.AddComponent<TankController>();
        player.AddComponent<HealthSystem>();
        player.AddComponent<ArmorSystem>();
        player.AddComponent<ProjectileSpawner>();
    }
    
    static void CreateEnemy()
    {
        var enemy = new GameObject("Enemy");
        enemy.transform.position = new Vector3(5, 0, 0);
        
        var spriteRenderer = enemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 1f);
        
        var collider = enemy.AddComponent<BoxCollider2D>();
        var rigidbody = enemy.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.drag = 0.5f;
        
        enemy.AddComponent<EnemyAIController>();
        enemy.AddComponent<HealthSystem>();
        enemy.AddComponent<ArmorSystem>();
    }
    
    static void CreateProjectile()
    {
        var projectile = new GameObject("Projectile");
        projectile.transform.position = new Vector3(0, 10, 0);
        
        var spriteRenderer = projectile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.color = new Color(1f, 0.5f, 0f, 1f);
        spriteRenderer.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        
        var collider = projectile.AddComponent<CircleCollider2D>();
        var rigidbody = projectile.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        
        projectile.AddComponent<Projectile>();
        projectile.SetActive(false);
    }
    
    static void CreateGameManager()
    {
        var gameManager = new GameObject("LocalGameManager");
        gameManager.AddComponent<LocalGameManager>();
    }
    
    static void CreateUI()
    {
        var canvas = new GameObject("Canvas");
        var canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        var panel = new GameObject("MenuPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);
        
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        var mainMenu = panel.AddComponent<MainMenu>();
        
        CreateButton(panel, "StartButton", "СТАРТ ИГРЫ", new Vector2(0, 50), () => {
            mainMenu.StartGame();
        });
        
        CreateButton(panel, "QuitButton", "ВЫХОД", new Vector2(0, -50), () => {
            mainMenu.QuitGame();
        });
    }
    
    static void CreateButton(GameObject parent, string name, string text, Vector2 position, System.Action onClick)
    {
        var button = new GameObject(name);
        button.transform.SetParent(parent.transform, false);
        
        var buttonImage = button.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        var buttonComponent = button.AddComponent<Button>();
        buttonComponent.onClick.AddListener(() => onClick?.Invoke());
        
        var buttonRect = button.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(250, 60);
        buttonRect.anchoredPosition = position;
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        var buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 28;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
    
    static Sprite CreateSquareSprite()
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }
    
    static Sprite CreateCircleSprite()
    {
        var texture = new Texture2D(32, 32);
        var center = new Vector2(16, 16);
        var radius = 16f;
        
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                var distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
} 