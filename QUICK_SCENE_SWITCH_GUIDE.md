# 🎮 Quick Fix: Switch to TankDuel Scene

## ❗ **PROBLEM IDENTIFIED**
You're currently in the wrong scene! The tanks appear small and not moving because you're in an old scene (MainScene or TestScene) instead of the new TankDuel scene.

## 🚀 **QUICK FIX - Switch to Correct Scene:**

### **Step 1: Open TankDuel Scene**
1. In Unity's **Project** window (bottom panel)
2. Navigate to `Assets/Scenes/`
3. **Double-click** on `TankDuel.unity`

### **Step 2: What You Should See**
After switching, you should see:
- **Large red tank** on the left (Player)
- **Large blue tank** on the right (Enemy)
- **Gray arena** with white walls
- **Fixed camera** showing the entire battlefield

### **Step 3: Test the Game**
1. **Press Play** ▶️
2. **Use WASD** to move the red tank
3. **Move mouse** to aim the turret
4. **Left-click** to shoot

---

## 🔧 **Alternative: Manual Scene Setup**

If the TankDuel scene doesn't work, here's how to quickly set up the current scene:

### **Fix Current Scene:**
1. **Delete** the current Player and Enemy objects
2. **Drag** the `Tank.prefab` from `Assets/Prefabs/` into the scene **twice**
3. **Position** first tank at `(-6, 0, 0)` and set tag to "Player"
4. **Position** second tank at `(6, 0, 0)` and set tag to "Enemy"
5. **Set colors**: Player = Red, Enemy = Blue
6. **Add GameManager**: Create empty GameObject, add `GameManager.cs` script

---

## 🎯 **Expected Result**
- **Two large square tanks** facing each other
- **Visible turrets** (darker colored rectangles)
- **Smooth movement** with WASD
- **Turret rotation** following mouse
- **Working combat** system

The tanks should be **much larger** and clearly visible, not tiny red dots like in your current scene!

## 🚨 **If Still Having Issues**
If switching scenes doesn't work, let me know and I'll help you debug further. The issue is definitely that you're in the wrong scene - the new TankDuel scene has properly sized and positioned tanks.