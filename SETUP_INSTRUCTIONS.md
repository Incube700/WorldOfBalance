# 🛠️ Инструкция по настройке сцены

## ❌ Проблема
При запуске игры вылетает из-за сетевых компонентов без NetworkManager.

## ✅ Решение

### 1. Добавить NetworkDisabler в сцену
1. Создайте пустой GameObject в сцене
2. Назовите его "NetworkDisabler"
3. Добавьте компонент `NetworkDisabler.cs`

### 2. Создать UI для главного меню
1. Создайте Canvas в сцене
2. Добавьте Panel для главного меню
3. Добавьте кнопки "Старт" и "Выход"
4. Добавьте компонент `MainMenu.cs` на Canvas

### 3. Настроить GameManager
1. Создайте пустой GameObject "GameManager"
2. Добавьте компонент `GameManager.cs`
3. Настройте ссылки на UI элементы

### 4. Отключить сетевые компоненты вручную
В Inspector для каждого объекта с NetworkIdentity:
1. Найдите компонент NetworkIdentity
2. Снимите галочку "Enabled"

### 5. Настроить Player и Enemy
Убедитесь, что у Player и Enemy есть:
- ✅ TankController (для Player)
- ✅ EnemyAIController (для Enemy)
- ✅ HealthSystem
- ✅ ArmorSystem
- ✅ ProjectileSpawner
- ❌ НЕ должно быть NetworkIdentity (отключить)

### 6. Настроить Projectile
Убедитесь, что у Projectile есть:
- ✅ Projectile.cs
- ✅ Rigidbody2D
- ✅ CircleCollider2D
- ❌ НЕ должно быть NetworkIdentity (отключить)

## 🎮 Как запустить
1. Нажмите Play в Unity
2. Появится главное меню
3. Нажмите "Старт" для начала игры
4. Используйте WASD для движения, ЛКМ для стрельбы

## 🔧 Дополнительные настройки
- ESC - пауза
- R - перезапуск игры
- Враги автоматически преследуют игрока 