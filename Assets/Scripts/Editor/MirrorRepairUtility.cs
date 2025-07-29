using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public static class MirrorRepairUtility
{
    private const string mirrorGitUrl = "https://github.com/vis2k/Mirror.git?path=/Assets/Mirror";

    [MenuItem("Tools/🛠 Починить Mirror")]
    public static void FixMirrorPackage()
    {
        Debug.Log("<color=orange>🔍 MirrorRepairUtility: Проверка зависимостей...</color>");

        string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
        if (!File.Exists(manifestPath))
        {
            Debug.LogError("❌ Не найден manifest.json. Убедитесь, что вы в Unity-проекте.");
            return;
        }

        string manifestText = File.ReadAllText(manifestPath);

        // Проверка на наличие Mirror в manifest.json
        if (manifestText.Contains("com.vis2k.mirror"))
        {
            Debug.Log("<color=green>✅ Mirror уже указан в манифесте.</color>");
        }
        else
        {
            Debug.LogWarning("⚠️ Mirror не найден в manifest.json. Добавляю...");

            // Попробуем вставить Mirror в секцию dependencies
            int index = manifestText.IndexOf("\"dependencies\":");
            if (index != -1)
            {
                int bracketIndex = manifestText.IndexOf('{', index);
                int insertPos = manifestText.IndexOf('\n', bracketIndex) + 1;
                string mirrorEntry = "    \"com.vis2k.mirror\": \"" + mirrorGitUrl + "\",\n";
                manifestText = manifestText.Insert(insertPos, mirrorEntry);
                File.WriteAllText(manifestPath, manifestText);

                Debug.Log("<color=green>✅ Mirror добавлен в манифест. Unity скачает его после перезапуска.</color>");
            }
            else
            {
                Debug.LogError("❌ Не удалось найти раздел dependencies в manifest.json.");
            }
        }

        // Очистка возможных конфликтных локальных пакетов
        string localMirror = Path.Combine(Application.dataPath, "Mirror");
        if (Directory.Exists(localMirror))
        {
            Debug.LogWarning("🧹 Найдена старая локальная папка Assets/Mirror. Удалите её вручную для полной очистки.");
        }

        Debug.Log("<color=cyan>📦 Завершено. Теперь удалите папку Library и перезапустите Unity.</color>");
    }
}