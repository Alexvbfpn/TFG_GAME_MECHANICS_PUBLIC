using System;
using UnityEngine.Events;

namespace Items
{
    [Serializable]
    public class ItemEvents
    {
        public UnityEvent onMoveAimPosition;
        
        public UnityEvent onMoveSpecialPosition;
        
        public UnityEvent onMoveDefaultPosition;
        
        public UnityEvent onMoveInactivePosition;
        
        public UnityEvent onShoot;
    }
}