# 🌐 Настройка NetworkManager

## ✅ Решение проблемы с вылетами

### **Автоматическое решение:**
1. Добавьте **NetworkDisabler** в сцену
2. Скрипт автоматически создаст **SimpleNetworkManager**
3. Все сетевые компоненты будут отключены

### **Ручное решение:**

#### 1. Добавить NetworkManager в сцену:
1. Создайте пустой GameObject **"NetworkManager"**
2. Добавьте компонент **"SimpleNetworkManager"**
3. Настройте параметры:
   - **Player Prefab:** оставьте пустым
   - **Auto Start Server Build:** false
   - **Auto Start Client Build:** false

#### 2. Отключить NetworkIdentity:
В Inspector для каждого объекта:
- **Player** → NetworkIdentity → снять галочку "Enabled"
- **Enemy** → NetworkIdentity → снять галочку "Enabled"  
- **Projectile** → NetworkIdentity → снять галочку "Enabled"

#### 3. Добавить NetworkDisabler:
1. Создайте пустой GameObject **"NetworkDisabler"**
2. Добавьте компонент **"NetworkDisabler"**
3. Настройте:
   - **Disable Network Components:** ✅
   - **Auto Start Network:** ✅

## 🎮 Результат:
- ✅ Игра не будет вылетать
- ✅ NetworkManager будет создан автоматически
- ✅ Все сетевые компоненты отключены
- ✅ Игра работает в локальном режиме

## 🔧 Дополнительно:
Если хотите полностью убрать сеть:
1. Удалите все `using Mirror;` из скриптов
2. Удалите все `NetworkBehaviour` наследования
3. Удалите все `[Command]` и `[ClientRpc]` атрибуты 