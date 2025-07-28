# 🎮 Мир Баланса (World of Balance)

**PvP аркада на физике пробития и рикошетов**

[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-blue.svg)](https://unity.com/)
[![Mirror](https://img.shields.io/badge/Mirror-Networking-orange.svg)](https://mirror-networking.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 🎯 О проекте

"Мир Баланса" — это минималистичная PvP-игра, где два игрока управляют квадратами с пушками. Основная механика основана на физике пробития брони и рикошетов в зависимости от угла попадания.

### 🧩 Ключевые особенности

- **Сетевой мультиплеер** через Mirror
- **Физика пробития** с углами попадания
- **Система рикошетов** от стен и брони
- **Минималистичная графика** для максимальной читаемости
- **Готовность к сборке** под Windows и Android

## 🧮 Физика пробития

### Формула расчёта эффективной брони:
```csharp
float angle = Vector3.Angle(hit.normal, -projectile.direction);
float effectiveArmor = baseArmor / Mathf.Cos(angle * Mathf.Deg2Rad);
```

### Логика пробития:
- **Прямой угол** (90°) — обычная броня
- **Острый угол** (< 90°) — броня эффективнее
- **Угол < 30°** — всегда рикошет
- **penetrationPower >= effectiveArmor** → пробитие
- **penetrationPower < effectiveArmor** → рикошет

## 🎮 Управление

### ПК:
- **WASD** — движение
- **Мышь** — прицеливание
- **ЛКМ** — выстрел

### Мобильные устройства (планируется):
- **Джойстик** — движение
- **Касание экрана** — прицеливание
- **Кнопка FIRE** — выстрел

## 🏗️ Архитектура проекта

```
/Assets/Scripts/
├── Player/
│   └── NetworkPlayerController.cs
├── Projectile/
│   └── NetworkProjectile.cs
├── Networking/
│   └── NetworkManagerLobby.cs
├── Systems/
│   ├── ArmorSystem.cs
│   ├── HealthSystem.cs
│   └── NetworkGameManager.cs
├── Effects/
│   ├── HitEffect.cs
│   └── RicochetEffect.cs
├── UI/
│   ├── ConnectionMenu.cs
│   └── GameHUD.cs
└── Map/
    └── SpawnManager.cs
```

## 🚀 Быстрый старт

### Требования
- Unity 2022.3.0f1 или новее
- Mirror Networking (устанавливается автоматически)

### Установка
1. **Клонируйте репозиторий**:
   ```bash
   git clone https://github.com/Incube700/WorldOfBalance.git
   ```

2. **Откройте проект** в Unity

3. **Следуйте инструкциям** в `QUICK_SETUP.md`

4. **Настройте сцену** по `SCENE_SETUP.md`

### Тестирование
1. **Запустите игру** в Unity
2. **Создайте хост** через UI
3. **Подключитесь** как клиент
4. **Протестируйте** стрельбу и рикошеты

## 📚 Документация

- **[QUICK_SETUP.md](QUICK_SETUP.md)** — быстрая настройка за 5 минут
- **[SCENE_SETUP.md](SCENE_SETUP.md)** — пошаговая настройка сцены
- **[CURSOR_PROMPT.md](CURSOR_PROMPT.md)** — промт для Cursor AI
- **[GITHUB_REPOSITORY_SETUP.md](GITHUB_REPOSITORY_SETUP.md)** — настройка GitHub

## 🎯 Механики игры

### Система брони
- **4 стороны** брони: front, back, left, right
- **Разные значения** для каждой стороны
- **Эффективная броня** зависит от угла попадания

### Система снарядов
- **Параметр пробития** (penetrationPower)
- **Физика рикошета** от стен
- **Визуальные эффекты** попаданий

### Сетевая синхронизация
- **Mirror Networking** для мультиплеера
- **Синхронизация** движения, стрельбы, урона
- **Автоматический респаун** после смерти

## 🔧 Разработка

### Добавление новых функций
1. **Создайте ветку** для новой функции
2. **Следуйте архитектуре** проекта
3. **Протестируйте** в сети
4. **Создайте Pull Request**

### Отладка
- **Включите Gizmos** в Scene view
- **Проверьте Network Info** в Game view
- **Используйте OnGUI()** для отображения информации

## 📱 Сборка

### Windows (Standalone)
1. File → Build Settings
2. Выберите Windows
3. Нажмите Build

### Android
1. File → Build Settings
2. Выберите Android
3. Настройте Player Settings
4. Нажмите Build

## 🤝 Вклад в проект

1. **Fork** репозитория
2. **Создайте ветку** для функции (`git checkout -b feature/amazing-feature`)
3. **Сделайте коммит** (`git commit -m 'Add amazing feature'`)
4. **Отправьте в ветку** (`git push origin feature/amazing-feature`)
5. **Создайте Pull Request**

## 📄 Лицензия

Этот проект распространяется под лицензией MIT. См. файл [LICENSE](LICENSE) для подробностей.

## 🙏 Благодарности

- **Mirror Networking** — за отличную сетевую библиотеку
- **Unity Technologies** — за мощный движок
- **Сообщество Unity** — за поддержку и идеи

## 📞 Контакты

- **GitHub**: [Incube700](https://github.com/Incube700)
- **Проект**: [WorldOfBalance](https://github.com/Incube700/WorldOfBalance)

---

**Создано с ❤️ для сообщества Unity** 