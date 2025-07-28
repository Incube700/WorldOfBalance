# 🎯 Простые шаги настройки в Unity

## 📋 Быстрая настройка (5 минут)

### 1. **Откройте Unity и проект**
- Запустите Unity Hub
- Откройте проект: `/Users/serg/Desktop/Projeсts/My project`
- Дождитесь компиляции

### 2. **Откройте тестовую сцену**
- В Project Window найдите `Assets/Scenes/TestScene.unity`
- Дважды кликните для открытия

### 3. **Настройте NetworkManager**
- В сцене выберите объект `NetworkManager`
- В Inspector нажмите **Add Component**
- Найдите и добавьте `NetworkManagerLobby`
- В NetworkManagerLobby назначьте:
  - **Player Prefab:** перетащите `Assets/Prefabs/Player.prefab`
  - **Spawn Point A:** перетащите объект `SpawnPointA`
  - **Spawn Point B:** перетащите объект `SpawnPointB`

### 4. **Настройте Player Prefab**
- Откройте `Assets/Prefabs/Player.prefab`
- Выберите корневой объект Player
- В NetworkPlayerController назначьте:
  - **Projectile Prefab:** перетащите `Assets/Prefabs/Projectile.prefab`
  - **Fire Point:** перетащите дочерний объект `FirePoint`
  - **Sprite Renderer:** перетащите компонент SpriteRenderer
  - **Rigidbody:** перетащите компонент Rigidbody2D
  - **Armor System:** перетащите компонент ArmorSystem
  - **Health System:** перетащите компонент HealthSystem

### 5. **Запустите тест**
- Нажмите **Play** в Unity
- Игрок должен появиться в сцене
- Протестируйте WASD и ЛКМ

---

## 🔧 Если что-то не работает:

### Unity не видит файлы:
1. Закройте Unity
2. Удалите папку `Library/`
3. Откройте проект заново

### Ошибки "Missing Reference":
1. Убедитесь, что все префабы назначены
2. Проверьте, что компоненты связаны
3. Перезапустите Unity

### Игрок не появляется:
1. Проверьте настройки NetworkManagerLobby
2. Убедитесь, что Player Prefab назначен
3. Проверьте консоль на ошибки

---

## ✅ Чек-лист:

- [ ] Unity открыт и проект загружен
- [ ] TestScene открыта
- [ ] NetworkManagerLobby добавлен
- [ ] Player Prefab назначен в NetworkManager
- [ ] Spawn Point A и B назначены
- [ ] Projectile Prefab назначен в Player
- [ ] Все компоненты связаны в Player
- [ ] Игра запускается без ошибок
- [ ] Игрок появляется в сцене
- [ ] Движение работает (WASD)
- [ ] Стрельба работает (ЛКМ)

---

**Готово! Прототип должен работать! 🚀** 