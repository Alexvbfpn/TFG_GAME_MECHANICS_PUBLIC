using UnityEngine;
using UnityEngine.InputSystem;

namespace GameMechanics.EntitiesSystem.GowPlayer
{
    public class ArmedPlayerInputManager : PlayerInputManager
    {
        protected InputAction Attack;
        protected InputAction Aim;
        protected InputAction Shoot;
        protected InputAction SpecialAbility;
        protected InputAction WeaponSwap;

        protected override void CacheActions()
        {
            base.CacheActions();
            Attack = actions["Attack"];
            Aim = actions["Aim"];
            Shoot = actions["Shoot"];
            SpecialAbility = actions["SpecialAbility"];
            WeaponSwap = actions["WeaponSwap"];
        }
        
        #region -- GET INPUT PRESSED --
        public virtual bool GetAttackDown() => Attack.WasPressedThisFrame();
        
        public virtual bool GetAimDown() => Aim.WasPressedThisFrame();
        
        public virtual bool GetAimUp() => Aim.WasReleasedThisFrame();
        
        public virtual bool GetWeaponSwapDown() => WeaponSwap.WasPressedThisFrame();
        
        public virtual bool GetSpecialAbilityDown() => SpecialAbility.WasPressedThisFrame();
        
        public virtual bool GetShootDown() => Shoot.WasPressedThisFrame();

        #endregion
        
        #region -- GET DIRECTIONS --

        public virtual float GetWeaponSwapDirection() => WeaponSwap.ReadValue<float>();
        
        #endregion
    }
}