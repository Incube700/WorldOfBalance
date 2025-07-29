using UnityEngine;

namespace WorldOfBalance.Systems
{
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
        
        public float FrontArmor => frontArmor;
        public float SideArmor => sideArmor;
        public float BackArmor => backArmor;
        
        /// <summary>
        /// Получает эффективную броню на основе угла попадания
        /// </summary>
        /// <param name="hitDirection">Направление попадания</param>
        /// <param name="playerForward">Направление "вперед" игрока</param>
        /// <returns>Эффективная броня</returns>
        public float GetEffectiveArmor(Vector2 hitDirection, Vector2 playerForward)
        {
            hitDirection = hitDirection.normalized;
            playerForward = playerForward.normalized;
            
            float angle = Vector2.Angle(hitDirection, playerForward);
            ArmorZone zone = GetArmorZone(angle);
            float baseArmor = GetBaseArmor(zone);
            
            // Формула эффективной брони: baseArmor / cos(angle)
            float effectiveArmor = baseArmor / Mathf.Cos(angle * Mathf.Deg2Rad);
            
            Debug.Log($"Hit angle: {angle}°, Zone: {zone}, Base armor: {baseArmor}, Effective armor: {effectiveArmor}");
            
            return effectiveArmor;
        }
        
        /// <summary>
        /// Определяет зону брони на основе угла попадания
        /// </summary>
        /// <param name="angle">Угол попадания в градусах</param>
        /// <returns>Зона брони</returns>
        private ArmorZone GetArmorZone(float angle)
        {
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
        /// Получает базовую броню для указанной зоны
        /// </summary>
        /// <param name="zone">Зона брони</param>
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
                    return sideArmor;
            }
        }
        
        /// <summary>
        /// Проверяет, может ли снаряд пробить броню
        /// </summary>
        /// <param name="penetrationPower">Пробивная способность снаряда</param>
        /// <param name="hitDirection">Направление попадания</param>
        /// <param name="playerForward">Направление "вперед" игрока</param>
        /// <returns>true, если снаряд может пробить броню</returns>
        public bool CanPenetrate(float penetrationPower, Vector2 hitDirection, Vector2 playerForward)
        {
            float effectiveArmor = GetEffectiveArmor(hitDirection, playerForward);
            return penetrationPower >= effectiveArmor;
        }
        
        /// <summary>
        /// Устанавливает значения брони
        /// </summary>
        /// <param name="front">Лобовая броня</param>
        /// <param name="side">Бортовая броня</param>
        /// <param name="back">Кормовая броня</param>
        public void SetArmorValues(float front, float side, float back)
        {
            frontArmor = front;
            sideArmor = side;
            backArmor = back;
        }
    }
} 