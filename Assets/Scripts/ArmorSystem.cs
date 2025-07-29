using UnityEngine;

public class ArmorSystem : MonoBehaviour
{
    [Header("Armor Settings")]
    [SerializeField] private float frontArmor = 50f;
    [SerializeField] private float sideArmor = 50f;
    [SerializeField] private float backArmor = 50f;
    [SerializeField] private float topArmor = 50f;
    
    [Header("Angle Thresholds")]
    [SerializeField] private float frontAngle = 45f;
    [SerializeField] private float sideAngle = 45f;
    
    private Transform tankTransform;
    
    void Start()
    {
        tankTransform = transform;
    }
    
    public float GetEffectiveArmor(Vector2 hitPoint, Vector2 hitNormal)
    {
        // Calculate hit angle relative to tank's forward direction
        Vector2 tankForward = tankTransform.right; // Assuming tank faces right
        Vector2 hitDirection = (hitPoint - (Vector2)tankTransform.position).normalized;
        
        float angle = Vector2.Angle(tankForward, hitDirection);
        
        // Determine which side was hit
        ArmorSide side = DetermineHitSide(angle, hitDirection, tankForward);
        
        // Return appropriate armor value
        switch (side)
        {
            case ArmorSide.Front:
                return frontArmor;
            case ArmorSide.Side:
                return sideArmor;
            case ArmorSide.Back:
                return backArmor;
            case ArmorSide.Top:
                return topArmor;
            default:
                return sideArmor;
        }
    }
    
    ArmorSide DetermineHitSide(float angle, Vector2 hitDirection, Vector2 tankForward)
    {
        // Front hit (0-45 degrees)
        if (angle <= frontAngle)
        {
            return ArmorSide.Front;
        }
        
        // Back hit (135-180 degrees)
        if (angle >= 180f - frontAngle)
        {
            return ArmorSide.Back;
        }
        
        // Side hit (45-135 degrees)
        return ArmorSide.Side;
    }
    
    public float CalculateDamageReduction(float effectiveArmor, float penetrationPower)
    {
        // Calculate damage reduction based on armor vs penetration
        float armorRatio = effectiveArmor / penetrationPower;
        float damageReduction = Mathf.Clamp01(armorRatio);
        
        return damageReduction;
    }
    
    public Vector2 GetRicochetDirection(Vector2 incomingDirection, Vector2 hitNormal)
    {
        // Calculate ricochet direction
        Vector2 ricochetDirection = Vector2.Reflect(incomingDirection, hitNormal);
        return ricochetDirection.normalized;
    }
    
    public bool ShouldRicochet(float impactAngle, float ricochetThreshold)
    {
        return impactAngle > ricochetThreshold;
    }
    
    public enum ArmorSide
    {
        Front,
        Side,
        Back,
        Top
    }
    
    // Getters for armor values
    public float FrontArmor => frontArmor;
    public float SideArmor => sideArmor;
    public float BackArmor => backArmor;
    public float TopArmor => topArmor;
}