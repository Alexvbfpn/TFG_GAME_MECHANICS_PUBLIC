using System.Collections.Generic;
using Items.Weapons.Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace GameMechanics.EntitiesSystem
{
    public class PlayerWeaponsManager : MonoBehaviour
    {
        public enum WeaponSwitchState
        {
            Up,
            Down,
            PutDownPrevious,
            PutUpNew,
        }
        
        [Tooltip("List of weapons the player will start with")]
        public List<WeaponController> startWeapons = new List<WeaponController>();
        
        [Header("References")] [Tooltip("Secondary camera used to avoid seeing weapon go throw geometries")]
        public Camera weaponCamera;
        
        [Tooltip("Parent transform where all weapon will be added in the hierarchy")]
        public Transform weaponParentSocket;

        [Tooltip("Position for weapons when active but not actively aiming")]
        public Transform defaultWeaponPosition;

        [Tooltip("Position for weapons when aiming")]
        public Transform aimingWeaponPosition;

        [Tooltip("Position for innactive weapons")]
        public Transform downWeaponPosition;

        #region -- WEAPON BOB --
        
        [Header("Weapon Bob")]
        [Tooltip("Frequency at which the weapon will move around in the screen when the player is in movement")]
        public float bobFrequency = 10f;
        
        [Tooltip("How fast the weapon bob is applied, the bigger value the fastest")]
        public float bobSharpness = 10f;
        
        [Tooltip("Distance the weapon will move when not aiming")]
        public float defaultBobAmount = 0.05f;
        
        [Tooltip("Distance the weapon bobs when aiming")]
        public float aimingBobAmount = 0.02f;
        
        #endregion

        #region -- WEAPON RECOIL --
        [Header("Weapon Recoil")]
        [Tooltip("This will affect how fast the recoil moves the weapon, the bigger the value, the fastest")]
        public float recoilSharpness = 50f;
        
        [Tooltip("Maximum distance the recoil can affect the weapon")]
        public float maxRecoilDistance = 0.5f;
        
        [Tooltip("How fast the weapon goes back to it's original position after the recoil is finished")]
        public float recoilRestitutionSharpness = 10f;

        #endregion
        
        [Header("Misc")] [Tooltip("Speed at which the aiming animatoin is played")]
        public float aimingAnimationSpeed = 10f;
        //YA VEREMOS SI NO SE SEPARA ESTO
        [Tooltip("Field of view when not aiming")]
        public float defaultFov = 60f;
        
        [Tooltip("Portion of the regular FOV to apply to the weapon camera")]
        public float weaponFovMultiplier = 1f;

        [Tooltip("Delay before switching weapon a second time, to avoid recieving multiple inputs from mouse wheel")]
        public float weaponSwitchDelay = 0.5f;

        public float weaponPickupRange = 10f;
        
        [Tooltip("Layer to set FPS weapon gameObjects to")]
        public LayerMask fpsWeaponLayer;
        
        public int activeItemIndex { get; protected set; }
        public bool isAiming { get; protected set; }
        public bool isPointingAtEnemy { get; set; }
        
        public UnityAction<WeaponController> OnReloadAmmoWeapon;

        public UnityAction<WeaponController> OnSwitchedToWeapon;
        public UnityAction<WeaponController> OnAddedWeapon;
        public UnityAction<WeaponController> OnRemovedWeapon;
        
        protected Player m_player;
        
        protected WeaponController[] m_weaponSlots = new WeaponController[1];
        protected WeaponController m_CurrentWeapon;
        protected float m_weaponBobFactor;
        
        protected Vector3 m_lastCharacterPosition;
        protected Vector3 m_weaponMainLocalPosition;
        protected Vector3 m_weaponBobLocalPosition;
        protected Vector3 m_weaponRecoilLocalPosition;
        protected Vector3 m_accumulatedRecoil;
        
        protected float m_timeStartedWeaponSwitch;
        protected WeaponSwitchState m_weaponSwitchState;
        protected int m_weaponSwitchNewWeaponIndex;

        protected virtual void InitializePlayer() => m_player = FindObjectOfType<Player>();
        
        public virtual void SetFOV(float fov)
        {
            Camera.main.fieldOfView = fov;
            weaponCamera.fieldOfView = fov * weaponFovMultiplier;
        }
        
        protected void ShootCurrentWeapon()
        {
            CurrentWeaponShootHandling();
        }
        
        protected void CurrentWeaponShootHandling()
        {
            if (m_CurrentWeapon == null) return;
            
            bool hasFired = m_CurrentWeapon.TryUse();
            bool hasThrown = m_CurrentWeapon.isThrown;
            if (hasFired && !hasThrown)
            {
                m_accumulatedRecoil += Vector3.back * m_CurrentWeapon.recoilForce;
                m_accumulatedRecoil = Vector3.ClampMagnitude(m_accumulatedRecoil, maxRecoilDistance);
            }

            if (hasFired && hasThrown)
            {
                ThrowCurrentWeapon();
            }

        }

        public virtual void ThrowCurrentWeapon()
        {
            if (m_CurrentWeapon == null) return;
            
            (m_player as FirstPersonPlayer).Throw();
            m_CurrentWeapon = null;
        }

        protected void WeaponSwitchHandling()
        {
            Debug.Log("Cambiando arma");
        }
        
        protected void PointingAtEnemyHandling()
        {
            // if(m_CurrentWeapon == null) return;
            // isPointingAtEnemy = false;
            
            //if(Physics.Raycast(weaponCamera.transform.position, weaponCamera.transform.forward, out RaycastHit hit, m_CurrentWeapon.stats.current.range, m_CurrentWeapon.stats.current.hitLayer)) GameLayers.GetLayerMask(GameLayers.Enemy)
            // if(Physics.Raycast(weaponCamera.transform.position, weaponCamera.transform.forward, out RaycastHit hit, 1000, -1, QueryTriggerInteraction.Ignore))
            // {
            //     if (hit.transform.TryGetComponent(out IDamageable damageable))
            //     {
            //         isPointingAtEnemy = true;
            //     }
            //
            //     // if (hit.transform.TryGetComponent(out WeaponController pickableWeapon))
            //     // {
            //     //     Debug.Log("Estoy apuntando a un objeto que puedo recoger");
            //     //     if (m_player.playerInputs.GetInteractDown())
            //     //     {
            //     //         m_CurrentWeapon = pickableWeapon;
            //     //     }
            //     // }
            // }
        }
        
        public void AddWeapon(WeaponController weapon)
        {
            if (weapon == null) return;
            
            // weapon.transform.SetParent(weaponParentSocket);
            // weapon.transform.localPosition = Vector3.zero;
            // weapon.transform.localRotation = Quaternion.identity;
            // weapon.gameObject.layer = fpsWeaponLayer;
            // weapon.gameObject.SetActive(false);
            //
            //m_weaponSlots[0] = weapon;
            m_CurrentWeapon = weapon;
            OnAddedWeapon?.Invoke(weapon);
            OnSwitchedToWeapon?.Invoke(weapon);
        }
        
        public void RemoveCurrentWeapon()
        {
            Debug.Log("Removing weapon");
            m_CurrentWeapon = null;
            OnRemovedWeapon?.Invoke(m_CurrentWeapon);
            OnSwitchedToWeapon?.Invoke(null);
        }
        protected void OnWeaponSwitched(WeaponController newWeapon)
        {
            if (newWeapon != null)
            {
                newWeapon.Show();
            }
        }

        public WeaponController GetActiveWeapon() => m_CurrentWeapon;
        

        public void SetActiveWeapon(WeaponController weapon)
        {
            if (weapon == null) return;
            
            m_CurrentWeapon = weapon;
            OnSwitchedToWeapon?.Invoke(weapon);
        }
        
        
        protected void UpdateWeaponAiming()
        {
            WeaponController activeWeapon = GetActiveWeapon();
            if (isAiming && activeWeapon)
            {
                m_weaponMainLocalPosition = Vector3.Lerp(m_weaponMainLocalPosition,
                    aimingWeaponPosition.localPosition + activeWeapon.aimOffset,
                    aimingAnimationSpeed * Time.deltaTime);
                SetFOV(Mathf.Lerp(Camera.main.fieldOfView,
                    activeWeapon.aimZoomRatio * defaultFov, aimingAnimationSpeed * Time.deltaTime));
            }
            else
            {
                m_weaponMainLocalPosition = Vector3.Lerp(m_weaponMainLocalPosition,
                    defaultWeaponPosition.localPosition, aimingAnimationSpeed * Time.deltaTime);
                SetFOV(Mathf.Lerp(Camera.main.fieldOfView, defaultFov,
                    aimingAnimationSpeed * Time.deltaTime));
            }
            // if (m_weaponSwitchState == WeaponSwitchState.Up)
            // {
            //     WeaponController activeWeapon = GetActiveWeapon();
            //     if (isAiming && activeWeapon)
            //     {
            //         m_weaponMainLocalPosition = Vector3.Lerp(m_weaponMainLocalPosition,
            //             aimingWeaponPosition.localPosition + activeWeapon.aimOffset,
            //             aimingAnimationSpeed * Time.deltaTime);
            //         SetFOV(Mathf.Lerp(Camera.main.fieldOfView,
            //             activeWeapon.aimZoomRatio * defaultFov, aimingAnimationSpeed * Time.deltaTime));
            //     }
            //     else
            //     {
            //         m_weaponMainLocalPosition = Vector3.Lerp(m_weaponMainLocalPosition,
            //             defaultWeaponPosition.localPosition, aimingAnimationSpeed * Time.deltaTime);
            //         SetFOV(Mathf.Lerp(Camera.main.fieldOfView, defaultFov,
            //             aimingAnimationSpeed * Time.deltaTime));
            //     }
            // }
        }
        
        protected void UpdateWeaponBob()
        {
            if (m_CurrentWeapon == null) return;
            
            if (Time.deltaTime > 0f)
            {
                Vector3 playerCharacterVelocity =
                    (m_player.position - m_player.lastPosition) / Time.deltaTime;

                // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
                float characterMovementFactor = 0f;
                if (m_player.isGrounded)
                {
                    characterMovementFactor =
                        Mathf.Clamp01(playerCharacterVelocity.magnitude /
                                      (m_player.stats.current.topSpeed *
                                       (m_player.stats.current.runningTopSpeed / m_player.stats.current.topSpeed)));
                }

                m_weaponBobFactor =
                    Mathf.Lerp(m_weaponBobFactor, characterMovementFactor, bobSharpness * Time.deltaTime);

                // Calculate vertical and horizontal weapon bob values based on a sine function
                float bobAmount = isAiming ? aimingBobAmount : defaultBobAmount;
                float frequency = bobFrequency;
                float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * m_weaponBobFactor;
                float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount *
                                  m_weaponBobFactor;

                // Apply weapon bob
                m_weaponBobLocalPosition.x = hBobValue;
                m_weaponBobLocalPosition.y = Mathf.Abs(vBobValue);

                m_lastCharacterPosition = m_player.lastPosition;
            }
        }
        
        protected void UpdateWeaponRecoil()
        {
            // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
            if (m_weaponRecoilLocalPosition.z >= m_accumulatedRecoil.z * 0.99f)
            {
                m_weaponRecoilLocalPosition = Vector3.Lerp(m_weaponRecoilLocalPosition, m_accumulatedRecoil,
                    recoilSharpness * Time.deltaTime);
            }
            // otherwise, move recoil position to make it recover towards its resting pose
            else
            {
                m_weaponRecoilLocalPosition = Vector3.Lerp(m_weaponRecoilLocalPosition, Vector3.zero,
                    recoilRestitutionSharpness * Time.deltaTime);
                m_accumulatedRecoil = m_weaponRecoilLocalPosition;
            }
        }
        
        protected void SetupWeapons()
        {
            activeItemIndex = 0;
            m_CurrentWeapon.ChangeActiveState(true);
            OnSwitchedToWeapon?.Invoke(m_CurrentWeapon);
            //GetActiveItem().MoveToDefaultPosition();
        }

        protected void InitializeStartingWeapons()
        {
            if (startWeapons.Count == 0) return;
            
            m_CurrentWeapon = startWeapons[0];
            
            SetupWeapons();
            // foreach (WeaponController weapon in startWeapons)
            // {
            //     AddWeapon(weapon);
            // }
        }
        protected void Awake()
        {
            InitializePlayer();
            m_player.events.onAim.AddListener((x) => ThrowCurrentWeapon());
            //m_player.events.onStopAim.AddListener(() => ChangeAimingState(false));
            m_player.events.onShoot.AddListener(() => ShootCurrentWeapon());
        }
        
        protected void Start()
        {
            m_weaponMainLocalPosition = defaultWeaponPosition.position;
            activeItemIndex = -1;
            m_weaponSwitchState = WeaponSwitchState.Down;
            
            SetFOV(defaultFov);
            InitializeStartingWeapons();
        }
        
        protected void Update()
        {
            //PointingAtEnemyHandling();
        }

        protected void LateUpdate()
        {
            UpdateWeaponAiming();
            UpdateWeaponBob();
            UpdateWeaponRecoil();
            
            weaponParentSocket.localPosition = m_weaponMainLocalPosition + m_weaponBobLocalPosition + m_weaponRecoilLocalPosition;
        }
    }
}