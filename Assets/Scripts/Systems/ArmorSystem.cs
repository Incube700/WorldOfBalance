using UnityEngine;
using Mirror;

namespace WorldOfBalance.Systems
{
    /// <summary>
    /// Система брони игрока, определяющая эффективность защиты в зависимости от угла попадания
    /// </summary>
    public class ArmorSystem : NetworkBehaviour
{
    [Header("Armor Values")]
    public float frontArmor = 50f;
    public float sideArmor = 30f;
    public float backArmor = 20f;
    
    [Header("Penetration Settings")]
    public float penetrationThreshold = 0.7f; // Минимальный шанс пробития
    
    /// <summary>
    /// Определяет сторону попадания и возвращает эффективную брони
    /// </summary>
    /// <param name="hitDirection">Направление удара (от снаряда к игроку)</param>
    /// <param name="playerForward">Направление взгляда игрока</param>
    /// <returns>Эффективная броня для данного угла</returns>
    public float GetEffectiveArmor(Vector2 hitDirection, Vector2 playerForward)
    {
        // Нормализуем векторы
        hitDirection = hitDirection.normalized;
        playerForward = playerForward.normalized;
        
        // Вычисляем угол между направлением удара и направлением игрока
        float angle = Vector2.Angle(hitDirection, playerForward);
        
        // Определяем зону попадания
        ArmorZone zone = GetArmorZone(angle);
        
        // Получаем базовую броню для данной зоны
        float baseArmor = GetBaseArmor(zone);
        
        // Вычисляем эффективную броню с учетом угла попадания
        // Чем ближе угол к 90°, тем выше эффективная броня
        float effectiveArmor = baseArmor / Mathf.Cos(angle * Mathf.Deg2Rad);
        
        Debug.Log($"Hit angle: {angle}°, Zone: {zone}, Base armor: {baseArmor}, Effective armor: {effectiveArmor}");
        
        return effectiveArmor;
    }
    
    /// <summary>
    /// Определяет зону попадания на основе угла
    /// </summary>
    /// <param name="angle">Угол между направлением удара и направлением игрока</param>
    /// <returns>Зона попадания</returns>
    private ArmorZone GetArmorZone(float angle)
    {
        // Нормализуем угол к 0-180 градусам
        angle = Mathf.Abs(angle);
        
        if (angle <= 45f)
        {
            return ArmorZone.Front;
        }
        else if (angle <= 135f)
        {
            return ArmorZone.Side;
        }
        else
        {
            return ArmorZone.Back;
        }
    }
    
    /// <summary>
    /// Возвращает базовую броню для указанной зоны
    /// </summary>
    /// <param name="zone">Зона попадания</param>
    /// <returns>Базовая броня</returns>
    private float GetBaseArmor(ArmorZone zone)
    {
        switch (zone)
        {
            case ArmorZone.Front:
                return frontArmor;
            case ArmorZone.Side:
                return sideArmor;
            case ArmorZone.Back:
                return backArmor;
            default:
                return frontArmor;
        }
    }
    
    /// <summary>
    /// Проверяет, пробивает ли снаряд броню
    /// </summary>
    /// <param name="penetrationPower">Сила пробития снаряда</param>
    /// <param name="effectiveArmor">Эффективная броня</param>
    /// <returns>true если снаряд пробивает броню</returns>
    public bool CanPenetrate(float penetrationPower, float effectiveArmor)
    {
        // Вычисляем шанс пробития на основе соотношения силы пробития и брони
        float penetrationChance = Mathf.Clamp01(penetrationPower / effectiveArmor);
        
        // Добавляем базовый шанс пробития
        penetrationChance = Mathf.Max(penetrationChance, penetrationThreshold);
        
        // Проверяем пробитие
        bool canPenetrate = Random.value <= penetrationChance;
        
        Debug.Log($"Penetration check: Power={penetrationPower}, Armor={effectiveArmor}, Chance={penetrationChance}, Result={canPenetrate}");
        
        return canPenetrate;
    }
    
    /// <summary>
    /// Зоны брони игрока
    /// </summary>
    public enum ArmorZone
    {
        Front,  // Передняя броня (0-45°)
        Side,   // Боковая броня (45-135°)
        Back    // Задняя броня (135-180°)
    }
}
} 