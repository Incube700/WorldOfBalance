using UnityEngine;
using Mirror;

namespace WorldOfBalance.Systems
{
    public class ArmorSystem : NetworkBehaviour
    {
        [Header("Armor Values")]
        [SerializeField] private float frontArmor = 50f;
        [SerializeField] private float sideArmor = 50f;
        [SerializeField] private float backArmor = 50f;
        
        [Header("Armor Zones")]
        [SerializeField] private float frontZoneAngle = 45f;
        [SerializeField] private float sideZoneAngle = 90f;
        
        public enum ArmorZone
        {
            Front,
            Side,
            Back
        }
        
        private void Awake()
        {
            // Убеждаемся, что углы зон корректны
            if (frontZoneAngle <= 0f || sideZoneAngle <= frontZoneAngle)
            {
                Debug.LogWarning("ArmorSystem: Invalid zone angles! Using defaults.");
                frontZoneAngle = 45f;
                sideZoneAngle = 90f;
            }
        }
        
        /// <summary>
        /// Вычисляет эффективную броню на основе угла попадания
        /// </summary>
        /// <param name="hitDirection">Направление попадания (от снаряда к игроку)</param>
        /// <param name="playerForward">Направление, куда смотрит игрок</param>
        /// <returns>Эффективная броня</returns>
        public float GetEffectiveArmor(Vector2 hitDirection, Vector2 playerForward)
        {
            hitDirection = hitDirection.normalized;
            playerForward = playerForward.normalized;
            
            // Вычисляем угол между направлением попадания и направлением игрока
            float angle = Vector2.Angle(hitDirection, playerForward);
            
            // Определяем зону брони
            ArmorZone zone = GetArmorZone(angle);
            
            // Получаем базовую броню для этой зоны
            float baseArmor = GetBaseArmor(zone);
            
            // Вычисляем эффективную броню по формуле: effectiveArmor = baseArmor / cos(angle)
            float effectiveArmor = baseArmor / Mathf.Cos(angle * Mathf.Deg2Rad);
            
            Debug.Log($"Hit angle: {angle}°, Zone: {zone}, Base armor: {baseArmor}, Effective armor: {effectiveArmor}");
            
            return effectiveArmor;
        }
        
        /// <summary>
        /// Проверяет, может ли снаряд пробить броню
        /// </summary>
        /// <param name="penetrationPower">Сила пробития снаряда</param>
        /// <param name="hitDirection">Направление попадания</param>
        /// <param name="playerForward">Направление игрока</param>
        /// <returns>true, если снаряд может пробить броню</returns>
        public bool CanPenetrate(float penetrationPower, Vector2 hitDirection, Vector2 playerForward)
        {
            float effectiveArmor = GetEffectiveArmor(hitDirection, playerForward);
            return penetrationPower >= effectiveArmor;
        }
        
        /// <summary>
        /// Определяет зону брони на основе угла попадания
        /// </summary>
        /// <param name="angle">Угол между направлением попадания и направлением игрока</param>
        /// <returns>Зона брони</returns>
        private ArmorZone GetArmorZone(float angle)
        {
            if (angle <= frontZoneAngle)
            {
                return ArmorZone.Front;
            }
            else if (angle <= sideZoneAngle)
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
        /// Устанавливает значения брони для всех зон
        /// </summary>
        /// <param name="front">Броня спереди</param>
        /// <param name="side">Броня сбоку</param>
        /// <param name="back">Броня сзади</param>
        [Server]
        public void SetArmorValues(float front, float side, float back)
        {
            frontArmor = front;
            sideArmor = side;
            backArmor = back;
        }
        
        /// <summary>
        /// Получает текущие значения брони
        /// </summary>
        /// <returns>Кортеж с значениями брони (front, side, back)</returns>
        public (float front, float side, float back) GetArmorValues()
        {
            return (frontArmor, sideArmor, backArmor);
        }
        
        /// <summary>
        /// Получает информацию о зонах брони
        /// </summary>
        /// <returns>Кортеж с углами зон (frontAngle, sideAngle)</returns>
        public (float frontAngle, float sideAngle) GetZoneAngles()
        {
            return (frontZoneAngle, sideZoneAngle);
        }
    }
} 