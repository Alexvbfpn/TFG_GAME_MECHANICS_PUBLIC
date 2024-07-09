using Cinemachine;
using DG.Tweening;
using GameMechanics.EntitiesSystem.GowPlayer;
using Misc.BreakableObjects;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Weapons
{
    public class AxeWeapon : AWeapon
    {
        public Transform spine;
        public Transform curvePoint;

        [Header("Specific References")]
        public TwoBoneIKConstraint pullConstraint;
        
        public TrailRenderer trailRenderer;
        
        public float impulseCameraMultiplier = 1f;
        
        protected bool _throwed = false;
        protected bool pulling = false;

        protected float returnTime = 0f;
        
        protected Vector3 pullPosition;

        protected CinemachineImpulseSource _impulseSource;
        
        protected void InitializeImpulseSource() => _impulseSource = FindObjectOfType<CinemachineImpulseSource>();
        
        public override void Shoot()
        {
            TrajectoryCorrection();
            //Debug.Log("AxeWeapon Shoot");
            ChangeTrailState(true);
            
            _isOnAir = true;
            _player.isHoldingWeapon = false;
            _weaponRb.isKinematic = false;
            _weaponRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            transform.parent = null;
            //transform.eulerAngles = new Vector3(0, -90 +transform.eulerAngles.y, 0);
            transform.position += transform.right/5;
            
            _weaponCollider.enabled = true;
            _weaponRb.AddForce(Camera.main.transform.forward * weaponStats.shootForce, ForceMode.Impulse);
        }

        public override void SpecialWeaponAbility()
        {
            if (_player.isHoldingWeapon || pulling) return;
            
            WeaponStartPull();
        }

        public override void InitializeAxis() => axis = 1;

        protected void WeaponStartPull()
        {
            GetComponent<Collider>().enabled = false;
            _player.isPulling = true;
            pullPosition = transform.position;
            _weaponRb.Sleep();
            _weaponRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _weaponRb.isKinematic = true;
            transform.DORotate(new Vector3(-90, -90, 0), .2f).SetEase(Ease.InOutSine);
            transform.DOBlendableLocalRotateBy(Vector3.right * 90, .5f);
            _isOnAir = true;
            pulling = true;
            pullConstraint.weight = 1;
            ChangeTrailState(true);
        }

        protected void WeaponCatch()
        {
            _weaponCollider.enabled = false;
            _player.isPulling = false;
            returnTime = 0;
            pulling = false;
            ResetParent();
            //transform.parent = hand;
            _isOnAir = false;
            transform.localEulerAngles = origLocRot;
            transform.localPosition = origLocPos;
            _player.isHoldingWeapon = true;
            //_player.states.Change<IdlePlayerState>();
            
            _impulseSource.GenerateImpulse(Vector3.right * impulseCameraMultiplier);
            ChangeTrailState(false);
            _player.onWeaponCatch?.Invoke();
            //pullConstraint.weight = 0;
        }

        #region --- COLLISIONS ---

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 11)
            {
                _isOnAir = false;
                print(collision.gameObject.name);
                _weaponRb.Sleep();
                _weaponRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                _weaponRb.isKinematic = true;
                ChangeTrailState(false);
            }

        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IBreakable> (out var breakable))
            {
                breakable.Break();
            }
        }

        #endregion
        
        
        
        protected void ChangeTrailState(bool state)
        {
            trailRenderer.emitting = state;
        }
        
        public Vector3 GetQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            return (uu * p0) + (2 * u * t * p1) + (tt * p2);
        }
        
        protected override void Awake()
        {
            base.Awake();
            //_player.events.onShoot.AddListener(Shoot);
            
            
            
            InitializeImpulseSource();
            ChangeTrailState(false);
            
            //_player.events.onSpecialAbility.AddListener(SpecialWeaponAbility);
            //Tween tween = transform.DOLocalRotate(Vector3.forward * weaponStats.rotationSpeed * Time.deltaTime, 0.1f).SetLoops();
        }

        public override void OnStep()
        {
            if (_isOnAir)
            {
                transform.localEulerAngles += Vector3.forward * (weaponStats.rotationSpeed * Time.deltaTime);
                CorrectionDrift();
            }
            
            if (pulling)
            {
                if(returnTime < 1)
                {
                    transform.position = GetQuadraticCurvePoint(returnTime, pullPosition, curvePoint.position, hand.position);
                    returnTime += Time.deltaTime * 1.5f;
                }
                else
                {
                    WeaponCatch();
                }
            }
        }
    }
}