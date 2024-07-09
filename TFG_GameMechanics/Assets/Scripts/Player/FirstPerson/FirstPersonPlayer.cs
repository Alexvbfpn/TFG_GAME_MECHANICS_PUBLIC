using Misc;
using Misc.PickableObjects;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    public class FirstPersonPlayer : Player
    {
        public Transform pickableSlot;

        public LayerMask interactableLayer;
        public LayerMask ignoreLayer;


        public float offsetFactor;
        
        protected bool HitDetect;
        
        protected RaycastHit MHit;

        /// <summary>
        /// Returns true if the Player is holding an object.
        /// </summary>
        public bool holding { get; protected set; }
        
        /// <summary>
        /// Return the Pickable instance which the Player is holding.
        /// </summary>
        public IPickable pickable { get; protected set; }
        
        /// <summary>
        /// Return the Main camera instance.
        /// </summary>
        public Transform mainCamera { get; protected set; }
        
        
        protected void InitializeCamera() => mainCamera = Camera.main.transform;

        protected virtual void PickUpLogic()
        {
            if (MHit.transform.TryGetComponent(out APickable pickable))
            {
                PickUp(pickable);
                AimSightChanger.instance.ChangeSightSprite(AimType.Holding);
            }
        }
        
        public virtual void PickAndThrow()
        {
            if (stats.current.canPickUp && playerInputs.GetInteractDown())
            {
                if (!holding)
                {
                    //RaycastHit hit;
                    if (Physics.Raycast(mainCamera.transform.position, mainCamera.forward, out MHit, stats.current.pickDistance,
                            interactableLayer, QueryTriggerInteraction.Ignore))
                    {
                        PickUpLogic();
                    }
                }
                else
                {
                    Throw();
                }
            }
        }
        public virtual void PickUp(IPickable pickable)
        {
            if (!holding && (isGrounded || stats.current.canPickUpOnAir))
            {
                holding = true;
                this.pickable = pickable;
                pickable.PickUp(pickableSlot);
                if (pickable is APickable)
                {
                    pickable.pickableGameObject.GetComponent<APickable>().onRespawn.AddListener(RemovePickable);
                }
                events.onPickUp?.Invoke();
            }
        }
        
        public virtual void Throw()
        {
            if (holding)
            {
                //var force = lateralVelocity.magnitude * stats.current.throwVelocityMultiplier;
                pickable.Release(transform.forward);
                RemovePickable();
                events.onThrowPickable?.Invoke();
                //AimSightChanger.instance.ChangeSightSprite(AimType.Interact);
            }
        }
        
        public virtual void RemovePickable()
        {
            if (holding)
            {
                pickable = null;
                holding = false;
            }
        }

        public virtual void Interact()
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.forward, out hit, stats.current.pickDistance,
                    interactableLayer, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.TryGetComponent(out IInteractable interactable))
                {
                    AimSightChanger.instance.ChangeSightSprite(AimType.Interact);
                    interactable.Interact();
                }
            }
            else
            {
                AimSightChanger.instance.ChangeSightSprite(AimType.Default);
            }
        }

        public override void IdleStepSpecificLogic()
        {
            PickAndThrow();
            if (!holding)
            {
                Interact();
            }
        }
        
        public override void WalkStepSpecificLogic()
        {
            PickAndThrow();
            if (!holding)
            {
                Interact();
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            InitializeCamera();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}