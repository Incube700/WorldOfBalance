# Overlord:Rise and Slice

## Unity 2D Game - Player Controller

### Решение проблем с Input System

#### Проблема: Персонаж не двигается

##### Вариант 1: Использование Input Action Asset
1. **Откройте Unity** и сцену `Overlord.unity`
2. **Выберите объект игрока** в иерархии
3. **В компоненте PlayerController**:
   - Перетащите `InputSystem_Actions.inputactions` в поле `Input Actions`
   - Убедитесь, что скрипт `PlayerController` прикреплен к игроку
4. **Проверьте консоль** - должны появиться сообщения:
   - "Move action found successfully!"
   - "Dash action created successfully!"

##### Вариант 2: Простое управление (рекомендуется)
1. **Удалите компонент PlayerController** с объекта игрока
2. **Добавьте компонент SimplePlayerController** к объекту игрока
3. **Настройте параметры** в инспекторе:
   - Move Speed: 5
   - Dash Speed: 15
   - Dash Duration: 0.2
   - Dash Cooldown: 0.5

### Управление
- **WASD** - движение персонажа
- **Space** - даш (работает только при движении)

### Контроль FPS

#### Встроенный контроль FPS в PlayerController
PlayerController теперь включает базовый контроль FPS:
- **Limit FPS**: Включить/выключить ограничение FPS
- **Target FPS**: Целевой FPS (по умолчанию 60)
- **Show FPS**: Показывать FPS в консоли

#### Отдельный FPSController (рекомендуется)
Для более продвинутого контроля FPS добавьте компонент `FPSController`:

**Управление клавишами:**
- **F1** - включить/выключить ограничение FPS
- **F2** - включить/выключить отображение FPS в консоли
- **F3** - включить/выключить отображение FPS в игре
- **F4** - установить FPS 30
- **F5** - установить FPS 60
- **F6** - установить FPS 120

**Настройки в инспекторе:**
- **Limit FPS**: Ограничить FPS
- **Target FPS**: Целевой FPS
- **Show FPS**: Показывать в консоли
- **Show FPS In Game**: Показывать в игре
- **FPS Text Color**: Цвет текста FPS
- **Font Size**: Размер шрифта
- **FPS Display Position**: Позиция отображения

### Отладка
1. **Добавьте скрипт InputSystemChecker** к любому объекту в сцене для проверки Input System
2. **Проверьте консоль Unity** - должны появиться сообщения о подключенных устройствах
3. **При нажатии клавиш** в консоли должны появляться сообщения

### Возможные проблемы и решения

#### 1. "Move action not found!"
**Решение**: Убедитесь, что `InputSystem_Actions.inputactions` назначен в поле `Input Actions`

#### 2. Input System не работает
**Решение**: 
1. Проверьте, что Input System установлен в Package Manager
2. Window → Package Manager → Input System
3. Убедитесь, что пакет установлен

#### 3. Клавиши не реагируют
**Решение**:
1. Используйте `SimplePlayerController` вместо `PlayerController`
2. Этот скрипт работает напрямую с Input System без Input Action Asset

#### 4. Даш не работает
**Решение**: 
1. Убедитесь, что вы двигаетесь (нажимаете WASD) перед нажатием Space
2. Проверьте параметры Dash Cooldown и Dash Duration

#### 5. Низкий FPS
**Решение**:
1. Добавьте `FPSController` для контроля производительности
2. Используйте F1-F6 для быстрого управления FPS
3. Проверьте настройки качества в Project Settings

### Настройка в Unity Editor
1. Откройте сцену `Overlord.unity`
2. Выберите объект игрока
3. **Для простого управления**: Добавьте компонент `SimplePlayerController`
4. **Для Input Action Asset**: Добавьте компонент `PlayerController` и назначьте `InputSystem_Actions.inputactions`
5. **Для контроля FPS**: Добавьте компонент `FPSController` к любому объекту
6. Настройте параметры по вашему усмотрению

### Проверка работы
1. Запустите игру
2. Нажмите WASD - персонаж должен двигаться
3. Двигаясь, нажмите Space - должен произойти даш
4. Используйте F1-F6 для управления FPS
5. Проверьте консоль Unity для отладочных сообщений 