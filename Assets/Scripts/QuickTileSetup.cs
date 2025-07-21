using UnityEngine;
using UnityEngine.Tilemaps;

public class QuickTileSetup : MonoBehaviour
{
    [Header("Быстрая настройка тайлов")]
    public bool setupOnStart = true;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupBasicTiles();
        }
    }
    
    [ContextMenu("Настроить базовые тайлы")]
    public void SetupBasicTiles()
    {
        // Находим или создаем Grid
        Grid grid = FindObjectOfType<Grid>();
        if (grid == null)
        {
            GameObject gridObj = new GameObject("Grid");
            grid = gridObj.AddComponent<Grid>();
            grid.cellSize = Vector3.one;
            grid.cellGap = Vector3.zero;
        }
        
        // Создаем тайлмапы если их нет
        CreateTilemapIfNeeded(grid, "Floor", -1);
        CreateTilemapIfNeeded(grid, "Walls", 0);
        CreateTilemapIfNeeded(grid, "Decorations", 1);
        
        Debug.Log("Базовые тайлмапы созданы! Теперь можете рисовать тайлы.");
    }
    
    void CreateTilemapIfNeeded(Grid grid, string name, int sortingOrder)
    {
        Tilemap tilemap = grid.GetComponentInChildren<Tilemap>();
        if (tilemap == null || tilemap.name != name)
        {
            GameObject tilemapObj = new GameObject(name);
            tilemapObj.transform.SetParent(grid.transform);
            tilemap = tilemapObj.AddComponent<Tilemap>();
            tilemapObj.AddComponent<TilemapRenderer>();
        }
        
        // Настраиваем рендерер
        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = sortingOrder;
        }
        
        tilemap.name = name;
    }
    
    [ContextMenu("Очистить все тайлы")]
    public void ClearAllTiles()
    {
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            tilemap.ClearAllTiles();
        }
        Debug.Log("Все тайлы очищены!");
    }
} 