# 🚀 Создание GitHub репозитория для "Мир Баланса"

## 📋 Пошаговая инструкция

### 1. Создание репозитория на GitHub

1. **Перейдите на [GitHub.com](https://github.com)**
2. **Нажмите "New repository"** (зеленая кнопка)
3. **Заполните форму**:
   - **Repository name**: `WorldOfBalance`
   - **Description**: `PvP аркада на физике пробития и рикошетов`
   - **Visibility**: Public (или Private по вашему выбору)
   - **НЕ ставьте галочки** на "Add a README file", "Add .gitignore", "Choose a license"
4. **Нажмите "Create repository"**

### 2. Настройка аутентификации

#### Вариант A: SSH ключ (рекомендуется)

1. **Проверьте SSH ключи**:
   ```bash
   ls -la ~/.ssh
   ```

2. **Если ключей нет, создайте новый**:
   ```bash
   ssh-keygen -t ed25519 -C "your_email@example.com"
   ```

3. **Добавьте ключ в SSH агент**:
   ```bash
   ssh-add ~/.ssh/id_ed25519
   ```

4. **Скопируйте публичный ключ**:
   ```bash
   cat ~/.ssh/id_ed25519.pub
   ```

5. **Добавьте ключ в GitHub**:
   - Перейдите в Settings → SSH and GPG keys
   - Нажмите "New SSH key"
   - Вставьте скопированный ключ

#### Вариант B: Personal Access Token

1. **Создайте токен**:
   - Перейдите в Settings → Developer settings → Personal access tokens
   - Нажмите "Generate new token"
   - Выберите "Tokens (classic)"
   - Назовите токен "WorldOfBalance"
   - Выберите scopes: `repo`, `workflow`
   - Скопируйте токен

### 3. Настройка локального репозитория

#### Если используете SSH:

```bash
# Удалите старый remote
git remote remove worldofbalance

# Добавьте новый с SSH
git remote add worldofbalance git@github.com:Incube700/WorldOfBalance.git

# Отправьте код
git push -u worldofbalance main
```

#### Если используете токен:

```bash
# При запросе пароля используйте токен вместо пароля
git push -u worldofbalance main
# Username: Incube700
# Password: [ваш токен]
```

### 4. Настройка репозитория

После создания репозитория настройте:

#### README.md
Скопируйте содержимое из нашего README.md в репозиторий

#### .gitignore
Убедитесь, что у вас есть правильный .gitignore для Unity

#### Описание
Добавьте описание проекта в настройках репозитория

### 5. Настройка GitHub Pages (опционально)

1. **Перейдите в Settings → Pages**
2. **Source**: Deploy from a branch
3. **Branch**: main
4. **Folder**: / (root)
5. **Нажмите Save**

### 6. Настройка Issues и Projects

#### Создайте шаблоны Issues:

1. **Bug Report**:
   ```markdown
   ## 🐛 Описание бага
   
   ## 🔄 Шаги для воспроизведения
   
   ## 💻 Ожидаемое поведение
   
   ## ❌ Фактическое поведение
   
   ## 📱 Система
   - Unity: 2022.3.0f1
   - Mirror: последняя версия
   ```

2. **Feature Request**:
   ```markdown
   ## 🎯 Описание функции
   
   ## 💡 Мотивация
   
   ## 🔧 Предлагаемое решение
   ```

#### Создайте Project:

1. **Перейдите в Projects**
2. **Создайте новый проект**
3. **Добавьте колонки**:
   - Backlog
   - To Do
   - In Progress
   - Review
   - Done

### 7. Настройка Actions (опционально)

Создайте `.github/workflows/unity.yml`:

```yaml
name: Unity CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Cache Library
      uses: actions/cache@v3
      with:
        path: Library
        key: Library-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
        restore-keys: |
          Library-
    
    - name: Build Windows
      run: |
        unity-builder build \
          --targetPlatform Windows \
          --buildName "WorldOfBalance" \
          --buildPath "builds/Windows"
    
    - name: Upload Build
      uses: actions/upload-artifact@v3
      with:
        name: Windows Build
        path: builds/Windows
```

### 8. Настройка Wiki

1. **Перейдите в Wiki**
2. **Создайте главную страницу**:
   ```markdown
   # Мир Баланса - Wiki
   
   Добро пожаловать в Wiki проекта "Мир Баланса"!
   
   ## 📖 Содержание
   - [Установка и настройка](Установка-и-настройка)
   - [Архитектура проекта](Архитектура-проекта)
   - [Физика пробития](Физика-пробития)
   - [Сетевой мультиплеер](Сетевой-мультиплеер)
   - [Отладка](Отладка)
   - [FAQ](FAQ)
   ```

### 9. Настройка Releases

1. **Перейдите в Releases**
2. **Создайте первый релиз**:
   - Tag: `v1.0.0`
   - Title: `Initial Release - PvP Architecture`
   - Description: Описание архитектуры игры

### 10. Настройка Labels

Создайте метки:
- `bug` - Баги
- `enhancement` - Улучшения
- `feature` - Новые функции
- `documentation` - Документация
- `high` - Высокий приоритет
- `medium` - Средний приоритет
- `low` - Низкий приоритет

## 🎯 Результат

После выполнения всех шагов у вас будет:

- ✅ **Полноценный GitHub репозиторий** для игры "Мир Баланса"
- ✅ **Настроенная документация** с README и Wiki
- ✅ **Система Issues** с шаблонами
- ✅ **Project для управления задачами**
- ✅ **CI/CD pipeline** для автоматической сборки
- ✅ **Готовность к совместной разработке**

## 🔧 Команды для быстрой настройки

```bash
# После создания репозитория на GitHub
git remote add origin git@github.com:Incube700/WorldOfBalance.git
git push -u origin main

# Создание тега для релиза
git tag v1.0.0
git push origin v1.0.0
```

---

**Теперь ваш проект готов для совместной разработки! 🚀** 