# 🎮 World of Balance - Minimalistic Tank Duel Game

## 🎯 **CORE CONCEPT**

A skill-based 2D top-down tank duel game focused on **precise physics**, **honest mechanics**, and **strategic positioning**. No randomness, no unfair advantages - pure skill and physics determine the winner.

---

## 🧠 **GAME MECHANICS**

### **Tank Design**
- **Perfect square tanks** with visible turrets
- **Player Tank**: Red square with dark red turret
- **Enemy AI Tank**: Blue square with dark blue turret
- Each tank: `50 HP`, `50 Armor`

### **Turret System**
- **30-degree rotation limit** from tank body
- Turret always tries to aim at target (mouse for player, enemy for AI)
- When turret can't reach target, the **tank body rotates** to align
- Smooth interpolation for realistic movement

### **Combat System**
- **Bullet Stats**: `26 damage`, `51 penetration`, `12 speed`
- **30-degree rule**: Bullets only penetrate if impact angle ≤ 30°
- **Ricochet**: Bullets bounce off at steeper angles (> 30°)
- **Armor Effectiveness**: Calculated based on impact angle
- **No randomness**: Every shot is deterministic and predictable

### **Physics**
- **Precise 2D physics** with Rigidbody2D
- **Realistic movement** with proper damping and mass
- **Collision detection** with accurate angle calculations
- **Ricochet mechanics** follow real physics laws

---

## 🎮 **CONTROLS**

### **Player Controls**
- **WASD** - Tank movement
- **Mouse** - Turret aiming
- **Left Mouse Button** or **Space** - Fire
- **ESC** - Quit game

### **AI Behavior**
- **Intelligent targeting** - aims at player
- **Strategic movement** - moves towards player
- **Automatic firing** when in range
- **Same physics constraints** as player

---

## 🏗️ **TECHNICAL ARCHITECTURE**

### **Core Scripts**

#### **`TankController.cs`**
- Unified controller for both player and AI tanks
- Handles movement, turret rotation, and shooting
- Automatic detection of player vs AI based on tag
- Precise angle calculations for turret limits

#### **`TankBullet.cs`**
- Realistic bullet physics with ricochet
- Angle-based damage calculation
- Bounce limit system (max 5 bounces)
- Visual feedback for ricochets

#### **`GameManager.cs`**
- Simple game state management
- Win/lose condition detection
- Health UI updates
- Scene management

### **Scene Structure**

#### **`TankDuel.unity`**
- **Main Camera**: Fixed orthographic (size: 10)
- **Arena**: Gray background with white walls
- **Player Tank**: Position (-6, 0, 0)
- **Enemy Tank**: Position (6, 0, 0)
- **GameManager**: Handles game flow

### **Prefabs**

#### **`Tank.prefab`**
- Complete tank with turret and fire point
- Proper physics setup (mass: 2, damping: 5/10)
- Square sprite with rectangular turret

#### **`TankBullet.prefab`**
- Yellow circle bullet
- Physics-based movement
- Collision detection setup

---

## ⚙️ **GAME BALANCE**

### **Tank Stats**
```
Health: 50 HP
Armor: 50
Move Speed: 5 units/sec
Rotation Speed: 90°/sec
Fire Rate: 1 shot/sec
```

### **Bullet Stats**
```
Damage: 26 HP
Penetration: 51 (beats 50 armor)
Speed: 12 units/sec
Lifetime: 8 seconds
Max Bounces: 5
```

### **Turret Stats**
```
Max Angle: 30° from body
Rotation Speed: 180°/sec (player) / 120°/sec (AI)
```

### **Strategic Elements**
- **Positioning**: Angle your tank to deflect enemy shots
- **Timing**: Wait for the right moment to fire
- **Movement**: Use mobility to gain tactical advantage
- **Prediction**: Lead your shots for moving targets

---

## 🎨 **VISUAL DESIGN**

### **Minimalistic Style**
- **Clean pixel art** with basic shapes
- **High contrast colors** for clarity
- **No unnecessary effects** or animations
- **Readable UI** with essential information only

### **Color Scheme**
- **Player**: Red tank, dark red turret
- **Enemy**: Blue tank, dark blue turret
- **Bullets**: Yellow circles
- **Arena**: Gray background, white walls
- **Effects**: Yellow ricochet sparks

---

## 🚀 **HOW TO PLAY**

### **Setup in Unity**
1. Open `Assets/Scenes/TankDuel.unity`
2. Press Play
3. Game starts immediately (no menu system yet)

### **Gameplay Loop**
1. **Move** with WASD to position your tank
2. **Aim** with mouse - turret follows cursor
3. **Fire** with LMB when turret is aligned
4. **Dodge** enemy shots by angling your tank
5. **Win** by destroying the enemy tank (reduce HP to 0)

### **Strategy Tips**
- **Angle matters**: Face enemy shots at steep angles to deflect
- **Turret limits**: Use body rotation strategically
- **Ricochet shots**: Use walls to hit enemies from unexpected angles
- **Positioning**: Control distance and angles for advantage

---

## 🔧 **DEVELOPMENT NOTES**

### **Key Features Implemented**
✅ **Precise turret control** with 30° limit  
✅ **Angle-based armor system** (30° penetration rule)  
✅ **Realistic ricochet physics**  
✅ **Deterministic gameplay** (no randomness)  
✅ **Clean modular architecture**  
✅ **Fixed camera** for consistent gameplay  
✅ **AI opponent** with same mechanics as player  

### **Design Principles**
- **Honesty**: Every mechanic is visible and predictable
- **Skill-based**: Player skill determines outcome
- **Minimalism**: Focus on core mechanics, not flashy effects
- **Balance**: Both tanks have identical capabilities
- **Physics**: Realistic behavior within game constraints

### **Future Enhancements** (Optional)
- Simple start/game over UI
- Sound effects for shots and hits
- Particle effects for ricochets
- Multiple arena layouts
- Score/statistics tracking

---

## 🎯 **CORE PHILOSOPHY**

> *"This is a minimalistic but honest combat game. No randomness, no unfair mechanics. Every ricochet, penetration, and damage is readable and logical. The goal is purity and balance."*

The game rewards:
- **Tactical thinking** over reflexes
- **Understanding physics** over memorizing patterns  
- **Strategic positioning** over button mashing
- **Precision aiming** over spray-and-pray

---

## 📁 **PROJECT STRUCTURE**

```
Assets/
├── Scenes/
│   └── TankDuel.unity          # Main game scene
├── Scripts/
│   ├── TankController.cs       # Tank movement, turret, shooting
│   ├── TankBullet.cs          # Bullet physics and damage
│   └── GameManager.cs         # Game state management
└── Prefabs/
    ├── Tank.prefab            # Complete tank setup
    └── TankBullet.prefab      # Bullet with physics
```

**Ready to play!** 🎮 Open Unity, load the TankDuel scene, and experience precise tank combat!