using System;
using UnityEngine.Events;

namespace GameMechanics.EntitiesSystem
{
    [Serializable]
    public class PlayerEvents
    {
        public UnityEvent<bool> onAim;
        
        public UnityEvent onStopAim;
        
        public UnityEvent onShoot;
        
        public UnityEvent onSpecialAbility;
        
        public UnityEvent<bool> onPointingAtEnemy;
        
        public UnityEvent<bool> onPointingAtInteractable;
        
        public UnityEvent onPickUp;
        
        public UnityEvent onThrowPickable;
        
        // --- PLATFORMER EVENTS ---
        
        public UnityEvent onJump;
        
        public UnityEvent onLedgeGrabbed;
    }
}