# 🎯 Полная настройка Tank объекта

## 🚀 Быстрый способ (Рекомендуется)

### Вариант A: Автоматическая настройка
1. Откройте Unity
2. В меню выберите **Tools > Complete Tank Setup**
3. Все будет настроено автоматически!

### Вариант B: Пошаговая настройка
Выполните команды по порядку:

1. **Tools > Create Tank in Scene** - создает Tank в сцене
2. **Tools > Create Tank Prefab from Scene** - создает префаб
3. **Tools > Delete Tank from Scene** - удаляет из сцены
4. **Tools > Setup NetworkManager** - настраивает NetworkManager

## 🧪 Проверка настройки

После настройки выполните:
**Tools > Validate Tank Setup** - проверит все компоненты

## 📋 Что создается автоматически

### Tank GameObject с компонентами:

| Компонент | Настройки |
|-----------|-----------|
| **NetworkIdentity** | Делает объект сетевым |
| **SpriteRenderer** | Белый квадрат, синий цвет (0, 0.5, 1, 1) |
| **Rigidbody2D** | Dynamic, Drag 0.5, Gravity 0, Freeze Rotation |
| **BoxCollider2D** | Размер 1x1, не триггер |
| **TankController** | Управление и стрельба (NetworkBehaviour) |
| **HealthSystem** | Система здоровья |
| **ArmorSystem** | Система брони |
| **ProjectileSpawner** | Создание снарядов |

### Дополнительно создается:
- **Turret** дочерний объект (башня танка)
- **Tank.prefab** в папке Assets/Prefabs
- **NetworkManager** с настроенным Player Prefab

## 🎮 Тестирование

### Быстрый тест:
1. Нажмите **Play** в Unity
2. Нажмите **Host** в NetworkManagerHUD
3. Танчик должен заспавниться и двигаться

### Управление:
- **WASD** - движение
- **ЛКМ** - стрельба
- **Мышь** - прицеливание

## 🐛 Возможные проблемы

### Если Tank не создается:
1. Проверьте, что все скрипты компилируются
2. Убедитесь, что Mirror установлен
3. Проверьте консоль на ошибки

### Если Tank не спавнится:
1. Выполните **Tools > Validate Tank Setup**
2. Проверьте, что Tank.prefab назначен в NetworkManager
3. Убедитесь, что NetworkIdentity есть на префабе

### Если движение не работает:
1. Проверьте настройки Rigidbody2D
2. Убедитесь, что TankController наследуется от NetworkBehaviour
3. Проверьте, что isLocalPlayer работает

## 🎯 Готовый prompt для Cursor

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

## ✅ Результат

После выполнения всех шагов у вас будет:
- ✅ Полностью настроенный Tank объект
- ✅ Сетевое взаимодействие через Mirror
- ✅ Физика и коллизии
- ✅ Система здоровья и брони
- ✅ Стрельба и снаряды
- ✅ Автоматический спавн через NetworkManager

## 🚀 Готово к игре!

Теперь можно:
1. Открыть Unity
2. Выполнить **Tools > Complete Tank Setup**
3. Нажать Play
4. Нажать Host
5. Играть! 🎮 