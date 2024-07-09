using GameMechanics.EntitiesSystem;
using GameMechanics.EntitiesSystem.States;
using UnityEngine;

namespace Weapons
{
    public class WeaponAnimListener : MonoBehaviour
    {
        public void OnShoot()
        {
            GetComponentInChildren<WeaponManager>().currentWeapon.Shoot();
            FindObjectOfType<Player>().states.Change<IdlePlayerState>();
        }
        
    }
}