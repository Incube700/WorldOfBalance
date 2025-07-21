using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSetupHelper : MonoBehaviour
{
    [Header("Tilemap References")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap decorationTilemap;
    
    [Header("Tile Assets")]
    public RuleTile floorTile;
    public RuleTile wallTile;
    public RuleTile decorationTile;
    
    [Header("Grid Settings")]
    public Vector3Int gridSize = new Vector3Int(20, 20, 1);
    public Vector3 cellSize = Vector3.one;
    
    void Start()
    {
        SetupTilemaps();
        CreateTestLevel();
    }
    
    void SetupTilemaps()
    {
        // Настройка слоев для правильного отображения
        if (floorTilemap != null)
        {
            var floorRenderer = floorTilemap.GetComponent<TilemapRenderer>();
            if (floorRenderer != null)
            {
                floorRenderer.sortingOrder = -1;
            }
            floorTilemap.gameObject.name = "Floor Tilemap";
        }
        
        if (wallTilemap != null)
        {
            var wallRenderer = wallTilemap.GetComponent<TilemapRenderer>();
            if (wallRenderer != null)
            {
                wallRenderer.sortingOrder = 0;
            }
            wallTilemap.gameObject.name = "Wall Tilemap";
        }
        
        if (decorationTilemap != null)
        {
            var decorationRenderer = decorationTilemap.GetComponent<TilemapRenderer>();
            if (decorationRenderer != null)
            {
                decorationRenderer.sortingOrder = 1;
            }
            decorationTilemap.gameObject.name = "Decoration Tilemap";
        }
        
        Debug.Log("Tilemaps настроены!");
    }
    
    void CreateTestLevel()
    {
        if (floorTilemap == null || floorTile == null) return;
        
        // Создаем простой тестовый уровень
        for (int x = -10; x < 10; x++)
        {
            for (int y = -10; y < 10; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                floorTilemap.SetTile(pos, floorTile);
            }
        }
        
        // Создаем стены по периметру
        if (wallTilemap != null && wallTile != null)
        {
            for (int x = -10; x < 10; x++)
            {
                wallTilemap.SetTile(new Vector3Int(x, -10, 0), wallTile);
                wallTilemap.SetTile(new Vector3Int(x, 9, 0), wallTile);
            }
            
            for (int y = -10; y < 10; y++)
            {
                wallTilemap.SetTile(new Vector3Int(-10, y, 0), wallTile);
                wallTilemap.SetTile(new Vector3Int(9, y, 0), wallTile);
            }
        }
        
        Debug.Log("Тестовый уровень создан!");
    }
    
    [ContextMenu("Clear All Tiles")]
    void ClearAllTiles()
    {
        if (floorTilemap != null) floorTilemap.ClearAllTiles();
        if (wallTilemap != null) wallTilemap.ClearAllTiles();
        if (decorationTilemap != null) decorationTilemap.ClearAllTiles();
        
        Debug.Log("Все тайлы очищены!");
    }
    
    [ContextMenu("Create Floor")]
    void CreateFloor()
    {
        if (floorTilemap == null || floorTile == null) return;
        
        for (int x = -10; x < 10; x++)
        {
            for (int y = -10; y < 10; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                floorTilemap.SetTile(pos, floorTile);
            }
        }
        
        Debug.Log("Пол создан!");
    }
} 