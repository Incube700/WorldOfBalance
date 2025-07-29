#!/bin/bash

echo "🧹 Полная очистка и переустановка Unity проекта..."

# Удаляем все кэши Unity
echo "🗑️ Удаляем кэши Unity..."
rm -rf Library
rm -rf Logs
rm -rf UserSettings
rm -f Packages/packages-lock.json

# Создаем чистый manifest.json с Mirror
echo "📦 Создаем чистый manifest.json..."
cat > Packages/manifest.json << 'EOF'
{
  "dependencies": {
    "com.unity.2d.animation": "11.0.0",
    "com.unity.2d.aseprite": "1.2.5",
    "com.unity.2d.psdimporter": "10.1.1",
    "com.unity.2d.sprite": "1.0.0",
    "com.unity.2d.spriteshape": "11.0.0",
    "com.unity.2d.tilemap": "1.0.0",
    "com.unity.2d.tilemap.extras": "4.3.1",
    "com.unity.collab-proxy": "2.8.2",
    "com.unity.ide.rider": "3.0.36",
    "com.unity.ide.visualstudio": "2.0.23",
    "com.unity.inputsystem": "1.14.0",
    "com.unity.multiplayer.center": "1.0.0",
    "com.unity.multiplayer.mirror": "70.0.0",
    "com.unity.render-pipelines.universal": "17.1.0",
    "com.unity.test-framework": "1.5.1",
    "com.unity.timeline": "1.8.7",
    "com.unity.ugui": "2.0.0",
    "com.unity.visualscripting": "1.9.7",
    "com.unity.modules.accessibility": "1.0.0",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.cloth": "1.0.0",
    "com.unity.modules.director": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.terrain": "1.0.0",
    "com.unity.modules.terrainphysics": "1.0.0",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.umbra": "1.0.0",
    "com.unity.modules.unityanalytics": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestassetbundle": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.unitywebrequestwww": "1.0.0",
    "com.unity.modules.vehicles": "1.0.0",
    "com.unity.modules.video": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.wind": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
  }
}
EOF

echo "✅ Manifest.json создан с Mirror"

# Проверяем, что все .meta файлы на месте
echo "📋 Проверяем .meta файлы..."
ls -la Assets/Prefabs/*.meta
ls -la Assets/Scenes/*.meta

echo "🔄 Теперь закройте Unity и откройте проект заново"
echo "📦 Unity автоматически установит все пакеты при открытии"
echo "⏱️ Время установки: 5-10 минут"