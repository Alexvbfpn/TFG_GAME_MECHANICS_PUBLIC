using DG.Tweening;
using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class WalkSuperhotPlayerState : WalkPlayerState
    {
        private float targetTimeScale;
        private float lerpTime;
        private float finalTargetTimeScale;
        private float finalLerpTime;
        protected override void OnEnter(Player player)
        {
            targetTimeScale = (player as FirstPersonArmedPlayer).movementTimeScale;
            lerpTime = (player as FirstPersonArmedPlayer).movementLerpTime;
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