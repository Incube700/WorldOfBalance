using UnityEngine;
using UnityEditor;

public class SceneDecorator : MonoBehaviour
{
    [MenuItem("Tools/Add Decorations to Scene")]
    public static void AddDecorations()
    {
        Debug.Log("=== ДОБАВЛЯЕМ ДЕКОРАЦИИ ===");
        
        CreateObstacles();
        CreatePowerUps();
        CreateBackground();
        
        Debug.Log("=== ДЕКОРАЦИИ ДОБАВЛЕНЫ ===");
    }
    
    static void CreateObstacles()
    {
        Debug.Log("Создаем препятствия...");
        
        // Создаем несколько препятствий
        CreateObstacle(new Vector3(-3, 2, 0), new Vector3(2, 1, 1), "Obstacle1");
        CreateObstacle(new Vector3(3, -2, 0), new Vector3(1, 2, 1), "Obstacle2");
        CreateObstacle(new Vector3(0, 3, 0), new Vector3(3, 1, 1), "Obstacle3");
        CreateObstacle(new Vector3(-2, -3, 0), new Vector3(1, 1, 1), "Obstacle4");
        
        Debug.Log("Препятствия созданы");
    }
    
    static void CreateObstacle(Vector3 position, Vector3 scale, string name)
    {
        var obstacle = new GameObject(name);
        obstacle.transform.position = position;
        obstacle.transform.localScale = scale;
        
        var spriteRenderer = obstacle.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(0.6f, 0.4f, 0.2f, 1f);
        spriteRenderer.sortingOrder = 0;
        
        var collider = obstacle.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        
        var rigidbody = obstacle.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;
        rigidbody.gravityScale = 0f;
    }
    
    static void CreatePowerUps()
    {
        Debug.Log("Создаем бонусы...");
        
        // Создаем бонусы здоровья
        CreatePowerUp(new Vector3(-7, 7, 0), "HealthPowerUp", Color.green);
        CreatePowerUp(new Vector3(7, -7, 0), "HealthPowerUp2", Color.green);
        
        // Создаем бонусы брони
        CreatePowerUp(new Vector3(7, 7, 0), "ArmorPowerUp", Color.blue);
        CreatePowerUp(new Vector3(-7, -7, 0), "ArmorPowerUp2", Color.blue);
        
        Debug.Log("Бонусы созданы");
    }
    
    static void CreatePowerUp(Vector3 position, string name, Color color)
    {
        var powerUp = new GameObject(name);
        powerUp.transform.position = position;
        powerUp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        
        var spriteRenderer = powerUp.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = 2;
        
        var collider = powerUp.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;
        
        var rigidbody = powerUp.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.bodyType = RigidbodyType2D.Static;
        
        // Добавляем компонент для вращения
        powerUp.AddComponent<PowerUpRotator>();
    }
    
    static void CreateBackground()
    {
        Debug.Log("Создаем фон...");
        
        // Создаем фоновые элементы
        for (int i = 0; i < 10; i++)
        {
            var x = Random.Range(-8f, 8f);
            var y = Random.Range(-8f, 8f);
            
            // Проверяем, что не создаем на танках
            if (Mathf.Abs(x + 5) > 2 && Mathf.Abs(x - 5) > 2)
            {
                CreateBackgroundElement(new Vector3(x, y, 0));
            }
        }
        
        Debug.Log("Фон создан");
    }
    
    static void CreateBackgroundElement(Vector3 position)
    {
        var element = new GameObject("BackgroundElement");
        element.transform.position = position;
        element.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        
        var spriteRenderer = element.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        spriteRenderer.sortingOrder = -2;
        
        // Добавляем легкое вращение
        element.AddComponent<BackgroundRotator>();
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

// Компонент для вращения бонусов
public class PowerUpRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;
    
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}

// Компонент для вращения фоновых элементов
public class BackgroundRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;
    
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
} 