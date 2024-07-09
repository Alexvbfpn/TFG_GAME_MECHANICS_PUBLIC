using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using GameMechanics;
using GameMechanics.EntitiesSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Items.Weapons.Controllers
{
    public class WeaponController : ItemController
    {
        [Tooltip("Default data for the crosshair")]
        public CrosshairData crosshairDataDefault;
        
        [Tooltip("Data for the crosshair when targeting an enemy")]
        public CrosshairData crosshairDataTargetInSight;
        
        // [Tooltip("Data for the crosshair when aiming")]
        // public CrosshairData crosshairDataAiming;

        [Header("Internal References")] 
        public GameObject weaponRoot;
        
        [Tooltip("Tip of the weapon, where the projectiles are shot")]
        public Transform weaponMuzzle;
        
        [Header("Shoot Parameters")] [Tooltip("The type of weapon wil affect how it shoots")]
        public WeaponShootType shootType;
        
        //Projectile or bullet CLASS
        public GameObject projectilePrefab;
        
        [Tooltip("Minimum duration between two shots")]
        public float delayBetweenShots = 0.5f;
        
        [Tooltip("Angle for the cone in which the bullets will be shot randomly (0 means no spread at all)")]
        public float bulletSpreadAngle = 0f;
        
        [Tooltip("Amount of bullets per shot")]
        public int bulletsPerShot = 1;
        
        [Tooltip("Force that will push back the weapon after each shot")] [Range(0f, 2f)]
        public float recoilForce = 1;
        
        [Tooltip("Ratio of the default FOV that this weapon applies while aiming")] [Range(0f, 1f)]
        public float aimZoomRatio = 1f;
        
        [Tooltip("Maximum amount of ammo in the gun")]
        public int maxAmmo = 5;

        [Tooltip("Duration of first movement of the weapon when detach")]
        public float throwWeaponDetachDuration = 0.01f;
        [Tooltip("Force of weapon throw")]
        public float throwForce = 10;
        [Tooltip("Torque of weapon throw")]
        public float throwTorque = 20;

        protected Transform m_shootOrigin;
        protected float m_shootOffset;
        
        public bool isCurrentPlayerWeapon { get; protected set; }
        
        public bool isThrown { get; protected set; }
        
        public UnityAction OnShoot;
        public event Action OnShootProcessed;
        
        protected float m_currentAmmo;
        protected float m_lastTimeShot = Mathf.NegativeInfinity;
        public float lastChargeTriggerTimestamp { get; private set; }
        Vector3 m_lastMuzzlePosition;
        
        protected Rigidbody m_rigidbody;
        protected Collider m_collider;
        public int GetCurrentAmmo() => Mathf.FloorToInt(m_currentAmmo);
        
        private Queue<Rigidbody> m_PhysicalAmmoPool;

        protected void InitializeWeaponMuzzle() => weaponMuzzle = weaponMuzzle == null ? Camera.main.transform : weaponMuzzle;
        protected void InitializeRigidbody() => m_rigidbody = GetComponent<Rigidbody>();
        protected void InitializeCollider() => m_collider = GetComponent<Collider>();
        
        public override void Show()
        {
            ShowWeapon(true);
        }
        
        public virtual void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);

            if (show)
            {
                //WEAPON SHOW SOUND?
            }
            
            isActive = show;
        }

        public bool HandleWeaponShoot()
        {
            switch (shootType)
            {
                case WeaponShootType.Manual:
                    return TryShoot();
                case WeaponShootType.Automatic:
                    return TryShoot();
                default:
                    return false;
                
            }
        }

        protected override void OnUse()
        {
            HandleWeaponShoot();
        }

        public override bool IsReadyToUse() => (GetCurrentAmmo() >= 0f && CooldownPassed());
        
        public bool CooldownPassed() => m_lastTimeShot + delayBetweenShots < Time.time;
        
        protected bool TryShoot()
        {
            if(!CooldownPassed()) return false;
            
            isThrown = false;
            //Debug.Log("TryShoot");
            if (GetCurrentAmmo() > 0f)
            {
                HandleShoot();
                m_currentAmmo--;
            }
            else
            {
                isThrown = true;
            }
            return true;
        }
        
        protected void HandleShoot()
        {
            //Debug.Log("HandleShoot");
            int bulletsPerShotFinal = bulletsPerShot; // CAMBIAR SI PONEMOS ARMA QUE SE TENGA QUE CARGAR
            
            for (int i = 0; i < bulletsPerShotFinal; i++)
            {
                Vector3 shotDirection = GetShootDirectionWithinSpread(weaponMuzzle);
                Vector3 shotOrigin;
                if (GetComponentInParent<EnemyProtScript>() == null)
                {
                    shotOrigin = SpawnProjectilePosition();
                }
                else
                {
                    shotOrigin = m_shootOrigin.position + m_shootOrigin.transform.forward * m_shootOffset;;
                }
                //Vector3 shotOrigin = m_shootOrigin.position + m_shootOrigin.transform.forward * m_shootOffset;
                GameObject projectile = Instantiate(projectilePrefab, shotOrigin, Quaternion.LookRotation(shotDirection));
                //Logica de lanzamiento si es necesaria (si hacemos que las balas se muevan por update no sería necesario)
            }
            
            m_lastTimeShot = Time.time;
            
            //Muzzle flash
            // if (MuzzleFlashPrefab != null)
            // {
            //     GameObject muzzleFlashInstance = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position,
            //         WeaponMuzzle.rotation, WeaponMuzzle.transform);
            //     // Unparent the muzzleFlashInstance
            //     if (UnparentMuzzleFlash)
            //     {
            //         muzzleFlashInstance.transform.SetParent(null);
            //     }
            //
            //     Destroy(muzzleFlashInstance, 2f);
            // }
            
            //Sound
            
            //For the recoil, sound and things like that
            OnShoot?.Invoke();
            //GetComponentInChildren<ParticleSystem>().Play();
            OnShootProcessed?.Invoke();
        }
        
        public Vector3 GetShootDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRatio = bulletSpreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
                spreadAngleRatio);
            return spreadWorldDirection;
        }

        public void SetShootOriginAndOffset(bool enemy)
        {
            m_shootOrigin = weaponMuzzle; //enemy ? weaponMuzzle : Camera.main.transform;
            m_shootOffset = enemy ? 0f : FindObjectOfType<Player>().radius;
        }
        
        protected Vector3 SpawnProjectilePosition()
        {
            return Camera.main.transform.position + (Camera.main.transform.forward * 0.5f) + (Camera.main.transform.up * -0.05f);
        }
        
        // public void PickUpWeapon(Transform slot)
        // {
        //     ChangePhysicsSettings(false);
        //     transform.parent = slot;
        //     
        //     transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
        //     transform.DOLocalRotate(Vector3.zero, 0.25f).SetUpdate(true);
        //     
        //     //transform.localPosition = Vector3.zero + offset;
        // }
        
        // public void ThrowWeapon()
        // {
        //     isThrown = true;
        //     Sequence throwSequence = DOTween.Sequence();
        //     throwSequence.Append(transform.DOMove(transform.position - transform.forward, throwWeaponDetachDuration)).SetUpdate(true);
        //     throwSequence.AppendCallback(() => transform.parent = null);
        //     throwSequence.AppendCallback(() =>
        //     transform.position = Camera.main.transform.position + (Camera.main.transform.right * FindObjectOfType<Player>().radius));// +(FindObjectOfType<Player>().radius * Camera.main.transform.forward)
        //     throwSequence.AppendCallback(() => ChangePhysicsSettings(true));
        //     throwSequence.AppendCallback(() => m_rigidbody.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse));
        //     throwSequence.AppendCallback(() => m_rigidbody.AddTorque(-Camera.main.transform.up * throwTorque, ForceMode.Impulse));
        //     throwSequence.AppendCallback(() => isThrown = false);
        //
        // }

        // public void ChangePhysicsSettings(bool detach = true)
        // {
        //     if(transform.parent != null)
        //         return;
        //     m_rigidbody.isKinematic = !detach;
        //     m_rigidbody.interpolation = detach ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        //     m_collider.isTrigger = !detach;
        // }
        
        protected override IEnumerator SpecialAction()
        {
            throw new System.NotImplementedException();
        }

        public override void ItemInitializations()
        {
            InitializeWeaponMuzzle();
            m_currentAmmo = maxAmmo;
            InitializeRigidbody();
            InitializeCollider();
        }

        protected void Awake()
        {
            ItemInitializations();
        }

        protected void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(GameTags.Enemy) && collision.relativeVelocity.magnitude < 15)
            {
                BodyRagdollPart bpPart = collision.gameObject.GetComponent<BodyRagdollPart>();

                if (!bpPart.enemyOwner.dead)
                    //Instantiate(SuperHotScript.instance.hitParticlePrefab, transform.position, transform.rotation);

                //bpPart.HidePartAndReplace();
                bpPart.enemyOwner.Ragdoll();
            }
        }
    }
}