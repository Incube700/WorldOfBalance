using UnityEngine;
using UnityEditor;
using System.IO;

public class MirrorCleaner : MonoBehaviour
{
    [MenuItem("Tools/Clean Mirror Package")]
    public static void CleanMirrorPackage()
    {
        Debug.Log("=== ОЧИСТКА MIRROR ПАКЕТА ===");
        
        // 1. Удаляем папку Mirror из Assets
        string mirrorPath = "Assets/Mirror";
        if (Directory.Exists(mirrorPath))
        {
            Directory.Delete(mirrorPath, true);
            Debug.Log("✅ Папка Mirror удалена из Assets");
        }
        else
        {
            Debug.Log("ℹ️ Папка Mirror не найдена в Assets");
        }
        
        // 2. Удаляем Mirror из Packages/manifest.json
        RemoveMirrorFromManifest();
        
        // 3. Очищаем кэш пакетов
        ClearPackageCache();
        
        // 4. Перезапускаем Unity
        RestartUnity();
        
        Debug.Log("=== ОЧИСТКА ЗАВЕРШЕНА ===");
        Debug.Log("Unity перезапустится автоматически!");
    }
    
    static void RemoveMirrorFromManifest()
    {
        string manifestPath = "Packages/manifest.json";
        if (File.Exists(manifestPath))
        {
            try
            {
                string content = File.ReadAllText(manifestPath);
                if (content.Contains("com.vis2k.mirror"))
                {
                    // Удаляем строку с Mirror
                    string[] lines = content.Split('\n');
                    var newLines = new System.Collections.Generic.List<string>();
                    
                    bool skipNext = false;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("com.vis2k.mirror"))
                        {
                            skipNext = true;
                            continue;
                        }
                        
                        if (skipNext && lines[i].Contains("}"))
                        {
                            skipNext = false;
                            continue;
                        }
                        
                        if (!skipNext)
                        {
                            newLines.Add(lines[i]);
                        }
                    }
                    
                    string newContent = string.Join("\n", newLines);
                    File.WriteAllText(manifestPath, newContent);
                    Debug.Log("✅ Mirror удален из manifest.json");
                }
                else
                {
                    Debug.Log("ℹ️ Mirror не найден в manifest.json");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Ошибка при редактировании manifest.json: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Файл manifest.json не найден");
        }
    }
    
    static void ClearPackageCache()
    {
        string cachePath = "Library/PackageCache";
        if (Directory.Exists(cachePath))
        {
            try
            {
                // Удаляем временные файлы Mirror
                var tempDirs = Directory.GetDirectories(cachePath, ".tmp-*-*");
                foreach (var dir in tempDirs)
                {
                    if (dir.Contains("Mirror") || dir.Contains("vis2k"))
                    {
                        Directory.Delete(dir, true);
                        Debug.Log($"✅ Удален кэш: {Path.GetFileName(dir)}");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Ошибка при очистке кэша: {e.Message}");
            }
        }
    }
    
    static void RestartUnity()
    {
        Debug.Log("🔄 Перезапуск Unity...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.OpenProject(System.IO.Directory.GetCurrentDirectory());
        #endif
    }
    
    [MenuItem("Tools/Check Mirror Status")]
    public static void CheckMirrorStatus()
    {
        Debug.Log("=== ПРОВЕРКА СТАТУСА MIRROR ===");
        
        // Проверяем папку Mirror
        string mirrorPath = "Assets/Mirror";
        if (Directory.Exists(mirrorPath))
        {
            Debug.LogWarning("⚠️ Папка Mirror найдена в Assets");
        }
        else
        {
            Debug.Log("✅ Папка Mirror отсутствует в Assets");
        }
        
        // Проверяем manifest.json
        string manifestPath = "Packages/manifest.json";
        if (File.Exists(manifestPath))
        {
            string content = File.ReadAllText(manifestPath);
            if (content.Contains("com.vis2k.mirror"))
            {
                Debug.LogWarning("⚠️ Mirror найден в manifest.json");
            }
            else
            {
                Debug.Log("✅ Mirror отсутствует в manifest.json");
            }
        }
        
        // Проверяем кэш
        string cachePath = "Library/PackageCache";
        if (Directory.Exists(cachePath))
        {
            var tempDirs = Directory.GetDirectories(cachePath, ".tmp-*-*");
            bool foundMirror = false;
            foreach (var dir in tempDirs)
            {
                if (dir.Contains("Mirror") || dir.Contains("vis2k"))
                {
                    Debug.LogWarning($"⚠️ Найден кэш Mirror: {Path.GetFileName(dir)}");
                    foundMirror = true;
                }
            }
            
            if (!foundMirror)
            {
                Debug.Log("✅ Кэш Mirror очищен");
            }
        }
        
        Debug.Log("=== ПРОВЕРКА ЗАВЕРШЕНА ===");
    }
} 