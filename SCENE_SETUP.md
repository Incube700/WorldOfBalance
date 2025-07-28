# 🎮 Создание стартовой боевой сцены "Мир Баланса"

## 🚀 Пошаговая настройка сцены

### 1. Создание новой сцены

1. **Создайте новую сцену**:
   - File → New Scene
   - Выберите "2D" template
   - Сохраните как `PvP_Battle.unity`

### 2. Настройка камеры

1. **Выберите Main Camera**
2. **Добавьте компонент CameraFollow**:
   ```csharp
   // Настройки камеры
   Smooth Speed: 5
   Offset: (0, 0, -10)
   ```

3. **Настройте камеру**:
   - Position: (0, 0, -10)
   - Size: 10 (для ортографической камеры)
   - Projection: Orthographic

### 3. Создание NetworkManager

1. **Создайте пустой объект** "NetworkManager"
2. **Добавьте компонент NetworkManagerLobby**
3. **Настройте параметры**:
   ```
   Player Prefab: (назначим позже)
   Spawn Point A: (назначим позже)
   Spawn Point B: (назначим позже)
   Connection Menu: (назначим позже)
   Game HUD: (назначим позже)
   ```

### 4. Создание спавн-точек

1. **Создайте SpawnPointA**:
   - Создайте пустой объект
   - Назовите "SpawnPointA"
   - Position: (-8, 0, 0)
   - Rotation: (0, 0, 0)

2. **Создайте SpawnPointB**:
   - Создайте пустой объект
   - Назовите "SpawnPointB"
   - Position: (8, 0, 0)
   - Rotation: (0, 0, 180)

### 5. Создание арены

1. **Создайте пустой объект** "Arena"
2. **Добавьте компонент SpawnManager**
3. **Настройте параметры**:
   ```
   Arena Size: (20, 20)
   Player A Color: Blue
   Player B Color: Red
   Respawn Delay: 3
   ```

### 6. Создание префаба игрока

1. **Создайте новый префаб** "PlayerPrefab":
   ```
   GameObject "PlayerPrefab"
   ├── SpriteRenderer
   │   ├── Sprite: Square (1x1)
   │   └── Color: White
   ├── Rigidbody2D
   │   ├── Body Type: Kinematic
   │   ├── Gravity Scale: 0
   │   └── Constraints: Freeze Z Rotation
   ├── BoxCollider2D
   │   └── Size: (1, 1)
   ├── NetworkIdentity
   ├── NetworkPlayerController
   ├── HealthSystem
   └── ArmorSystem
   ```

2. **Настройте NetworkPlayerController**:
   ```
   Move Speed: 5
   Rotation Speed: 180
   Fire Rate: 0.5
   Projectile Speed: 10
   Projectile Damage: 10
   Projectile Penetration: 51
   ```

3. **Настройте HealthSystem**:
   ```
   Max Health: 100
   ```

4. **Настройте ArmorSystem**:
   ```
   Front Armor: 50
   Side Armor: 30
   Back Armor: 20
   ```

### 7. Создание префаба снаряда

1. **Создайте новый префаб** "ProjectilePrefab":
   ```
   GameObject "ProjectilePrefab"
   ├── SpriteRenderer
   │   ├── Sprite: Circle (0.2x0.2)
   │   └── Color: Yellow
   ├── Rigidbody2D
   │   ├── Body Type: Kinematic
   │   ├── Gravity Scale: 0
   │   └── Constraints: Freeze Z Rotation
   ├── CircleCollider2D
   │   └── Radius: 0.1
   ├── NetworkIdentity
   └── NetworkProjectile
   ```

2. **Настройте NetworkProjectile**:
   ```
   Speed: 10
   Damage: 25
   Penetration Power: 51
   Lifetime: 5
   Ricochet Threshold: 30
   Bounce Force: 0.8
   ```

### 8. Создание UI

#### Меню подключения:
1. **Создайте Canvas** "ConnectionCanvas"
2. **Настройте Canvas**:
   ```
   Render Mode: Screen Space - Overlay
   UI Scale Mode: Scale With Screen Size
   Reference Resolution: 1920x1080
   ```

3. **Добавьте UI элементы**:
   ```
   Canvas "ConnectionCanvas"
   ├── Panel "Background"
   │   └── Image (черный фон)
   ├── Text "Title"
   │   └── Text: "Мир Баланса"
   ├── InputField "IPInput"
   │   └── Placeholder: "localhost"
   ├── Button "HostButton"
   │   └── Text: "Создать хост"
   ├── Button "JoinButton"
   │   └── Text: "Подключиться"
   ├── Button "DisconnectButton"
   │   └── Text: "Отключиться"
   └── Text "StatusText"
       └── Text: "Не подключен"
   ```

