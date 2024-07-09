using UnityEngine;

namespace GameMechanics.EntitiesSystem.GowPlayer
{
    public class ArmedPlayerAnimator : PlayerAnimator<ArmedPlayer>
    {

        protected override void InitializeAnimatorTriggers()
        {
            base.InitializeAnimatorTriggers();
            m_player.onWeaponCatch.AddListener(() => animator.SetTrigger(m_onWeaponCatchHash));
        }
        
        protected override void HandleAnimatorParameters()
        {
            base.HandleAnimatorParameters();
           
            animator.SetBool(m_isHoldingHash, m_player.isHoldingWeapon);
            animator.SetBool(m_isPullingHash, m_player.isPulling);
            
        }
    }
}