using UnityEngine;

namespace Misc.PickableObjects
{
    public interface IPickable
    {
        public GameObject pickableGameObject { get; set; }

        protected virtual void InitializeGameObject() { }
        
        public virtual void PickUp(Transform slot) { }
        public virtual void Release(Vector3 direction, float force = default) { }
        public virtual void Respawn() { }
    }
}