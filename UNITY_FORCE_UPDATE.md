# 🔄 Принудительное обновление Unity

## ✅ Кэш Unity полностью очищен!

Мы удалили все кэши Unity:
- ❌ `Library/` - основной кэш Unity
- ❌ `Logs/` - логи Unity  
- ❌ `UserSettings/` - пользовательские настройки

## 🎯 Что делать дальше:

### 1. **Закройте Unity полностью**
- Выйдите из Unity
- Убедитесь, что процесс Unity завершен

### 2. **Откройте проект заново**
- Запустите Unity Hub
- Нажмите "Open"
- Выберите папку: `/Users/serg/Desktop/Projeсts/My project`
- Дождитесь полной компиляции (2-5 минут)

### 3. **Проверьте Project Window**
После открытия проекта в Project Window должны быть видны:

#### 📁 Scenes:
- ✅ `TestScene.unity` (новая тестовая сцена)
- ✅ `PvP_Battle.unity` (основная сцена)
- ✅ `SampleScene.unity`

#### 📁 Prefabs:
- ✅ `Player.prefab` (готов к использованию)
- ✅ `Projectile.prefab` (готов к использованию)

#### 📁 Scripts:
- ✅ `Player/NetworkPlayerController.cs`
- ✅ `Projectile/NetworkProjectile.cs`
- ✅ `Systems/ArmorSystem.cs`
- ✅ `Systems/HealthSystem.cs`
- ✅ `Networking/NetworkManagerLobby.cs`

### 4. **Откройте TestScene**
- Дважды кликните на `Assets/Scenes/TestScene.unity`
- Сцена должна открыться с объектами:
  - Main Camera
  - Directional Light
  - NetworkManager
  - SpawnPointA
  - SpawnPointB

### 5. **Настройте NetworkManager**
- Выберите объект `NetworkManager` в сцене
- В Inspector нажмите **Add Component**
- Найдите и добавьте `NetworkManagerLobby`
- Назначьте префабы:
  - **Player Prefab:** `Assets/Prefabs/Player.prefab`
  - **Spawn Point A:** `SpawnPointA`
  - **Spawn Point B:** `SpawnPointB`

### 6. **Настройте Player Prefab**
- Откройте `Assets/Prefabs/Player.prefab`
- В NetworkPlayerController назначьте:
  - **Projectile Prefab:** `Assets/Prefabs/Projectile.prefab`
  - **Fire Point:** дочерний объект `FirePoint`
  - **Все компоненты:** SpriteRenderer, Rigidbody2D, ArmorSystem, HealthSystem

### 7. **Запустите тест**
- Нажмите **Play** в Unity
- В консоли должно появиться: "Сервер запущен"
- Игрок должен появиться в позиции SpawnPointA

## 🔧 Если файлы все еще не видны:

### Вариант 1: Перезапуск Unity
1. Закройте Unity полностью
2. Откройте проект заново через Unity Hub
3. Дождитесь компиляции (2-5 минут)

### Вариант 2: Проверка пути
1. Убедитесь, что открыли правильный проект
2. Путь должен быть: `/Users/serg/Desktop/Projeсts/My project`
3. В папке должны быть файлы `Assets/`, `ProjectSettings/`, `Packages/`

### Вариант 3: Создание нового проекта
1. Создайте новый 2D URP проект
2. Скопируйте папку `Assets/` в новый проект
3. Скопируйте `ProjectSettings/` и `Packages/`

## 📋 Проверка готовности:

После открытия проекта проверьте:

- [ ] Unity открылся без ошибок
- [ ] В Project Window видны все папки
- [ ] TestScene.unity можно открыть
- [ ] Player.prefab и Projectile.prefab видны
- [ ] Все скрипты компилируются без ошибок
- [ ] NetworkManagerLobby можно добавить в сцену

## 🚨 Важно!

- **НЕ открывайте отдельные файлы** - открывайте весь проект
- **Убедитесь, что выбрана правильная папка** - `My project`
- **Дождитесь полной компиляции** перед работой
- **Проверьте версию Unity** - должна быть совместима с URP

## 📁 Структура проекта после обновления:

```
My project/
├── Assets/
│   ├── Scenes/
│   │   ├── TestScene.unity ✅ (новая)
│   │   ├── PvP_Battle.unity ✅
│   │   └── SampleScene.unity ✅
│   ├── Prefabs/
│   │   ├── Player.prefab ✅ (готов)
│   │   └── Projectile.prefab ✅ (готов)
│   └── Scripts/ ✅ (все готово)
├── ProjectSettings/ ✅
├── Packages/ ✅
└── [документация] ✅
```

---

**Unity теперь должен увидеть все новые файлы! 🚀** 