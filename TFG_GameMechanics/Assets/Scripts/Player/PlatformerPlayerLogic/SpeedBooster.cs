using System.Collections;
using UnityEngine;

namespace GameMechanics.EntitiesSystem.PlatformerPlayerLogic
{
    public class SpeedBooster : MonoBehaviour
    {
        [Header("Implicit States")] 
        [SerializeField] 
        protected bool chargingSpeedBooster;
        [SerializeField]
        protected bool speedBoosterActive;
        
        
        [Header("Settings")]
        [SerializeField] protected float chargeTime = 1.5f;
        
        
        protected Coroutine ChargeSpeedBoosterCoroutine;
        
        
        public void ChargeSpeedBooster(bool charge)
        {
            chargingSpeedBooster = charge;

            if (charge)
            {
                
            }

            
        }
        protected IEnumerator ChargeCoroutine()
        {
            yield return new WaitForSeconds(chargeTime);
            
        }
    }
}