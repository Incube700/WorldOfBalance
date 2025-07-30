using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SceneCreator : MonoBehaviour
{
    [MenuItem("Tools/Create New Local Scene")]
    public static void CreateNewLocalScene()
    {
        Debug.Log("=== СОЗДАЕМ НОВУЮ СЦЕНУ ===");
        
        // Очищаем сцену
        ClearScene();
        
        // Создаем основные объекты
        CreateMainCamera();
        CreateGround();
        CreatePlayer();
        CreateEnemy();
        CreateProjectile();
        CreateGameManager();
        CreateUI();
        
        Debug.Log("=== НОВАЯ СЦЕНА СОЗДАНА ===");
        Debug.Log("Нажмите Play для тестирования!");
    }
    
    static void ClearScene()
    {
        Debug.Log("Очищаем сцену...");
        
        // Удаляем все объекты кроме камеры
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name != "Main Camera")
            {
                DestroyImmediate(obj);
            }
        }
    }
    
    static void CreateMainCamera()
    {
        Debug.Log("Создаем камеру...");
        
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
        
        Debug.Log("Камера настроена");
    }
    
    static void CreateGround()
    {
        Debug.Log("Создаем землю...");
        
        var ground = new GameObject("Ground");
        ground.transform.position = new Vector3(0, -5, 0);
        ground.transform.localScale = new Vector3(20, 1, 1);
        
        var spriteRenderer = ground.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(0.3f, 0.6f, 0.3f, 1f);
        
        var collider = ground.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        
        var rigidbody = ground.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;
        
        Debug.Log("Земля создана");
    }
    
    static void CreatePlayer()
    {
        Debug.Log("Создаем игрока...");
        
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(-5, 0, 0);
        
        // Визуал
        var spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(0, 0.5f, 1f, 1f);
        
        // Физика
        var collider = player.AddComponent<BoxCollider2D>();
        var rigidbody = player.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.drag = 0.5f;
        rigidbody.angularDrag = 0.05f;
        
        // Компоненты
        player.AddComponent<TankController>();
        player.AddComponent<HealthSystem>();
        player.AddComponent<ArmorSystem>();
        player.AddComponent<ProjectileSpawner>();
        
        Debug.Log("Игрок создан");
    }
    
    static void CreateEnemy()
    {
        Debug.Log("Создаем врага...");
        
        var enemy = new GameObject("Enemy");
        enemy.transform.position = new Vector3(5, 0, 0);
        
        // Визуал
        var spriteRenderer = enemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 1f);
        
        // Физика
        var collider = enemy.AddComponent<BoxCollider2D>();
        var rigidbody = enemy.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.drag = 0.5f;
        rigidbody.angularDrag = 0.05f;
        
        // Компоненты
        enemy.AddComponent<EnemyAIController>();
        enemy.AddComponent<HealthSystem>();
        enemy.AddComponent<ArmorSystem>();
        
        // Создаем башню врага
        var turret = new GameObject("EnemyTurret");
        turret.transform.SetParent(enemy.transform);
        turret.transform.localPosition = Vector3.zero;
        
        var turretSprite = turret.AddComponent<SpriteRenderer>();
        turretSprite.sprite = CreateSquareSprite();
        turretSprite.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        turretSprite.sortingOrder = 1;
        
        turret.AddComponent<EnemyTurretController>();
        
        Debug.Log("Враг создан");
    }
    
    static void CreateProjectile()
    {
        Debug.Log("Создаем снаряд...");
        
        var projectile = new GameObject("Projectile");
        projectile.transform.position = new Vector3(0, 10, 0); // Вне экрана
        
        // Визуал
        var spriteRenderer = projectile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.color = new Color(1f, 0.5f, 0f, 1f);
        spriteRenderer.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        
        // Физика
        var collider = projectile.AddComponent<CircleCollider2D>();
        var rigidbody = projectile.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.drag = 0f;
        rigidbody.angularDrag = 0.05f;
        
        // Компонент
        projectile.AddComponent<Projectile>();
        
        // Скрываем снаряд
        projectile.SetActive(false);
        
        Debug.Log("Снаряд создан");
    }
    
    static void CreateGameManager()
    {
        Debug.Log("Создаем GameManager...");
        
        var gameManager = new GameObject("LocalGameManager");
        gameManager.AddComponent<LocalGameManager>();
        
        Debug.Log("GameManager создан");
    }
    
    static void CreateUI()
    {
        Debug.Log("Создаем UI...");
        
        // Canvas
        var canvas = new GameObject("Canvas");
        var canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        // Panel
        var panel = new GameObject("MenuPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);
        
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // MainMenu скрипт
        var mainMenu = panel.AddComponent<MainMenu>();
        
        // Кнопки
        CreateButton(panel, "StartButton", "СТАРТ ИГРЫ", new Vector2(0, 50), () => {
            Debug.Log("Игра запущена!");
            mainMenu.StartGame();
        });
        
        CreateButton(panel, "QuitButton", "ВЫХОД", new Vector2(0, -50), () => {
            Debug.Log("Выход из игры!");
            mainMenu.QuitGame();
        });
        
        Debug.Log("UI создан");
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
        
        // Текст
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        var buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
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