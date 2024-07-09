using System;
using System.Collections.Generic;
using GameMechanics.EntitiesSystem.GowPlayer;
using UnityEngine;

namespace Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        public List<AWeapon> weapons;
        
        protected IWeapon _currentWeapon;
        protected ArmedPlayer _player;

        public IWeapon currentWeapon => _currentWeapon;
        
        protected void InitializePlayer() => _player = GetComponentInParent<ArmedPlayer>();
        
        public void SetCurrentWeapon(AWeapon weapon)
        {
            _currentWeapon = weapon;
        }

        public void ChangeWeapon()
        {
        }

        protected void SetUpNewWeapon(float axis)
        {
            _player.events.onSpecialAbility.RemoveListener(() => _currentWeapon.SpecialWeaponAbility()); 
            ChangeWeapon(axis);
            _player.events.onSpecialAbility.AddListener(() => _currentWeapon.SpecialWeaponAbility());
        }
        
        protected void ChangeWeapon(float axis)
        {
            if(_currentWeapon.axis == axis) return;
            
            _currentWeapon.Disappear();
            // Debug.Log("Comparacion de prueba, axis: " + axis + " y weapon.axis: " + _currentWeapon.axis + "! ¿son iguales? " + (-axis == _currentWeapon.axis));
            // Debug.Log("Weapon BEFORE change " + _currentWeapon.GetType().Name + " with axis " + _currentWeapon.axis + "!");
            // Debug.Log(weapons.Find(weapon => Math.Abs(weapon.axis - axis) < Mathf.Epsilon));
            foreach (AWeapon w in weapons)
            {
                Debug.Log(w.GetType().Name + " with axis " + w.axis + "!");
                Debug.Log("Comparacion de prueba, axis: " + axis + " y weapon.axis: " + w.axis + "! ¿son iguales? " + (axis == w.axis));
            }
            SetCurrentWeapon(weapons.Find(weapon => weapon.axis == axis));
            Debug.Log("Changed weapon to " + _currentWeapon.GetType().Name + " with axis " + axis + "!");
            _currentWeapon.Appear();
        }  
        
        // public void ChangeWeapon(int index)
        // {
        //     if (index < 0 || index >= weapons.Count) return;
        //     _currentWeapon.Disappear();
        //     _currentWeapon = weapons[index];
        //     _currentWeapon.Appear();
        // }


        protected void SetUpWeapons()
        {
            foreach (AWeapon weapon in weapons)
            {
                //weapons.Add(weapon);
                weapon.Disappear();
            }
            SetCurrentWeapon(weapons[1]);
            //SetUpNewWeapon(1);
            _player.onWeaponSwap.Invoke(1);
            //ChangeWeapon(1);
            //_currentWeapon.Appear();
        }
        
        protected void Awake()
        {
            InitializePlayer();
            //SetCurrentWeapon(weapons[0]);
        }

        protected void Start()
        {
            _player.onWeaponSwap.AddListener(SetUpNewWeapon);
            SetUpWeapons();
        }

        protected void Update()
        {
            
        }
    }
}