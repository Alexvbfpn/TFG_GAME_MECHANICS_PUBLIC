using GameMechanics.EntitiesSystem.GowPlayer;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    [AddComponentMenu("Mechanics/Entities/Player/Player Animator")]
    public class PlayerAnimator<T> : MonoBehaviour where T : Player
    {
        public Animator animator;

        [Header("Parameters Names")]
        public string stateName = "State";
        public string lastStateName = "Last State";
        public string lateralSpeedName = "Lateral Speed";
        public string verticalSpeedName = "Vertical Speed";
        public string lateralAnimationSpeedName = "Lateral Animation Speed";
        public string isGroundedName = "Is Grounded";
        public string isHoldingName = "Is Holding";
        public string onStateChangedName = "On State Changed";
        public string onShoot = "On Shoot";
        public string onPulling = "Pulling";
        public string onWeaponCatch = "Catch";

        [Header("Settings")]
        public float minLateralAnimationSpeed = 0.5f;
        
        protected int m_stateHash;
        protected int m_lastStateHash;
        protected int m_lateralSpeedHash;
        protected int m_verticalSpeedHash;
        protected int m_lateralAnimationSpeedHash;
        protected int m_isGroundedHash;
        protected int m_isHoldingHash;
        protected int m_onStateChangedHash;
        protected int m_onShootHash;
        protected int m_isPullingHash;
        protected int m_onWeaponCatchHash;
        
        protected T m_player;
        
        protected virtual void InitializePlayer()
        {
            m_player = GetComponent<T>();
        }
        
        protected virtual void InitializeAnimatorTriggers()
        {
            m_player.states.events.onChange.AddListener(() => animator.SetTrigger(m_onStateChangedHash));
            m_player.events.onShoot.AddListener(() => animator.SetTrigger(m_onShootHash));
        }
        
        protected virtual void InitializeParametersHash()
        {
            m_stateHash = Animator.StringToHash(stateName);
            m_lastStateHash = Animator.StringToHash(lastStateName);
            m_lateralSpeedHash = Animator.StringToHash(lateralSpeedName);
            m_verticalSpeedHash = Animator.StringToHash(verticalSpeedName);
            m_lateralAnimationSpeedHash = Animator.StringToHash(lateralAnimationSpeedName);
            m_isGroundedHash = Animator.StringToHash(isGroundedName);
            m_isHoldingHash = Animator.StringToHash(isHoldingName);
            m_onStateChangedHash = Animator.StringToHash(onStateChangedName);
            m_onShootHash = Animator.StringToHash(onShoot);
            m_isPullingHash = Animator.StringToHash(onPulling);
            m_onWeaponCatchHash = Animator.StringToHash(onWeaponCatch);
        }
        
        protected virtual void HandleAnimatorParameters()
        {
            var lateralSpeed = m_player.lateralVelocity.magnitude;
            var verticalSpeed = m_player.verticalVelocity.y;
            var lateralAnimationSpeed = Mathf.Max(minLateralAnimationSpeed, lateralSpeed / m_player.stats.current.topSpeed);

            animator.SetInteger(m_stateHash, m_player.states.index);
            animator.SetInteger(m_lastStateHash, m_player.states.lastIndex);
            animator.SetFloat(m_lateralSpeedHash, lateralSpeed);
            animator.SetFloat(m_verticalSpeedHash, verticalSpeed);
            animator.SetFloat(m_lateralAnimationSpeedHash, lateralAnimationSpeed);
            animator.SetBool(m_isGroundedHash, m_player.isGrounded);
        }
        
        protected virtual void Start()
        {
            InitializePlayer();
            InitializeParametersHash();
            InitializeAnimatorTriggers();
        }

        protected virtual void LateUpdate() => HandleAnimatorParameters();
        
    }
}