# 🛠 MirrorRepairUtility - Автоматическое исправление Mirror

## 📋 Что делает утилита:

### 🔧 Функции:
- **Проверяет** наличие Mirror в manifest.json
- **Добавляет** Mirror из GitHub, если его нет
- **Предупреждает** о конфликтных локальных папках
- **Автоматизирует** процесс исправления

### 🎯 Когда использовать:
- Unity не может найти Mirror
- Ошибки "The type or namespace name 'Mirror' could not be found"
- Проблемы с установкой пакетов
- Нужно быстро исправить Mirror

## 🚀 Как использовать:

### 1. **Откройте Unity**
```
Unity Hub → Open → /Users/serg/Desktop/Projeсts/My project
```

### 2. **Дождитесь компиляции**
- Unity скомпилирует MirrorRepairUtility
- Время: 30-60 секунд

### 3. **Запустите утилиту**
```
Tools → 🛠 Починить Mirror
```

### 4. **Проверьте Console**
- ✅ "Mirror добавлен в манифест"
- ✅ "Завершено. Теперь удалите папку Library и перезапустите Unity"

### 5. **Удалите Library**
```
rm -rf Library
```

### 6. **Перезапустите Unity**
```
Unity → Quit Unity
Unity Hub → Open → /Users/serg/Desktop/Projeсts/My project
```

### 7. **Дождитесь установки**
- Unity автоматически скачает Mirror
- Время: 2-5 минут
- В Console: "Packages were resolved"

## 📋 Проверка успеха:

### ✅ Unity Console должен показать:
- ❌ Нет ошибок "Mirror could not be found"
- ❌ Нет ошибок "NetworkBehaviour could not be found"
- ✅ Только информационные сообщения

### ✅ Package Manager показывает:
- ✅ Mirror установлен
- ✅ Источник: GitHub

### ✅ Project Window показывает:
- ✅ Все скрипты компилируются
- ✅ Префабы загружаются

## 🔧 Ручное исправление (если утилита не работает):

### 1. **Откройте manifest.json**
```
Packages/manifest.json
```

### 2. **Добавьте Mirror**
```json
"com.vis2k.mirror": "https://github.com/vis2k/Mirror.git?path=/Assets/Mirror",
```

### 3. **Удалите packages-lock.json**
```
rm Packages/packages-lock.json
```

### 4. **Перезапустите Unity**

## 📁 Файлы утилиты:

### Скрипты:
- ✅ `Assets/Scripts/Editor/MirrorRepairUtility.cs` - основная утилита
- ✅ `Assets/Scripts/Editor/MirrorRepairUtility.cs.meta` - GUID

### Пакеты:
- ✅ `Packages/manifest.json` - обновлен с правильным URL

## 🎯 Преимущества утилиты:

### ✅ Автоматизация:
- Не нужно вручную редактировать manifest.json
- Автоматическая проверка зависимостей
- Цветные логи для удобства

### ✅ Безопасность:
- Проверка существования файлов
- Обработка ошибок
- Предупреждения о конфликтах

### ✅ Удобство:
- Один клик для исправления
- Понятные сообщения
- Пошаговые инструкции

---

**Используйте утилиту при любых проблемах с Mirror! 🚀**