using System.Collections.Generic;
using Items.Weapons.Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace GameMechanics.EntitiesSystem
{
    public class EntityWeaponsManager : MonoBehaviour
    {
        [Tooltip("List of weapons the player will start with")]
        public List<WeaponController> startWeapons = new List<WeaponController>();
        
        [Tooltip("Parent transform where all weapon will be added in the hierarchy")]
        public Transform weaponParentSocket;
        
        public int activeItemIndex { get; protected set; }
        
        public UnityAction<WeaponController> OnSwitchedToWeapon;
        public UnityAction<WeaponController> OnAddedWeapon;
        public UnityAction<WeaponController> OnRemovedWeapon;
        
        protected Player m_player;
        
        protected WeaponController m_CurrentWeapon;
        
        protected virtual void InitializePlayer() => m_player = FindObjectOfType<Player>();
        
        
        protected void SetupWeapons()
        {
            activeItemIndex = 0;
            m_CurrentWeapon.ChangeActiveState(true);
            OnSwitchedToWeapon?.Invoke(m_CurrentWeapon);
            //GetActiveItem().MoveToDefaultPosition();
        }
        
        protected void InitializeStartingWeapons()
        {
            if (startWeapons.Count == 0) return;
            
            m_CurrentWeapon = startWeapons[0];
            
            SetupWeapons();
            // foreach (WeaponController weapon in startWeapons)
            // {
            //     AddWeapon(weapon);
            // }
        }
        
        protected virtual void Start()
        {
            activeItemIndex = -1;
            InitializeStartingWeapons();
        }
    }
}