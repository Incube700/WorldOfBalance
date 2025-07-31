using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PrefabCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Player Prefab")]
    public static void CreatePlayerPrefab()
    {
        Debug.Log("=== СОЗДАЕМ ПРЕФАБ ИГРОКА ===");
        
        // Создаем игрока
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        
        // Визуал
        SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(0, 0.5f, 1f, 1f); // Синий
        spriteRenderer.sortingOrder = 1;
        
        // Физика
        BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        
        Rigidbody2D rigidbody = player.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.linearDamping = 0.5f;
        rigidbody.angularDamping = 0.05f;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Компоненты
        player.AddComponent<PlayerController>();
        player.AddComponent<HealthSystem>();
        player.AddComponent<Weapon>();
        
        // Создаем башню игрока
        GameObject turret = new GameObject("PlayerTurret");
        turret.transform.SetParent(player.transform);
        turret.transform.localPosition = Vector3.zero;
        
        SpriteRenderer turretSprite = turret.AddComponent<SpriteRenderer>();
        turretSprite.sprite = CreateSquareSprite();
        turretSprite.color = new Color(0.2f, 0.4f, 0.8f, 1f); // Темно-синий
        turretSprite.sortingOrder = 2;
        turretSprite.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        
        // Создаем точку стрельбы
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform);
        firePoint.transform.localPosition = new Vector3(1f, 0, 0);
        
        // Назначаем firePoint в Weapon
        Weapon weapon = player.GetComponent<Weapon>();
        if (weapon != null)
        {
            var firePointField = typeof(Weapon).GetField("firePoint", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (firePointField != null)
            {
                firePointField.SetValue(weapon, firePoint.transform);
            }
        }
        
        // Сохраняем как префаб
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        string prefabPath = "Assets/Prefabs/Player.prefab";
        PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
        
        // Удаляем временный объект из сцены
        DestroyImmediate(player);
        
        AssetDatabase.Refresh();
        Debug.Log($"Префаб Player создан: {prefabPath}");
    }
    
    [MenuItem("Tools/Create Simple MobileUI")]
    public static void CreateSimpleMobileUI()
    {
        Debug.Log("=== СОЗДАЕМ ПРОСТОЙ МОБИЛЬНЫЙ UI ===");
        
        // Создаем простой пустой объект для мобильного UI
        GameObject mobileUI = new GameObject("MobileUI");
        
        // Добавляем простой компонент-маркер
        mobileUI.AddComponent<Transform>();
        
        // Сохраняем как префаб
        string prefabPath = "Assets/Prefabs/MobileUI.prefab";
        PrefabUtility.SaveAsPrefabAsset(mobileUI, prefabPath);
        
        // Удаляем временный объект из сцены
        DestroyImmediate(mobileUI);
        
        AssetDatabase.Refresh();
        Debug.Log($"Простой префаб MobileUI создан: {prefabPath}");
        Debug.Log("Для полноценного UI создайте Canvas вручную в сцене");
    }
    
    static Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }
}