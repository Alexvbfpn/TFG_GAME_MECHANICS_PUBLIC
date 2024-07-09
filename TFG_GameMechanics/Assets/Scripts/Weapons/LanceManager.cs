using System;
using System.Collections.Generic;
using Patterns.ObjectPool;
using Patterns.ObjectPool.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Weapons
{
    public class LanceManager : AWeapon
    {
        public float shootCooldown = 1.2f;
        
        public UnityEvent<int> onAmmoChange;
        
        public LanceWeapon currentLance { get; protected set; }
        
        public List<LanceWeapon> totalLances = new List<LanceWeapon>();
        protected bool _canShoot = true;
        //protected LanceObjectPool _lanceObjectPool;
        
        protected float m_lastTimeShot;
        
        public void SetCurrentLance(LanceWeapon lance)
        {
            currentLance = lance;
            currentLance.Appear();
        }
        
        public override void Shoot()
        {
            if(Time.time - m_lastTimeShot < shootCooldown) return;
            currentLance.Shoot();
            currentLance = null;
            m_lastTimeShot = Time.time;
            onWeaponShoot?.Invoke();
            // if (Time.time - m_lastTimeShot > 1 / elementsPerSecond)
            // {
            //     // currentLance = CreatePoolElement(elementPrototype, initialNumberOfElements, allowNewElements);
            //     // currentLance.gameObject.SetActive(true);
            //     // currentLance.transform.position = transform.position;
            //     // currentLance.transform.rotation = transform.rotation;
            //     // currentLance.Active = true;
            //     // m_lastTimeShot = Time.time;
            // }
        }

        public override void SpecialWeaponAbility()
        {
            if(_player.aiming) return;
            foreach (LanceWeapon l in totalLances)
            {
                if (l.Active && l != currentLance)
                {
                    l.SpecialWeaponAbility();
                }
            }
            onAmmoChange?.Invoke(-1);
        }

        public override void InitializeAxis() => axis = -1;
        public override void OnStep()
        {
            currentLance?.OnStep();
        }
    }
}