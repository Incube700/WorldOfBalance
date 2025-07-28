#!/bin/bash

echo "🔧 Исправление ошибок Unity..."

# Удаляем кэш Unity
echo "🗑️ Удаляем кэш Unity..."
rm -rf Library
rm -rf Logs
rm -rf UserSettings

# Проверяем и исправляем .meta файлы
echo "🔍 Проверяем .meta файлы..."

# Функция для генерации GUID
generate_guid() {
    python3 -c "import uuid; print(str(uuid.uuid4()).replace('-', ''))"
}

# Исправляем Player.prefab.meta
echo "📝 Исправляем Player.prefab.meta..."
cat > Assets/Prefabs/Player.prefab.meta << EOF
fileFormatVersion: 2
guid: 7cfed9b22e1144dbb2e7a3ca88e95f50
PrefabImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# Исправляем Projectile.prefab.meta
echo "📝 Исправляем Projectile.prefab.meta..."
cat > Assets/Prefabs/Projectile.prefab.meta << EOF
fileFormatVersion: 2
guid: 2ce46219f7094f4a819c5c603887bb52
PrefabImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# Проверяем все .meta файлы скриптов
echo "📝 Проверяем .meta файлы скриптов..."

# NetworkPlayerController.cs.meta
cat > Assets/Scripts/Player/NetworkPlayerController.cs.meta << EOF
fileFormatVersion: 2
guid: 9fb158204d9549c498fb8ca3ba927d7e
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# NetworkProjectile.cs.meta
cat > Assets/Scripts/Projectile/NetworkProjectile.cs.meta << EOF
fileFormatVersion: 2
guid: cd4509e9950b4c6aa7ea250ae6740830
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# ArmorSystem.cs.meta
cat > Assets/Scripts/Systems/ArmorSystem.cs.meta << EOF
fileFormatVersion: 2
guid: 8240138469534dc39ccad5674c04e342
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# HealthSystem.cs.meta
cat > Assets/Scripts/Systems/HealthSystem.cs.meta << EOF
fileFormatVersion: 2
guid: 55d38d7d957d4a9cb0f230b2f31a5365
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# NetworkGameManager.cs.meta
cat > Assets/Scripts/Systems/NetworkGameManager.cs.meta << EOF
fileFormatVersion: 2
guid: 71505f67c4ab46e6b7b736be7f0728ae
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# NetworkManagerLobby.cs.meta
cat > Assets/Scripts/Networking/NetworkManagerLobby.cs.meta << EOF
fileFormatVersion: 2
guid: 75dda2b119834d81a2655bb3cf798fc3
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# HitEffect.cs.meta
cat > Assets/Scripts/Effects/HitEffect.cs.meta << EOF
fileFormatVersion: 2
guid: f502baac93f94bc1b38450dc9cb83d16
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# RicochetEffect.cs.meta
cat > Assets/Scripts/Effects/RicochetEffect.cs.meta << EOF
fileFormatVersion: 2
guid: 9894e9d0fbde4478a2a12cb06709d6ba
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# SpawnManager.cs.meta
cat > Assets/Scripts/Map/SpawnManager.cs.meta << EOF
fileFormatVersion: 2
guid: 194e70c1b8374caaa4f4874057aaaf68
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# ConnectionMenu.cs.meta
cat > Assets/Scripts/UI/ConnectionMenu.cs.meta << EOF
fileFormatVersion: 2
guid: a15174179dce4e349c2e30435a2f00dd
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

# GameHUD.cs.meta
cat > Assets/Scripts/UI/GameHUD.cs.meta << EOF
fileFormatVersion: 2
guid: 15da14552e444cf6bf769c0f018701ee
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
EOF

echo "✅ Все .meta файлы исправлены!"
echo "🔄 Теперь закройте Unity и откройте проект заново" 