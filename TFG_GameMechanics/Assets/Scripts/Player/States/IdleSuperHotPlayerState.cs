using DG.Tweening;
using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class IdleSuperHotPlayerState : IdlePlayerState
    {
        private float targetTimeScale;
        private float lerpTime;
        private float finalTargetTimeScale;
        private float finalLerpTime;
        protected override void OnEnter(Player player)
        {
            targetTimeScale = (player as FirstPersonArmedPlayer).idleTimeScale;
            lerpTime = (player as FirstPersonArmedPlayer).idleLerpTime;
        }

        protected override void OnStep(Player player)
        {
            base.OnStep(player);
            
            finalTargetTimeScale = (player as FirstPersonArmedPlayer).doingAction ? 1f : targetTimeScale;
            finalLerpTime = (player as FirstPersonArmedPlayer).doingAction ? 0.1f : lerpTime;
            
            Time.timeScale = Mathf.Lerp(Time.timeScale, finalTargetTimeScale, finalLerpTime);
            
        }

        protected override void OnExit(Player player) { }
        
        
    }
}