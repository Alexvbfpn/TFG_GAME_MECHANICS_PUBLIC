using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "Weapon Stats", menuName = "GameMechanics/Weapons/New Weapon Stats", order = 0)]
    public class WeaponStats : ScriptableObject
    {
        [Header("Distance Attack Stats")]
        public float shootForce = 30f;
        public float rotationSpeed = -1800f;
        public float projectileSpeed = 10f;
        public float projectileBackSpeed = 5f;
        public float maximumProjectiles = 1;
        public float shootCooldown = 0.5f;
    }
}