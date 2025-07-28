# 🔧 Пошаговая настройка префабов для сцены

## 📋 Что нужно настроить:

### 1. **Player Prefab** - игрок с компонентами
### 2. **Projectile Prefab** - снаряд с физикой
### 3. **NetworkManager** - сетевой менеджер
### 4. **Связи между компонентами** - ссылки на префабы

---

## 🎯 Шаг 1: Настройка Player Prefab

### 1.1 Откройте Player Prefab
- В Project Window найдите `Assets/Prefabs/Player.prefab`
- Дважды кликните для открытия

### 1.2 Проверьте компоненты
Убедитесь, что у Player есть все компоненты:
- ✅ **SpriteRenderer** (квадрат)
- ✅ **Rigidbody2D** (Gravity Scale = 0)
- ✅ **BoxCollider2D** (размер 1x1)
- ✅ **NetworkIdentity**
- ✅ **NetworkPlayerController**
- ✅ **ArmorSystem**
- ✅ **HealthSystem**
- ✅ **FirePoint** (дочерний объект)

### 1.3 Настройте NetworkPlayerController
В компоненте NetworkPlayerController назначьте:
- **Projectile Prefab:** `Assets/Prefabs/Projectile.prefab`
- **Fire Point:** дочерний объект `FirePoint`
- **Sprite Renderer:** компонент SpriteRenderer
- **Rigidbody:** компонент Rigidbody2D
- **Armor System:** компонент ArmorSystem
- **Health System:** компонент HealthSystem

### 1.4 Настройте ArmorSystem
В компоненте ArmorSystem установите:
- **Front Armor:** 50
- **Side Armor:** 30
- **Back Armor:** 20

### 1.5 Настройте HealthSystem
В компоненте HealthSystem установите:
- **Max Health:** 100
- **Current Health:** 100

---

## 🎯 Шаг 2: Настройка Projectile Prefab

### 2.1 Откройте Projectile Prefab
- В Project Window найдите `Assets/Prefabs/Projectile.prefab`
- Дважды кликните для открытия

### 2.2 Проверьте компоненты
Убедитесь, что у Projectile есть все компоненты:
- ✅ **SpriteRenderer** (круг, красный цвет)
- ✅ **Rigidbody2D** (Gravity Scale = 0)
- ✅ **CircleCollider2D** (радиус 0.5)
- ✅ **NetworkIdentity**
- ✅ **NetworkProjectile**

### 2.3 Настройте NetworkProjectile
В компоненте NetworkProjectile установите:
- **Speed:** 10
- **Damage:** 25
- **Penetration Power:** 51
- **Lifetime:** 5
- **Ricochet Threshold:** 70
- **Bounce Force:** 0.8
- **Hit Effect Prefab:** (пока оставьте пустым)
- **Ricochet Effect Prefab:** (пока оставьте пустым)

---

## 🎯 Шаг 3: Настройка сцены

### 3.1 Откройте TestScene
- В Project Window найдите `Assets/Scenes/TestScene.unity`
- Дважды кликните для открытия

### 3.2 Проверьте объекты в сцене
Убедитесь, что в сцене есть:
- ✅ **Main Camera** (орфографическая камера)
- ✅ **Directional Light** (освещение)
- ✅ **NetworkManager** (пустой объект)
- ✅ **SpawnPointA** (позиция -5, 0, 0)
- ✅ **SpawnPointB** (позиция 5, 0, 0)

### 3.3 Настройте NetworkManager
1. Выберите объект `NetworkManager` в сцене
2. В Inspector нажмите **Add Component**
3. Найдите и добавьте `NetworkManagerLobby`
4. В компоненте NetworkManagerLobby назначьте:
   - **Player Prefab:** `Assets/Prefabs/Player.prefab`
   - **Spawn Point A:** объект `SpawnPointA`
   - **Spawn Point B:** объект `SpawnPointB`
   - **Connection Menu:** (пока оставьте пустым)
   - **Game HUD:** (пока оставьте пустым)

---

## 🎯 Шаг 4: Проверка связей

### 4.1 Проверьте Player Prefab
1. Откройте `Assets/Prefabs/Player.prefab`
2. Выберите корневой объект Player
3. В NetworkPlayerController проверьте:
   - ✅ Projectile Prefab назначен
   - ✅ Fire Point назначен
   - ✅ Все компоненты связаны

### 4.2 Проверьте Projectile Prefab
1. Откройте `Assets/Prefabs/Projectile.prefab`
2. Выберите корневой объект Projectile
3. В NetworkProjectile проверьте:
   - ✅ Все параметры установлены
   - ✅ Rigidbody2D настроен
   - ✅ Collider2D настроен

### 4.3 Проверьте сцену
1. Вернитесь в TestScene
2. Выберите NetworkManager
3. В NetworkManagerLobby проверьте:
   - ✅ Player Prefab назначен
   - ✅ Spawn Point A назначен
   - ✅ Spawn Point B назначен

---

## 🎯 Шаг 5: Тестирование

### 5.1 Запустите игру
1. Нажмите **Play** в Unity
2. В консоли должно появиться: "Сервер запущен"
3. Игрок должен появиться в позиции SpawnPointA

### 5.2 Протестируйте управление
- **WASD** - движение (должно работать)
- **Мышь** - прицеливание (игрок должен поворачиваться)
- **ЛКМ** - выстрел (должны создаваться снаряды)

### 5.3 Проверьте консоль
В Console Window должны быть сообщения:
- "Сервер запущен"
- "Клиент подключился к серверу" (если есть второй игрок)

---

## 🔧 Возможные проблемы и решения:

### Проблема: "Missing Reference" в NetworkPlayerController
**Решение:**
1. Откройте Player Prefab
2. В NetworkPlayerController назначьте все ссылки
3. Убедитесь, что Projectile Prefab назначен

### Проблема: Снаряды не создаются
**Решение:**
1. Проверьте, что Projectile Prefab назначен в Player
2. Убедитесь, что FirePoint правильно позиционирован
3. Проверьте консоль на ошибки

### Проблема: Игрок не появляется
**Решение:**
1. Проверьте настройки NetworkManagerLobby
2. Убедитесь, что Player Prefab назначен
3. Проверьте позиции спавн-точек

### Проблема: Ошибки компиляции
**Решение:**
1. Убедитесь, что Mirror установлен
2. Проверьте, что все .meta файлы на месте
3. Перезапустите Unity

---

## ✅ Чек-лист готовности:

- [ ] Player Prefab открыт и настроен
- [ ] Projectile Prefab открыт и настроен
- [ ] TestScene открыта
- [ ] NetworkManagerLobby добавлен в сцену
- [ ] Все ссылки назначены в NetworkManagerLobby
- [ ] Все ссылки назначены в NetworkPlayerController
- [ ] Игра запускается без ошибок
- [ ] Игрок появляется в сцене
- [ ] Движение работает (WASD)
- [ ] Стрельба работает (ЛКМ)

---

**После выполнения всех шагов прототип будет готов к тестированию! 🚀** 