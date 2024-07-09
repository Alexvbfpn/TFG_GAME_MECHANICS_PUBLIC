using UnityEngine;

namespace GameMechanics.EntitiesSystem.PlatformerPlayerLogic
{
    public class PlatformerPlayerAnimator : PlayerAnimator<PlatformerPlayer>
    {
        public string isBoosting = "Is Boosting";
        public string dashAngle = "Dash Angle";
        public string impactSide = "Impact Side";
        
        protected int m_isBoostingHash;
        protected int m_dashAngleHash;
        protected int m_impactSideHash;
        
        protected override void InitializeParametersHash()
        {
            base.InitializeParametersHash();
            m_isBoostingHash = Animator.StringToHash(isBoosting);
            m_dashAngleHash = Animator.StringToHash(dashAngle);
            m_impactSideHash = Animator.StringToHash(impactSide);
        }
        
        protected override void InitializeAnimatorTriggers()
        {
            base.InitializeAnimatorTriggers();
            m_player.platformerEvents.onBoost.AddListener((x) => animator.SetBool(m_isBoostingHash, x));
            
            m_player.platformerEvents.onDashAngleSet.AddListener((x) => animator.SetFloat(m_dashAngleHash, x));
            m_player.platformerEvents.onImpactSideSet.AddListener((x) => animator.SetInteger(m_impactSideHash, x));
        }
    }
}