4. **Добавьте компонент ConnectionMenu**

#### Игровой HUD:
1. **Создайте Canvas** "GameCanvas"
2. **Добавьте UI элементы**:
   ```
   Canvas "GameCanvas"
   ├── Panel "HealthPanel"
   │   ├── Slider "HealthBar"
   │   └── Text "HealthText"
   ├── Text "GameStatusText"
   │   └── Text: "ИГРА АКТИВНА"
   ├── Text "PlayerCountText"
   │   └── Text: "Игроков: 2/2"
   └── Text "ConnectionStatusText"
       └── Text: "Подключен"
   ```

3. **Добавьте компонент GameHUD**

### 9. Создание GameManager

1. **Создайте пустой объект** "GameManager"
2. **Добавьте компонент NetworkGameManager**
3. **Настройте параметры**:
   ```
   Match Duration: 300
   Max Score: 10
   Respawn Delay: 3
   ```

### 10. Настройка ссылок

#### NetworkManagerLobby:
- **Player Prefab**: перетащите PlayerPrefab
- **Spawn Point A**: перетащите SpawnPointA
- **Spawn Point B**: перетащите SpawnPointB
- **Connection Menu**: перетащите ConnectionCanvas
- **Game HUD**: перетащите GameCanvas

#### NetworkPlayerController (в префабе):
- **Projectile Prefab**: перетащите ProjectilePrefab
- **Fire Point**: создайте дочерний объект "FirePoint" в позиции (0.5, 0, 0)

#### SpawnManager:
- **Player Prefab**: перетащите PlayerPrefab
- **Spawn Point A**: перетащите SpawnPointA
- **Spawn Point B**: перетащите SpawnPointB

#### NetworkGameManager:
- **Spawn Manager**: перетащите объект с SpawnManager
- **Game HUD**: перетащите GameHUD компонент

### 11. Создание эффектов

#### HitEffect Prefab:
1. **Создайте префаб** "HitEffectPrefab":
   ```
   GameObject "HitEffectPrefab"
   ├── SpriteRenderer
   │   ├── Sprite: Circle (0.5x0.5)
   │   └── Color: Red
   ├── HitEffect
   └── ParticleSystem (опционально)
   ```

#### RicochetEffect Prefab:
1. **Создайте префаб** "RicochetEffectPrefab":
   ```
   GameObject "RicochetEffectPrefab"
   ├── SpriteRenderer
   │   ├── Sprite: Circle (0.3x0.3)
   │   └── Color: Yellow
   ├── RicochetEffect
   └── ParticleSystem (опционально)
   ```

### 12. Настройка слоев и тегов

1. **Создайте теги**:
   - "Player"
   - "Wall"
   - "Projectile"

2. **Создайте слои**:
   - "Player"
   - "Wall"
   - "Projectile"

3. **Назначьте слои**:
   - PlayerPrefab → Layer: "Player"
   - ProjectilePrefab → Layer: "Projectile"
   - Стены → Layer: "Wall"

### 13. Настройка физики

1. **Откройте Edit → Project Settings → Physics 2D**
2. **Настройте Layer Collision Matrix**:
   ```
   Player ↔ Player: ✓
   Player ↔ Wall: ✓
   Player ↔ Projectile: ✓
   Projectile ↔ Wall: ✓
   Projectile ↔ Projectile: ✗
   ```

### 14. Тестирование

1. **Сохраните сцену**
2. **Нажмите Play**
3. **Протестируйте**:
   - Создание хоста
   - Подключение клиента
   - Движение игроков
   - Стрельба
   - Рикошеты от стен
   - Пробитие брони

## 🎯 Ожидаемый результат

После настройки у вас должна быть:

- ✅ **Сетевая игра** с 2 игроками
- ✅ **Физика пробития** с углами попадания
- ✅ **Рикошеты** от стен и брони
- ✅ **UI** для подключения и игры
- ✅ **Система здоровья** и респауна
- ✅ **Визуальные эффекты** попаданий

## 🔧 Отладка

### Полезные команды в консоли:
```
"Игрок создан в позиции SpawnPointA. Всего игроков: 1"
"Hit angle: 45°, Effective armor: 70.7, Penetration: 51, Can penetrate: false"
"Projectile ricocheted from armor!"
```

### Визуальная отладка:
- Включите Gizmos в Scene view
- Проверьте Network Info в Game view
- Используйте OnGUI() для отображения информации

---

**Теперь ваша PvP-игра готова к тестированию! 🎮** 