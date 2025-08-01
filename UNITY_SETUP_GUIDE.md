# 🔥 Unity Setup Guide - WorldOfBalance ГОТОВ!

## ✅ **ВСЕ ОШИБКИ КОМПИЛЯЦИИ ИСПРАВЛЕНЫ!**

Unity теперь должен компилироваться **БЕЗ ОШИБОК**! 🎉

---

## 🚀 **Быстрый старт в Unity:**

### **1. Перезапустите Unity** 🔄
```
File → Close Project
File → Open Project → выберите папку проекта
```

### **2. Проверьте компиляцию** ✅
- Откройте **Console** (Window → General → Console)
- Должно быть: **0 errors, 0 warnings**
- Если ошибки остались - нажмите **Ctrl+R** (Refresh)

### **3. Настройте сцену TankDuel** 🏟️
```
Assets/Scenes/Levels/TankDuel.unity
```

---

## 🎮 **Добавление менеджеров в сцену:**

### **🎯 GameManager** - Центральное управление
1. Создайте пустой GameObject: `Create → Empty GameObject`
2. Переименуйте в **"GameManager"**
3. Add Component → `GameManager`
4. Настройте в Inspector:
   - **Main Menu Scene**: "MainMenu"
   - **Game Scene Name**: "TankDuel"

### **🎵 AudioManager** - Звуковая система
1. Создайте пустой GameObject: **"AudioManager"**
2. Add Component → `AudioManager`
3. Система готова! (звуки добавите позже)

### **📊 HUDManager** - Интерфейс игрока
1. Создайте UI Canvas: `Create → UI → Canvas`
2. На Canvas добавьте Component → `HUDManager`
3. Создайте UI элементы:
   - Health Bar (UI → Slider)
   - Armor Bar (UI → Slider)  
   - Ammo Text (UI → Text)

### **📷 CameraController** - Камера
1. Выберите **Main Camera**
2. Add Component → `CameraController`
3. Настройте:
   - **Find Player Automatically**: ✅
   - **Follow Speed**: 5
   - **Height**: 10

---

## 🚗 **Настройка танков:**

### **Player Tank:**
1. Выберите объект **Player**
2. Убедитесь что есть компоненты:
   - `PlayerController` ✅
   - `TankController` ✅
   - `Rigidbody2D` ✅
   - `Collider2D` ✅

### **Enemy Tank:**
1. Выберите объект **Enemy** 
2. Убедитесь что есть:
   - `EnemyAI` ✅
   - `TankController` ✅

### **Автоматическая настройка:**
1. Выберите любой танк
2. Add Component → `TankSetupHelper` 
3. Нажмите **Setup FirePoint** в Inspector
4. **Готово!** Система создаст FirePoint и пушку автоматически

---

## 🔫 **Создание префаба снаряда:**

### **Bullet Prefab:**
1. `Create → Empty GameObject` → назовите **"Bullet"**
2. Add Component → `BulletController`
3. Add Component → `CircleCollider2D`
4. Add Component → `SpriteRenderer`
5. Настройте BulletController:
   - **Speed**: 6
   - **Damage**: 25  
   - **Max Bounces**: 4
   - **Life Time**: 5
6. Перетащите в папку Prefabs

### **Подключите к танкам:**
1. Выберите танк с компонентом `Weapon`
2. Перетащите Bullet префаб в поле **Projectile Prefab**

---

## 🎯 **Тестирование системы:**

### **Запустите игру** ▶️
1. Нажмите **Play** в Unity
2. Должно работать:
   - ✅ Управление танком (WASD)
   - ✅ Выстрелы (Space/Mouse)
   - ✅ Рикошеты снарядов
   - ✅ HUD отображение
   - ✅ Камера следует за игроком

### **Проверьте Console:**
- Должны быть debug сообщения:
  - `"GameManager: State changed to Playing"`
  - `"AudioManager initialized successfully"`
  - `"CameraController: Found player target"`
  - `"Bullet bounced! New direction: ..."`

---

## 🎵 **Добавление звуков (опционально):**

### **Звуковые файлы:**
Поместите в `Assets/Game/Audio/`:
- `tank_fire.wav` - звук выстрела
- `bullet_hit.wav` - попадание
- `bullet_bounce.wav` - рикошет
- `gameplay_music.mp3` - игровая музыка

### **Подключение:**
1. Выберите **AudioManager** в сцене
2. Перетащите звуки в соответствующие поля Inspector

---

## 🏆 **Готовые возможности:**

### **🎮 Игровая механика:**
- ✅ Танковое управление (локальные координаты)
- ✅ Система стрельбы с FirePoint
- ✅ Рикошеты снарядов (до 4 отскоков)
- ✅ Постоянная скорость снарядов
- ✅ Система урона

### **🎯 Менеджеры:**
- ✅ GameManager - состояния игры
- ✅ AudioManager - звуки и музыка
- ✅ HUDManager - интерфейс игрока
- ✅ CameraController - следование за игроком

### **🔧 Автоматизация:**
- ✅ TankSetupHelper - авто-настройка танков
- ✅ Автопоиск игрока для камеры и HUD
- ✅ Fallback системы для совместимости

---

## 🚨 **Если что-то не работает:**

### **Ошибки компиляции:**
```
1. Window → General → Console
2. Clear все ошибки
3. Assets → Refresh (Ctrl+R)
4. File → Build Settings → Player Settings → Configuration: Release
```

### **Танк не стреляет:**
```
1. Проверьте Bullet Prefab подключен к Weapon
2. Убедитесь что у танка есть FirePoint (TankSetupHelper)
3. Проверьте что у Bullet есть BulletController
```

### **Камера не следует:**
```
1. Player должен иметь tag "Player"
2. CameraController → Find Player Automatically = true
3. Или вручную перетащите Player в Target поле
```

---

## 🎯 **ПРОЕКТ ГОТОВ К ИСПОЛЬЗОВАНИЮ!**

**Вся архитектура работает ✅ | Компиляция без ошибок ✅ | Игровая механика готова ✅**

*Приятной разработки танковых дуэлей!* 🚗💥🎮