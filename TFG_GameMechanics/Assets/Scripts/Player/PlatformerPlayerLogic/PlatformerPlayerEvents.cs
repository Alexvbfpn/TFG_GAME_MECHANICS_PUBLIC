using System;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameMechanics.EntitiesSystem.PlatformerPlayerLogic
{
    [Serializable]
    public class PlatformerPlayerEvents
    {
        public UnityEvent onSlide;
        
        public UnityEvent onStartSlide;
        
        public UnityEvent onStopSlide;
        
        public UnityEvent onChargingBoost;
        
        public UnityEvent onUnchargingBoost;

        public UnityEvent<bool> onBoost;

        public UnityEvent onShinesparkCharged;
        
        public UnityEvent onShinesparkUncharged;
        
        public UnityEvent onShinesparkStarted;
        
        public UnityEvent onShinesparkUsed;
        
        public UnityEvent<float> onShinesparkAngleSet;

        public UnityEvent onDirectionChange;

        public UnityEvent<int> onImpactSideSet;

        public UnityEvent<float> onDashAngleSet;
        
        
    }
}