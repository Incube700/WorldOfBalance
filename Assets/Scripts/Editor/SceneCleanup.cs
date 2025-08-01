using UnityEngine;
using UnityEditor;

public class SceneCleanup : MonoBehaviour
{
    [MenuItem("Tools/Clean Missing Scripts")]
    static void CleanMissingScripts()
    {
        // Find all GameObjects in scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int cleanedCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            // Get all components
            Component[] components = obj.GetComponents<Component>();
            
            for (int i = components.Length - 1; i >= 0; i--)
            {
                // If component is null (missing script)
                if (components[i] == null)
                {
                    // Remove the missing component
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                    cleanedCount++;
                    Debug.Log($"Cleaned missing script from: {obj.name}");
                    break; // Only need to clean once per object
                }
            }
        }
        
        Debug.Log($"Scene cleanup complete! Removed {cleanedCount} missing script references.");
    }
}