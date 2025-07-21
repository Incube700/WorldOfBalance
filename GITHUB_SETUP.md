# Подключение проекта к GitHub

## Шаг 1: Создание репозитория на GitHub

1. Перейдите на [GitHub.com](https://github.com)
2. Нажмите кнопку "New" или "+" в правом верхнем углу
3. Выберите "New repository"
4. Заполните форму:
   - **Repository name**: `unity-2d-game`
   - **Description**: `Unity 2D game with player controller and FPS control`
   - **Visibility**: Public или Private (по вашему выбору)
   - **НЕ** ставьте галочки на "Add a README file", "Add .gitignore", "Choose a license"
5. Нажмите "Create repository"

## Шаг 2: Подключение локального репозитория к GitHub

После создания репозитория на GitHub, выполните следующие команды в терминале:

```bash
# Добавить удаленный репозиторий (замените YOUR_USERNAME на ваше имя пользователя)
git remote add origin https://github.com/YOUR_USERNAME/unity-2d-game.git

# Переименовать ветку в main (если нужно)
git branch -M main

# Отправить код на GitHub
git push -u origin main
```

## Шаг 3: Проверка подключения

После выполнения команд проверьте:
1. Перейдите на страницу вашего репозитория на GitHub
2. Убедитесь, что все файлы загружены
3. Проверьте, что README.md отображается правильно

## Структура проекта

Проект содержит:
- **PlayerController.cs** - основной скрипт управления персонажем
- **FPSController.cs** - контроль производительности
- **InputSystem_Actions.inputactions** - настройки ввода
- **README.md** - документация проекта
- **.gitignore** - исключения для Git

## Управление проектом

### Основные команды Git:
```bash
# Проверить статус
git status

# Добавить изменения
git add .

# Создать коммит
git commit -m "Описание изменений"

# Отправить изменения на GitHub
git push

# Получить изменения с GitHub
git pull
```

### Управление в игре:
- **WASD** - движение персонажа
- **Space** - даш
- **F1-F6** - управление FPS (если добавлен FPSController)

## Полезные ссылки

- [Unity Documentation](https://docs.unity3d.com/)
- [GitHub Guides](https://guides.github.com/)
- [Git Cheat Sheet](https://education.github.com/git-cheat-sheet-education.pdf) 