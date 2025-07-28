# 🎯 Инструкция по открытию проекта в Unity

## ✅ Это правильный проект!

**Путь к проекту:** `/Users/serg/Desktop/Projeсts/My project`

**Название проекта:** "Мир Баланса" (PvP игра с рикошетами)

## 🚀 Как правильно открыть проект:

### 1. **Откройте Unity Hub**

### 2. **Нажмите "Open" (Открыть)**

### 3. **Выберите папку проекта:**
```
/Users/serg/Desktop/Projeсts/My project
```

### 4. **Убедитесь, что выбрана правильная версия Unity:**
- Unity 2022.3 LTS или новее
- URP (Universal Render Pipeline)

## 📁 Структура проекта:

```
My project/
├── Assets/
│   ├── Scenes/
│   │   ├── PvP_Battle.unity ✅ (главная сцена)
│   │   └── SampleScene.unity
│   ├── Scripts/
│   │   ├── Player/NetworkPlayerController.cs ✅
│   │   ├── Projectile/NetworkProjectile.cs ✅
│   │   ├── Networking/NetworkManagerLobby.cs ✅
│   │   ├── Systems/ArmorSystem.cs ✅
│   │   ├── Systems/HealthSystem.cs ✅
│   │   ├── Systems/NetworkGameManager.cs ✅
│   │   ├── UI/ConnectionMenu.cs ✅
│   │   ├── UI/GameHUD.cs ✅
│   │   ├── Effects/HitEffect.cs ✅
│   │   ├── Effects/RicochetEffect.cs ✅
│   │   ├── Map/SpawnManager.cs ✅
│   │   └── CameraFollow.cs ✅
│   └── [другие папки с ассетами]
├── ProjectSettings/
├── Packages/
└── [документация]
```

## 🎮 Главная сцена:

**Откройте:** `Assets/Scenes/PvP_Battle.unity`

Эта сцена содержит:
- ✅ Сетевой менеджер
- ✅ Точки спавна игроков
- ✅ UI для подключения
- ✅ Настройки камеры

## 🔧 Если Unity не видит файлы:

### Вариант 1: Перезапуск Unity
1. Закройте Unity полностью
2. Откройте проект заново через Unity Hub
3. Дождитесь компиляции (2-5 минут)

### Вариант 2: Принудительное обновление
1. Закройте Unity
2. Удалите папку `Library/` (если она есть)
3. Откройте проект заново

### Вариант 3: Создание нового проекта
1. Создайте новый 2D URP проект
2. Скопируйте папку `Assets/` в новый проект
3. Скопируйте `ProjectSettings/` и `Packages/`

## 🚨 Важно!

- **НЕ открывайте отдельные файлы** - открывайте весь проект
- **Убедитесь, что выбрана правильная папка** - `My project`
- **Дождитесь полной компиляции** перед работой
- **Проверьте версию Unity** - должна быть совместима с URP

## 📋 Проверка правильности проекта:

1. ✅ Есть файл `PvP_Battle.unity`
2. ✅ Есть папка `Assets/Scripts/` с нашими скриптами
3. ✅ Есть файл `README.md` с описанием "Мир Баланса"
4. ✅ Есть файл `CURSOR_PROMPT.md` с техническим заданием

---

**Это правильный проект для игры "Мир Баланса"! 🎯** 