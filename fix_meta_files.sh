#!/bin/bash

# Скрипт для исправления .meta файлов с правильными GUID
echo "Исправление .meta файлов..."

# Функция для генерации правильного GUID
generate_guid() {
    python3 -c "import uuid; print(str(uuid.uuid4()).replace('-', ''))"
}

# Список всех .meta файлов скриптов
meta_files=(
    "Assets/Scripts/CameraFollow.cs.meta"
    "Assets/Scripts/Effects/HitEffect.cs.meta"
    "Assets/Scripts/Effects/RicochetEffect.cs.meta"
    "Assets/Scripts/FPSController.cs.meta"
    "Assets/Scripts/Map/SpawnManager.cs.meta"
    "Assets/Scripts/Networking/NetworkManagerLobby.cs.meta"
    "Assets/Scripts/Player/NetworkPlayerController.cs.meta"
    "Assets/Scripts/Player/PlayerController.cs.meta"
    "Assets/Scripts/Projectile/NetworkProjectile.cs.meta"
    "Assets/Scripts/Projectile/Projectile.cs.meta"
    "Assets/Scripts/Systems/ArmorSystem.cs.meta"
    "Assets/Scripts/Systems/DamageSystem.cs.meta"
    "Assets/Scripts/Systems/GameManager.cs.meta"
    "Assets/Scripts/Systems/HealthSystem.cs.meta"
    "Assets/Scripts/Systems/NetworkGameManager.cs.meta"
    "Assets/Scripts/UI/ConnectionMenu.cs.meta"
    "Assets/Scripts/UI/GameHUD.cs.meta"
    "Assets/Scripts/UI/GameHUD.cs.meta"
)

# Исправляем каждый .meta файл
for meta_file in "${meta_files[@]}"; do
    if [ -f "$meta_file" ]; then
        echo "Исправляю: $meta_file"
        guid=$(generate_guid)
        
        # Создаем новый .meta файл с правильным GUID
        cat > "$meta_file" << EOF
fileFormatVersion: 2
guid: $guid
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
        echo "  ✅ Исправлен с GUID: $guid"
    else
        echo "  ❌ Файл не найден: $meta_file"
    fi
done

echo "Все .meta файлы исправлены!" 