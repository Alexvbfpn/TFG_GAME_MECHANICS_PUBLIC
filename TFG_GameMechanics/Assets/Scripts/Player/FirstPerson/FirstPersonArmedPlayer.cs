using System.Collections;
using GameMechanics.EntitiesSystem.GowPlayer;
using Misc;
using Misc.PickableObjects;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    [RequireComponent(typeof(PlayerWeaponsManager))]
    public class FirstPersonArmedPlayer : FirstPersonPlayer
    {
        protected PlayerWeaponsManager m_weaponsManager;
        
        public float movementTimeScale = 1f;
        public float idleTimeScale = 0.03f;
        public float idleLerpTime = 0.5f;
        public float movementLerpTime = 0.05f;

        public bool doingAction;
        
        #region -- INPUT MANAGER --
        
        /// <summary>
        /// Returns the Player Input Manager instance.
        /// </summary>
        public ArmedPlayerInputManager armedPlayerInputs
        {
            get => (ArmedPlayerInputManager)playerInputs; 
            protected set => playerInputs = value;
        }
        
        public override PlayerInputManager playerInputs
        {
            get => base.playerInputs;
            protected set
            {
                if (value is ArmedPlayerInputManager armedPlayerInputManager)
                    base.playerInputs = armedPlayerInputManager;
                else
                    Debug.LogError("Player Inputs must be of type ArmedPlayerInputManager.");
            }
        }
        protected override void InitializeInputs() => playerInputs = GetComponent<ArmedPlayerInputManager>();
        
        #endregion
        
        protected void InitializeWeaponsManager() => m_weaponsManager = GetComponent<PlayerWeaponsManager>();

        public override void SnapToGround(float force)
        {
            if (isGrounded && (verticalVelocity.y <= 0))
            {
                verticalVelocity = Vector3.down * (force * Time.deltaTime);
            }
        }
        
        public void CheckPointingToSomething()
        {
            CheckPointingToEnemy();
            CheckPointingToWeapon();
        }

        protected void CheckPointingToEnemy()
        {
            //if(m_weaponsManager.GetActiveWeapon() == null) return;
            //RaycastHit hit;
            if(Physics.Raycast(mainCamera.position, mainCamera.forward, out MHit, 1000, -1, QueryTriggerInteraction.Collide))
            {
                //m_weaponsManager.isPointingAtEnemy = false;
                if (MHit.transform.GetComponent<Damageable>())
                {
                    if (!m_weaponsManager.isPointingAtEnemy)
                    {
                        m_weaponsManager.isPointingAtEnemy = true;
                        events.onPointingAtEnemy?.Invoke(m_weaponsManager.isPointingAtEnemy);
                    }
                }
                else
                {
                    m_weaponsManager.isPointingAtEnemy = false;
                    events.onPointingAtEnemy?.Invoke(m_weaponsManager.isPointingAtEnemy);
                }
            }
            else
            {
                if (m_weaponsManager.isPointingAtEnemy)
                {
                    m_weaponsManager.isPointingAtEnemy = false;
                    events.onPointingAtEnemy?.Invoke(m_weaponsManager.isPointingAtEnemy);
                }
            }
        }

        protected void CheckPointingToWeapon()
        {
            PickAndThrow();
        }
        
        protected override void PickUpLogic()
        {
            if (MHit.transform.TryGetComponent(out PickableWeapon pickable))
            {
                m_weaponsManager.AddWeapon(pickable.weaponController);
                StopCoroutine(ActionExecuted(0.1f));
                StartCoroutine(ActionExecuted(0.1f));
                PickUp(pickable);
            }
        }

        public override void Throw()
        {
            if (holding)
            {
                StopCoroutine(ActionExecuted(0.4f));
                StartCoroutine(ActionExecuted(0.4f));
                pickable.Release(transform.forward);
                RemovePickable();
               
            }
        }

        public override void RemovePickable()
        {
            StopCoroutine(ActionExecuted(0.4f));
            StartCoroutine(ActionExecuted(0.4f));
            base.RemovePickable();
            m_weaponsManager.RemoveCurrentWeapon();
        }
        
        public void ShootItem()
        {
            if (armedPlayerInputs.GetShootDown())
            {
                StopCoroutine(ActionExecuted(0.03f));
                StartCoroutine(ActionExecuted(0.03f));
                
                events.onShoot?.Invoke();
            }
        }

        public void SlowMotionHandle()
        {
            // Debug.Log("Velocity: " + lateralVelocity + " Magnitude: " + lateralVelocity.magnitude);
            // float time = lateralVelocity.sqrMagnitude == 0 ? 0.3f : 1f;
            // float lerpTime = lateralVelocity.sqrMagnitude == 0 ? 0.5f : 0.1f;
            //
            // Time.timeScale = time;
        }

        protected IEnumerator ActionExecuted(float time)
        {
            doingAction = true;
            yield return new WaitForSecondsRealtime(0.06f);
            doingAction = false;
        }
        
        public override void IdleStepSpecificLogic()
        {
            CheckPointingToSomething();
            ShootItem();
            //SlowMotionHandle();
        }
        
        public override void WalkStepSpecificLogic()
        {
            CheckPointingToSomething();
            ShootItem();
            //SlowMotionHandle();
        }
        
        protected override void Awake()
        {
            base.Awake();
            InitializeWeaponsManager();
        }
        
    }
}