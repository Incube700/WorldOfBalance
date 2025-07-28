# ⚡ Быстрая настройка "Мир Баланса"

## 🚀 Быстрый старт (5 минут)

### 1. Установка Mirror
1. Window → Package Manager
2. "+" → "Add package from git URL"
3. Введите: `https://github.com/vis2k/Mirror.git?path=/Assets/Mirror`
4. Нажмите "Add"

### 2. Создание базовой сцены

#### Создайте новую сцену `PvP_Battle.unity`:

1. **Добавьте NetworkManagerLobby**:
   - Создайте пустой объект "NetworkManager"
   - Добавьте компонент `NetworkManagerLobby`

2. **Создайте спавн-точки**:
   ```
   SpawnPointA (позиция: -5, 0, 0)
   SpawnPointB (позиция: 5, 0, 0)
   ```

3. **Создайте стены**:
   ```
   Wall1 (позиция: 0, 3, 0, размер: 10x1)
   Wall2 (позиция: 0, -3, 0, размер: 10x1)
   Wall3 (позиция: 3, 0, 0, размер: 1x6)
   Wall4 (позиция: -3, 0, 0, размер: 1x6)
   ```

### 3. Создание префабов

#### Player Prefab:
1. Создайте GameObject "PlayerPrefab"
2. Добавьте компоненты:
   - `NetworkPlayerController`
   - `HealthSystem`
   - `ArmorSystem`
   - `SpriteRenderer` (квадрат 1x1, цвет: белый)
   - `Rigidbody2D` (Kinematic, Gravity Scale: 0)
   - `BoxCollider2D`
   - `NetworkIdentity`

#### Projectile Prefab:
1. Создайте GameObject "ProjectilePrefab"
2. Добавьте компоненты:
   - `NetworkProjectile`
   - `SpriteRenderer` (круг 0.2x0.2, цвет: желтый)
   - `Rigidbody2D` (Kinematic)
   - `CircleCollider2D`
   - `NetworkIdentity`

### 4. Настройка NetworkManager

В NetworkManagerLobby назначьте:
- **Player Prefab**: PlayerPrefab
- **Spawn Point A**: SpawnPointA
- **Spawn Point B**: SpawnPointB

### 5. Создание UI

#### Меню подключения:
1. Создайте Canvas
2. Добавьте UI элементы:
   - Button "Создать хост"
   - Button "Подключиться"
   - InputField "IP адрес"
   - Text "Статус"
3. Добавьте компонент `ConnectionMenu`

#### Игровой HUD:
1. Создайте Canvas
2. Добавьте UI элементы:
   - Slider "Health Bar"
   - Text "Health Text"
   - Text "Game Status"
3. Добавьте компонент `GameHUD`

### 6. Настройка камеры

1. Выберите Main Camera
2. Добавьте компонент `CameraFollow`
3. Установите `smoothSpeed = 5f`

## 🎮 Тестирование

### Локальная игра:
1. Нажмите Play
2. Нажмите "Создать хост"
3. Второй игрок: "Подключиться" (IP: localhost)

### Управление:
- **WASD** - движение
- **Мышь** - прицеливание
- **ЛКМ** - выстрел

## ⚙️ Настройка параметров

### ArmorSystem (на PlayerPrefab):
```
Front Armor: 50
Side Armor: 30
Back Armor: 20
```

### NetworkProjectile (на ProjectilePrefab):
```
Penetration Power: 51
Ricochet Threshold: 30
Bounce Force: 0.8
```

## 🔧 Устранение проблем

### Ошибка "NetworkManagerLobby не найден":
- Убедитесь, что NetworkManagerLobby добавлен на сцену
- Проверьте, что Mirror установлен

### Игроки не появляются:
- Проверьте, что PlayerPrefab назначен в NetworkManager
- Убедитесь, что спавн-точки существуют

### Снаряды не стреляют:
- Проверьте, что ProjectilePrefab назначен в NetworkPlayerController
- Убедитесь, что FirePoint существует на игроке

### Нет рикошетов:
- Проверьте, что стены имеют тег "Wall"
- Убедитесь, что ricochetThreshold > 0

## 📱 Сборка

### Windows:
1. File → Build Settings
2. PC, Mac & Linux Standalone
3. Build

### Android:
1. File → Build Settings
2. Android
3. Build

---

**Готово! Ваша PvP-игра настроена за 5 минут! 🎮** 