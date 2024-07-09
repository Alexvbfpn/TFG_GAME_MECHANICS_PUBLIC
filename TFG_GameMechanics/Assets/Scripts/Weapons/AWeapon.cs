using System;
using GameMechanics.EntitiesSystem.GowPlayer;
using Items.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public abstract class AWeapon : MonoBehaviour, IWeapon
    {
        public CrosshairData crosshairWeaponData;
        public WeaponStats weaponStats;

        public Transform weapon;
        public Transform hand;
        
        public UnityEvent onWeaponShoot;
        
        [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
        public Transform Root;
        
        [Tooltip
        (
            "Distance over which the projectile will correct its course to fit the intended trajectory " +
            "(used to drift projectiles towards center of screen in First Person view). At values under 0, there is no correction"
        )]
        public float TrajectoryCorrectionDistance = -1;
        
        protected bool m_HasTrajectoryOverride;
        protected Vector3 m_TrajectoryCorrectionVector;
        protected Vector3 m_ConsumedTrajectoryCorrectionVector;
        Vector3 m_LastRootPosition;
        public float axis { get; protected set; }

        protected Transform _startingParent;
        
        protected ArmedPlayer _player;
        protected Rigidbody _weaponRb;
        protected Collider _weaponCollider;
        
        protected bool _isOnAir = false;
        
        protected Vector3 origLocPos;
        protected Vector3 origLocRot;
        
        protected void InitializePlayer() => _player = GetComponentInParent<ArmedPlayer>();
        protected virtual void InitializeRigidbody() => _weaponRb = GetComponent<Rigidbody>();
        protected virtual void InitializeCollider() => _weaponCollider = GetComponent<Collider>();
        protected virtual void InitializeStartingParent() => _startingParent = transform.parent;
                
        public ArmedPlayer GetPlayer() => _player;
        
        protected virtual void Awake()
        {
            InitializePlayer();
            InitializeRigidbody();
            InitializeCollider();
            InitializeAxis();
            InitializeStartingParent();
            
            origLocPos = transform.localPosition;
            origLocRot = transform.localEulerAngles;
        }

        public virtual void ResetParent()
        {
            transform.parent = _startingParent;
            transform.localEulerAngles = origLocRot;
            transform.localPosition = origLocPos;
        }

        public virtual void Appear()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disappear()
        {
            gameObject.SetActive(false);
        }

        protected void TrajectoryCorrection()
        {
            m_HasTrajectoryOverride = true;
            
            Vector3 cameraToWeapon = weapon.position - Camera.main.transform.position;
            
            m_TrajectoryCorrectionVector = Vector3.ProjectOnPlane(-cameraToWeapon, Camera.main.transform.forward);

            if (TrajectoryCorrectionDistance > 0)
            {
                transform.position += m_TrajectoryCorrectionVector;
                m_ConsumedTrajectoryCorrectionVector = m_TrajectoryCorrectionVector;
            }
            else if(TrajectoryCorrectionDistance < 0)
            {
                m_HasTrajectoryOverride = false;
            }
        }
        
        protected void CorrectionDrift()
        {
            // Drift towards trajectory override (this is so that projectiles can be centered 
            // with the camera center even though the actual weapon is offset)
            if (m_HasTrajectoryOverride && m_ConsumedTrajectoryCorrectionVector.sqrMagnitude <
                m_TrajectoryCorrectionVector.sqrMagnitude)
            {
                Vector3 correctionLeft = m_TrajectoryCorrectionVector - m_ConsumedTrajectoryCorrectionVector;
                float distanceThisFrame = (Root.position - m_LastRootPosition).magnitude;
                Vector3 correctionThisFrame =
                    (distanceThisFrame / TrajectoryCorrectionDistance) * m_TrajectoryCorrectionVector;
                correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
                m_ConsumedTrajectoryCorrectionVector += correctionThisFrame;

                // Detect end of correction
                if (Math.Abs(m_ConsumedTrajectoryCorrectionVector.sqrMagnitude - m_TrajectoryCorrectionVector.sqrMagnitude) < Mathf.Epsilon)
                {
                    m_HasTrajectoryOverride = false;
                }

                transform.position += correctionThisFrame;
            }
                
            m_LastRootPosition = Root.position;
        }
        
        protected void Update()
        {
            OnStep();
        }

        public abstract void Shoot();

        public abstract void SpecialWeaponAbility();
        
        public abstract void InitializeAxis();

        public abstract void OnStep();

    }
}