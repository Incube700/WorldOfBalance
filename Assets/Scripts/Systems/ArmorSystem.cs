using UnityEngine;

public class ArmorSystem : MonoBehaviour
{
    [Header("Armor Values")]
    [SerializeField] private float frontArmor = 50f;
    [SerializeField] private float sideArmor = 30f;
    [SerializeField] private float backArmor = 20f;
    
    public enum ArmorZone
    {
        Front,
        Side,
        Back
    }
    
    public float GetEffectiveArmor(Vector2 hitDirection, Vector2 playerForward)
    {
        ArmorZone zone = GetArmorZone(hitDirection, playerForward);
        float baseArmor = GetBaseArmor(zone);
        
        // Вычисляем угол попадания относительно нормали поверхности
        float angle = Vector2.Angle(-hitDirection, playerForward);
        
        // Если угол острый (близко к касательной), броня эффективнее
        float effectiveArmor = baseArmor / Mathf.Cos(angle * Mathf.Deg2Rad);
        
        Debug.Log($"Hit zone: {zone}, Base armor: {baseArmor}, Angle: {angle:F1}°, Effective armor: {effectiveArmor:F1}");
        
        return effectiveArmor;
    }
    
    private ArmorZone GetArmorZone(Vector2 hitDirection, Vector2 playerForward)
    {
        // Вычисляем угол между направлением попадания и передней частью игрока
        float angle = Vector2.SignedAngle(playerForward, hitDirection);
        
        // Нормализуем угол к положительному диапазону
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
    
    public void SetArmorValues(float front, float side, float back)
    {
        frontArmor = front;
        sideArmor = side;
        backArmor = back;
    }
} 