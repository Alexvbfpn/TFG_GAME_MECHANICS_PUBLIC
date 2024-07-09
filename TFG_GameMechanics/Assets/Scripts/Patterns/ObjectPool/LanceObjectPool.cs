using System.Collections;
using GameMechanics.EntitiesSystem.GowPlayer;
using Patterns.ObjectPool.Interfaces;
using UnityEngine;
using Weapons;

namespace Patterns.ObjectPool
{
    public class LanceObjectPool : AObjectPoolManager<LanceWeapon>
    {
        protected LanceManager m_lanceManager;
        
        protected void InitializeLanceManager() => m_lanceManager = GetComponent<LanceManager>();
        
        protected void Start()
        {
            base.Start();
            InitializeLanceManager();
            m_lanceManager.SetCurrentLance(CreatePoolElement(elementPrototype));
            m_lanceManager.onWeaponShoot?.AddListener(SpawnNewLance);
            foreach (IPooleableObject o in _elementsPool.objects)
            {
                m_lanceManager.totalLances.Add((LanceWeapon)o);
            }
            
            m_lanceManager.onAmmoChange.AddListener(ReleaseAllLances);
        }
        
        protected void ReleaseAllLances(int n)
        {
            if (n != -1) return;
            
            foreach (IPooleableObject o in _elementsPool.objects)
            {
                if (o.Active && o != m_lanceManager.currentLance)
                {
                    _elementsPool.Release(o);
                }
            }
        }
        
        protected void SpawnNewLance()
        {
            StartCoroutine(SpawnCoroutine(m_lanceManager.shootCooldown));
        }
        
        protected IEnumerator SpawnCoroutine(float secondsBetweenSpawns)
        {
            yield return new WaitForSeconds(secondsBetweenSpawns);
            LanceWeapon lance = CreatePoolElement(elementPrototype);
            m_lanceManager.onAmmoChange?.Invoke(_elementsPool.GetActive()-1);
            m_lanceManager.SetCurrentLance(lance);
            m_lanceManager.GetPlayer().isHoldingWeapon = true;
        }
    }
}