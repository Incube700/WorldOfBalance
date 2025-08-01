# 🎮 ПОЛНАЯ НАСТРОЙКА WorldOfBalance В UNITY

## 🚀 **БЫСТРАЯ УСТАНОВКА - 5 МИНУТ!**

---

## **ШАГ 1: Автоматическая настройка** ⚡

### **A. Создайте Setup GameObject:**
1. В Hierarchy: **Create → Empty GameObject**
2. Назовите: **"SetupHelper"**
3. **Add Component → SetupHelper**

### **B. Запустите автоматическую настройку:**
1. В Inspector у SetupHelper найдите кнопку **"Setup Complete Game"**
2. **НАЖМИТЕ ЭТУ КНОПКУ!** ⬅️ **ВАЖНО!**
3. Посмотрите в Console - должны появиться ✅ сообщения об успешной настройке

### **Что произойдет автоматически:**
- ✅ Создается **SimpleGameManager**
- ✅ Создается **UI Canvas** с HUD
- ✅ Создаются панели меню (MainMenu, GameOver, Victory)
- ✅ Настраивается **Main Camera** с CameraController
- ✅ Создается арена со стенами
- ✅ Настраиваются танки (если найдены)

---

## **ШАГ 2: Создайте танки** 🚗

### **A. Создайте Player Tank:**
1. **Create → Empty GameObject** → назовите **"Player"**
2. **Add Component → PlayerController**
3. **Add Component → TankController** 
4. **Add Component → Rigidbody2D**
5. **Add Component → CircleCollider2D**
6. **Add Component → SpriteRenderer**
7. В SpriteRenderer: **Color → Blue (синий)**
8. **Tag → Player**
9. **Position: (-5, 0, 0)**

### **B. Создайте Enemy Tank:**
1. **Create → Empty GameObject** → назовите **"Enemy"**
2. **Add Component → EnemyAI**
3. **Add Component → TankController**
4. **Add Component → Rigidbody2D**  
5. **Add Component → CircleCollider2D**
6. **Add Component → SpriteRenderer**
7. В SpriteRenderer: **Color → Red (красный)**
8. **Tag → Enemy**
9. **Position: (5, 0, 0)**

---

## **ШАГ 3: Настройте UI кнопки** 🔘

### **Найдите в Hierarchy:**
- **UI Canvas → MainMenuPanel → Button_PLAY**
- **UI Canvas → MainMenuPanel → Button_QUIT**
- **UI Canvas → GameOverPanel → Button_RESTART**
- **UI Canvas → GameOverPanel → Button_MENU**
- **UI Canvas → VictoryPanel → Button_RESTART**
- **UI Canvas → VictoryPanel → Button_MENU**

### **Подключите каждую кнопку:**
1. **Выберите кнопку** в Hierarchy
2. В Inspector найдите **Button → On Click ()**
3. **Нажмите "+"** 
4. **Перетащите SimpleGameManager** из Hierarchy в поле Object
5. **Выберите функцию** из выпадающего меню:
   - **PLAY** → `SimpleGameManager → OnStartGameButton()`
   - **QUIT** → `SimpleGameManager → OnQuitButton()`
   - **RESTART** → `SimpleGameManager → OnRestartButton()`
   - **MENU** → `SimpleGameManager → OnMainMenuButton()`

---

## **ШАГ 4: Подключите HUD** 📊

### **Найдите SimpleHUD:**
1. Выберите **UI Canvas** в Hierarchy
2. В Inspector найдите **SimpleHUD** компонент

### **Подключите UI элементы:**
- **Health Bar** → перетащите `UI Canvas/GameplayPanel/HealthBar`
- **Health Text** → создайте Text рядом с Health Bar
- **Game Time Text** → перетащите `UI Canvas/GameplayPanel/GameTimeText`
- **Status Text** → перетащите `UI Canvas/GameplayPanel/StatusText`

---

