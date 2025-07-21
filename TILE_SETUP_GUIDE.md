# Руководство по настройке тайлов для Overlord:Rise and Slice

## Проблема: Не видно тайлы при рисовании уровней

### Решение:

#### 1. Настройка Tile Palette
1. **Откройте Window → 2D → Tile Palette**
2. **Создайте новую палитру:**
   - Нажмите "Create New Palette"
   - Назовите "OverlordPalette"
   - Выберите Grid Type: "Rectangle"
   - Cell Size: "Automatic"

#### 2. Настройка тайлсетов
1. **Выберите спрайт** в Project window
2. **В Inspector нажмите "Sprite Editor"**
3. **Slice спрайт:**
   - Type: "Grid By Cell Size"
   - Pixel Size: 16x16 (или размер ваших тайлов)
   - Нажмите "Slice"

#### 3. Создание Rule Tiles
1. **Правый клик в Project → Create → 2D → Tiles → Rule Tile**
2. **Назовите:**
   - FloorRuleTile
   - WallRuleTile
   - DecorationRuleTile

#### 4. Настройка Rule Tiles
1. **Выберите Rule Tile**
2. **В Inspector:**
   - Default Sprite: выберите базовый спрайт
   - Default Collider Type: 
     - Floor: "Sprite"
     - Wall: "Sprite"
     - Decoration: "None"

#### 5. Добавление в Tile Palette
1. **Откройте Tile Palette**
2. **Перетащите Rule Tiles** в палитру
3. **Теперь вы можете рисовать!**

### Необходимые тайлсеты для игры:

#### 1. Floor Tileset
- **Назначение:** Полы, дороги, тропинки
- **Коллизия:** Есть (для ходьбы)
- **Типы:** Камень, дерево, трава, ковер

#### 2. Wall Tileset
- **Назначение:** Стены, препятствия
- **Коллизия:** Есть (непроходимые)
- **Типы:** Каменные стены, деревянные, металлические

#### 3. Decoration Tileset
- **Назначение:** Декорации, мебель
- **Коллизия:** Нет (только визуал)
- **Типы:** Столы, стулья, факелы, сундуки

#### 4. Special Tileset
- **Назначение:** Особые элементы
- **Коллизия:** Зависит от типа
- **Типы:** Двери, лестницы, порталы

### Настройка в Unity Editor:

1. **Создайте Grid объект** в сцене
2. **Добавьте Tilemap компоненты:**
   - Floor Tilemap (Sorting Layer: Background)
   - Wall Tilemap (Sorting Layer: Default)
   - Decoration Tilemap (Sorting Layer: Foreground)

3. **Настройте слои:**
   - Background: -1
   - Default: 0
   - Foreground: 1

### Проверка работы:

1. **Выберите тайл** в Tile Palette
2. **Нажмите на сцену** - должен появиться тайл
3. **Если не видно:**
   - Проверьте Sorting Layer
   - Проверьте Z позицию камеры
   - Проверьте настройки спрайта

### Дополнительные настройки:

#### Для лучшей видимости:
1. **Camera → Projection: Orthographic**
2. **Camera → Size: 5** (или подходящий размер)
3. **Grid → Cell Size: 1,1,1**

#### Для автоматического тайлинга:
1. **Rule Tile → Tiling Rules**
2. **Добавьте правила** для соседних тайлов
3. **Настройте спрайты** для разных комбинаций

Теперь вы должны видеть тайлы при рисовании уровней! 