using UnityEngine;
using UnityEngine.Events;

namespace Misc.PickableObjects
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class APickable : MonoBehaviour, IPickable, IInteractable
    {
        [Header("General Settings")]
        public Vector3 offset;
        public float releaseOffset = 0.0f;

        [Header("Respawn Settings")]
        public bool autoRespawn;
        public bool respawnOnHitHazards;
        public float respawnHeightLimit = -100;
        
        [Space(15)]

        /// <summary>
        /// Called when this object is Picked.
        /// </summary>
        public UnityEvent onPicked;

        /// <summary>
        /// Called when this object is Released.
        /// </summary>
        public UnityEvent onReleased;

        /// <summary>
        /// Called when this object is respawned.
        /// </summary>
        public UnityEvent onRespawn;
        
        protected Collider m_collider { get; set; }
        protected Rigidbody m_rigidbody { get; set; }
        
        protected Vector3 m_initialPosition;
        protected Quaternion m_initialRotation;
        protected Transform m_initialParent;
        
        protected RigidbodyInterpolation m_interpolation;

        public bool beingHold { get; protected set; }
        
        protected virtual void InitializeGameObject() { pickableGameObject = gameObject; }
        
        // -- PICKED VIEW VARIABLES --
        
        protected GameObject m_skin;
        protected LayerMask m_defaultLayer;
        
        protected void InitializeRigidbody() => m_rigidbody = GetComponent<Rigidbody>();
        protected void InitializeCollider() => m_collider = TryGetComponent(out Collider collider) ? collider : GetComponentInChildren<Collider>();
        protected void InitializeSkin() => m_skin = GetComponentInChildren<MeshRenderer>().gameObject;
        protected void InitializeDefaultLayer() => m_defaultLayer = pickableGameObject.layer;
        
        public GameObject pickableGameObject { get; set; }
        public GameObject GetSkin() => m_skin;


        public virtual void PickUp(Transform slot)
        {
            if(beingHold) return;
            beingHold = true;
            //transform.parent = slot;
            //transform.localPosition = Vector3.zero + offset;
            m_collider.isTrigger = true;
            m_interpolation = m_rigidbody.interpolation;
            m_rigidbody.interpolation = RigidbodyInterpolation.None;
            m_rigidbody.isKinematic = true;
            
            ChangeLayer(beingHold);           
            
            onPicked?.Invoke();
        }
        
        public virtual void Release(Vector3 direction, float force = default)
        {
            if (!beingHold) return;
            
            transform.parent = m_initialParent;
            transform.position += direction * releaseOffset;
            m_collider.isTrigger = beingHold = false;
            m_rigidbody.interpolation = m_interpolation;
            //m_rigidBody.velocity = direction * force;
            m_rigidbody.isKinematic = false;

            ChangeLayer(beingHold);            
            
            onReleased?.Invoke();
        }
        
        public virtual void Respawn()
        {
            m_rigidbody.velocity = Vector3.zero;
            transform.parent = m_initialParent;
            transform.SetPositionAndRotation(m_initialPosition, m_initialRotation);
            m_rigidbody.isKinematic = m_collider.isTrigger = beingHold = false;
            onRespawn?.Invoke();
        }
        
        public virtual void Interact()
        {
            //Debug.Log("Interacting with " + name);
        }
        
        protected virtual void ChangeLayer(bool isPicked)
        {
            pickableGameObject.layer = m_skin.layer = LayerMask.NameToLayer(isPicked ? "FirstViewer" : LayerMask.LayerToName(m_defaultLayer));
        }

        protected virtual void Awake()
        {
            InitializeGameObject();
            InitializeRigidbody();
            InitializeCollider();
            InitializeSkin();
            InitializeDefaultLayer();
        }
    }
}