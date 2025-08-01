# 🧱 Создание Tank объекта в сцене

## 🎯 Шаг 1. Автоматическое создание (Рекомендуется)

### Вариант A: Через меню Unity
1. Откройте Unity
2. В меню выберите **Tools > Create Tank in Scene**
3. Tank объект будет создан автоматически со всеми компонентами

### Вариант B: Ручное создание
Если автоматическое создание не работает, создайте вручную:

## 🎯 Шаг 1. Создайте объект игрока в сцене

### 1.1 Создание базового объекта
- Правый клик в иерархии → **Create Empty**
- Назовите его **Tank**

### 1.2 Добавьте компоненты по порядку:

#### NetworkIdentity
- **Add Component** → **Network Identity**
- Это делает объект сетевым

#### Sprite Renderer
- **Add Component** → **Sprite Renderer**
- **Sprite**: выберите белый квадрат (Square из Sprites/Default)
- **Color**: синий (0, 0.5, 1, 1) или любой на ваш вкус
- **Sorting Order**: 0

#### Rigidbody2D
- **Add Component** → **Rigidbody2D**
- **Body Type**: Dynamic
- **Gravity Scale**: 0
- **Linear Drag**: 0.5
- **Angular Drag**: 0.05
- **Collision Detection**: Continuous
- **Sleep Mode**: Never Sleep
- **Interpolate**: Interpolate
- **Constraints**: Freeze Rotation

#### BoxCollider2D
- **Add Component** → **Box Collider 2D**
- **Size**: 1, 1 (совпадает с размером спрайта)
- **Is Trigger**: false

#### Скрипты (в указанном порядке):
1. **TankController.cs** (наследуется от NetworkBehaviour)
2. **HealthSystem.cs**
3. **ArmorSystem.cs**
4. **ProjectileSpawner.cs**

## 🧪 Шаг 2. Проверьте компоненты

### Порядок важен:
| Компонент | Назначение |
|-----------|------------|
| NetworkIdentity | Делает объект сетевым |
| TankController | Управление, движение, стрельба |
| HealthSystem | Здоровье |
| ArmorSystem | Расчёт урона |
| ProjectileSpawner | Создаёт снаряды |

### Проверка:
- ✅ Все скрипты компилируются без ошибок
- ✅ Mirror установлен и работает
- ✅ Tank объект выделен в иерархии

## 💾 Шаг 3. Создайте префаб

### Вариант A: Автоматически
1. В меню выберите **Tools > Create Tank Prefab from Scene**
2. Префаб будет создан автоматически

### Вариант B: Вручную
1. Перетащите **Tank** из сцены в папку **Assets/Prefabs**
2. Назовите его **Tank.prefab**
3. Удалите **Tank** из сцены — Mirror сам будет спавнить его

## 🧠 Шаг 4. Подключите в NetworkManager

### Вариант A: Автоматически
1. В меню выберите **Tools > Setup NetworkManager**
2. NetworkManager будет настроен автоматически

### Вариант B: Вручную
1. Найдите объект **NetworkManager** в сцене
2. В поле **Player Prefab** установите **Tank.prefab**

## 🧨 Шаг 5. Тест

### Быстрый тест:
1. Нажмите **Play**
2. Нажмите **Host** в NetworkManagerHUD
3. Танчик должен заспавниться и двигаться

### Управление:
- **WASD** - движение
- **ЛКМ** - стрельба
- **Мышь** - прицеливание

## 🐛 Возможные проблемы

### Если скрипты красные:
1. Проверьте, что Mirror установлен
2. Убедитесь, что все using директивы правильные
3. Перезапустите Unity

### Если Tank не спавнится:
1. Проверьте, что Tank.prefab назначен в NetworkManager
2. Убедитесь, что NetworkIdentity есть на префабе
3. Проверьте консоль на ошибки

### Если движение не работает:
1. Проверьте, что Rigidbody2D настроен правильно
2. Убедитесь, что TankController наследуется от NetworkBehaviour
3. Проверьте, что isLocalPlayer работает

## 🎮 Готовый prompt для Cursor

```markdown
1. Создай GameObject 'Tank', добавь компоненты:
   - SpriteRenderer (белый квадрат)
   - Rigidbody2D (Dynamic, Drag 0.5, Gravity 0)
   - BoxCollider2D
   - NetworkIdentity
   - TankController.cs (NetworkBehaviour)
   - HealthSystem.cs
   - ArmorSystem.cs
   - ProjectileSpawner.cs
2. Сохрани как префаб Tank.prefab в папку Assets/Prefabs
3. Удали Tank из сцены, чтобы не дублировался
4. В NetworkManager укажи Tank.prefab как Player Prefab
```

## 🚀 Готово!

Теперь у вас есть полностью настроенный Tank объект с:
- ✅ Сетевым взаимодействием
- ✅ Физикой и коллизиями
- ✅ Системой здоровья и брони
- ✅ Стрельбой и снарядами
- ✅ Автоматическим спавном через Mirror

Играйте и наслаждайтесь! 🎯 