# 🚀 План сборки прототипа "Мир Баланса"

## 📋 Анализ текущего состояния

### ✅ Что уже готово:
- ✅ Все скрипты созданы с правильными .meta файлами
- ✅ Сетевая архитектура через Mirror
- ✅ Логика пробития брони и рикошетов
- ✅ UI компоненты (ConnectionMenu, GameHUD)
- ✅ Системы здоровья и брони
- ✅ Эффекты попаданий и рикошетов

### ❌ Что нужно настроить:
- ❌ Сцена не настроена
- ❌ Префабы не созданы
- ❌ UI не настроен
- ❌ Mirror не установлен
- ❌ Спавн-точки не размещены

## 🎯 Пошаговый план сборки

### Этап 1: Подготовка проекта (30 мин)
- [ ] **1.1** Установить Mirror через Package Manager
- [ ] **1.2** Настроить URP (Universal Render Pipeline)
- [ ] **1.3** Создать базовую сцену PvP_Battle
- [ ] **1.4** Настроить камеру и освещение

### Этап 2: Создание префабов (45 мин)
- [ ] **2.1** Создать Player Prefab
  - [ ] Добавить SpriteRenderer (квадрат)
  - [ ] Добавить Rigidbody2D
  - [ ] Добавить Collider2D
  - [ ] Добавить NetworkIdentity
  - [ ] Добавить NetworkPlayerController
  - [ ] Добавить ArmorSystem
  - [ ] Добавить HealthSystem
  - [ ] Настроить FirePoint (Transform)

- [ ] **2.2** Создать Projectile Prefab
  - [ ] Добавить SpriteRenderer (круг)
  - [ ] Добавить Rigidbody2D
  - [ ] Добавить Collider2D
  - [ ] Добавить NetworkIdentity
  - [ ] Добавить NetworkProjectile
  - [ ] Настроить физику

- [ ] **2.3** Создать Effect Prefabs
  - [ ] HitEffect Prefab
  - [ ] RicochetEffect Prefab

### Этап 3: Настройка сцены (30 мин)
- [ ] **3.1** Добавить NetworkManagerLobby
- [ ] **3.2** Создать спавн-точки (SpawnPointA, SpawnPointB)
- [ ] **3.3** Добавить стены для рикошетов
- [ ] **3.4** Настроить границы арены
- [ ] **3.5** Добавить камеру с CameraFollow

### Этап 4: Настройка UI (30 мин)
- [ ] **4.1** Создать Canvas
- [ ] **4.2** Настроить ConnectionMenu
  - [ ] Кнопка "Host Game"
  - [ ] Кнопка "Join Game"
  - [ ] Поле ввода IP адреса
  - [ ] Кнопка "Disconnect"

- [ ] **4.3** Настроить GameHUD
  - [ ] Полоска здоровья
  - [ ] Счетчик игроков
  - [ ] Статус подключения

### Этап 5: Настройка NetworkManager (15 мин)
- [ ] **5.1** Назначить Player Prefab в NetworkManagerLobby
- [ ] **5.2** Назначить спавн-точки
- [ ] **5.3** Назначить UI элементы
- [ ] **5.4** Настроить сцену в Build Settings

### Этап 6: Тестирование (30 мин)
- [ ] **6.1** Запустить в редакторе
- [ ] **6.2** Протестировать подключение
- [ ] **6.3** Протестировать движение
- [ ] **6.4** Протестировать стрельбу
- [ ] **6.5** Протестировать рикошеты
- [ ] **6.6** Протестировать пробитие брони

## 🔧 Детальные задачи

### Задача 1.1: Установка Mirror
```bash
# В Unity Package Manager добавить:
com.unity.multiplayer.mirror
```

### Задача 2.1: Player Prefab
```
GameObject "Player"
├── SpriteRenderer (квадрат 1x1, цвет по умолчанию)
├── Rigidbody2D (Gravity Scale = 0, Constraints = Freeze Z)
├── BoxCollider2D (размер 1x1)
├── NetworkIdentity
├── NetworkPlayerController
├── ArmorSystem
├── HealthSystem
└── FirePoint (дочерний объект)
    └── Transform (позиция 0.5, 0, 0)
```

### Задача 2.2: Projectile Prefab
```
GameObject "Projectile"
├── SpriteRenderer (круг, красный цвет)
├── Rigidbody2D (Gravity Scale = 0)
├── CircleCollider2D (радиус 0.1)
├── NetworkIdentity
└── NetworkProjectile
```

### Задача 3.1: Сцена PvP_Battle
```
Scene "PvP_Battle"
├── NetworkManagerLobby
├── Main Camera
│   └── CameraFollow
├── SpawnPointA (Transform)
├── SpawnPointB (Transform)
├── Walls (GameObject)
│   ├── Wall_Top
│   ├── Wall_Bottom
│   ├── Wall_Left
│   └── Wall_Right
└── Canvas
    ├── ConnectionMenu
    └── GameHUD
```

## ⚡ Быстрый старт (5 минут)

### Минимальная настройка для тестирования:

1. **Откройте Unity** и проект
2. **Установите Mirror** через Package Manager
3. **Создайте простую сцену** с камерой
4. **Создайте Player Prefab** (квадрат + скрипты)
5. **Создайте Projectile Prefab** (круг + скрипты)
6. **Добавьте NetworkManagerLobby** в сцену
7. **Назначьте префабы** в NetworkManager
8. **Запустите и протестируйте**

## 🎮 Ожидаемый результат

После выполнения всех этапов у вас будет:

- ✅ **Рабочая PvP игра** с сетевым мультиплеером
- ✅ **Физика пробития** и рикошетов
- ✅ **UI для подключения** и игровой HUD
- ✅ **Визуальные эффекты** попаданий
- ✅ **Система здоровья** и респауна
- ✅ **Готовая к тестированию** арена

---

**Время выполнения:** ~3 часа  
**Сложность:** Средняя  
**Готовность к тестированию:** 100% 