## **ШАГ 5: Интеграция с танками** 🔗

### **В TankController или HealthSystem добавьте:**
```csharp
// Когда танк умирает:
if (health <= 0)
{
    if (SimpleGameManager.Instance != null)
    {
        SimpleGameManager.Instance.OnTankDestroyed(gameObject.name);
    }
}
```

### **Или используйте готовый метод в TankController:**
1. Откройте **TankController.cs**
2. В методе где танк умирает, добавьте вызов:
```csharp
SimpleGameManager.Instance.OnTankDestroyed(gameObject.name);
```

---

## **ШАГ 6: Настройте GameManager** 🎯

### **Выберите SimpleGameManager в Hierarchy**
### **В Inspector подключите панели:**
- **Main Menu Panel** → `UI Canvas/MainMenuPanel`
- **Gameplay Panel** → `UI Canvas/GameplayPanel`  
- **Game Over Panel** → `UI Canvas/GameOverPanel`
- **Win Panel** → `UI Canvas/VictoryPanel`
- **Game Scene Name** → `"TankDuel"` (или имя вашей сцены)

---

## **ШАГ 7: Тестирование** ✅

### **A. Нажмите Play в Unity**
### **B. Проверьте:**
- ✅ Видны синий и красный танки
- ✅ Видны стены арены
- ✅ Показывается главное меню с кнопкой PLAY
- ✅ Камера правильно позиционирована
- ✅ В Console нет ошибок!

### **C. Тестируйте игровой цикл:**
1. **Нажмите PLAY** → должен начаться геймплей
2. **"Убейте" танк** → должен показаться экран Game Over/Victory
3. **Нажмите RESTART** → игра перезапускается
4. **Нажмите MENU** → возврат в главное меню

---

## **ШАГ 8: Дополнительные настройки** ⚙️

### **A. Настройте Camera:**
- **Main Camera → CameraController**
- **Target** → перетащите Player танк
- **Follow Speed: 5**
- **Height: 10**
- **Orthographic Size: 8**

### **B. Настройте танки:**
- **PlayerController** → настройте скорость движения
- **TankController** → настройте здоровье и броню
- **EnemyAI** → настройте поведение врага

### **C. Создайте Bullet префаб:**
1. **Create → Empty GameObject** → "Bullet"
2. **Add Component → BulletController**
3. **Add Component → CircleCollider2D**
4. **Add Component → SpriteRenderer** (желтый цвет)
5. **Drag & Drop в папку Prefabs**

---

## **🎯 РЕЗУЛЬТАТ:**

### **✅ У ВАС ДОЛЖНО РАБОТАТЬ:**
- **Стартовое меню** с кнопкой PLAY
- **Два танка** (синий игрок, красный враг) 
- **Арена** со стенами
- **HUD** с здоровьем и временем
- **Игровой цикл:** Menu → Battle → Game Over → Menu
- **Камера** следует за игроком
- **0 ошибок** в Console

---

## **🚨 Если что-то не работает:**

### **Проверьте Console:** 
- **0 ошибок компиляции** ✅
- Сообщения "✅ Created..." от SetupHelper

### **Проверьте подключения:**
- Все кнопки подключены к SimpleGameManager
- UI элементы подключены к SimpleHUD
- SimpleGameManager имеет ссылки на все панели

### **Перезапуск Unity:**
```
File → Close Project → File → Open Project
```

---

## **🎮 ГОТОВО! ИГРА РАБОТАЕТ!**

**Теперь у вас есть полный игровой цикл WorldOfBalance!**

### **Следующие шаги:**
1. ✅ **Игровой цикл работает** 
2. 🔫 Добавить стрельбу танков
3. 💥 Настроить рикошеты снарядов  
4. 🎵 Добавить звуки (в последнюю очередь)
5. 🎨 Улучшить графику

## 🚗💥 **Счастливого тестирования танковых дуэлей!** 🎯