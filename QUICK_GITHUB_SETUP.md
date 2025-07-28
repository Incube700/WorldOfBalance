# 🚀 Быстрое создание GitHub репозитория для "Мир Баланса"

## 📋 Быстрые шаги (5 минут)

### 1. Создайте репозиторий на GitHub
1. Перейдите на [GitHub.com](https://github.com)
2. Нажмите "New repository"
3. Заполните:
   - **Name**: `WorldOfBalance`
   - **Description**: `PvP аркада на физике пробития и рикошетов`
   - **Public** или **Private**
4. НЕ ставьте галочки на README, .gitignore, license
5. Нажмите "Create repository"

### 2. Настройте SSH (если еще не настроен)
```bash
# Проверьте SSH ключи
ls -la ~/.ssh

# Если нет ключей, создайте
ssh-keygen -t ed25519 -C "your_email@example.com"

# Добавьте в SSH агент
ssh-add ~/.ssh/id_ed25519

# Скопируйте публичный ключ
cat ~/.ssh/id_ed25519.pub
```

3. Добавьте ключ в GitHub:
   - Settings → SSH and GPG keys → New SSH key
   - Вставьте скопированный ключ

### 3. Отправьте код
```bash
# Удалите старый remote
git remote remove worldofbalance

# Добавьте новый с SSH
git remote add origin git@github.com:Incube700/WorldOfBalance.git

# Отправьте код
git push -u origin main
```

### 4. Настройте репозиторий
1. **Добавьте описание** в настройках репозитория
2. **Создайте Issues** с шаблонами (см. GITHUB_REPOSITORY_SETUP.md)
3. **Создайте Project** для управления задачами
4. **Настройте Wiki** (опционально)

## 🎯 Готово!

Теперь у вас есть полноценный GitHub репозиторий для игры "Мир Баланса" с:
- ✅ Полной архитектурой PvP-игры
- ✅ Сетевым мультиплеером
- ✅ Физикой пробития и рикошетов
- ✅ Готовностью к совместной разработке

## 🔗 Ссылки

- **Подробная инструкция**: `GITHUB_REPOSITORY_SETUP.md`
- **Быстрая настройка проекта**: `QUICK_SETUP.md`
- **Настройка сцены**: `SCENE_SETUP.md`
- **Промт для Cursor**: `CURSOR_PROMPT.md`

---

**Ваш проект готов к разработке! 🎮** 