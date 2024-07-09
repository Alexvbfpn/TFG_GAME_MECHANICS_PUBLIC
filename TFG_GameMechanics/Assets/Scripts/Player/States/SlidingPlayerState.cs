using GameMechanics.EntitiesSystem.PlatformerPlayerLogic;
using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class SlidingPlayerState : PlayerState
    {
        public Vector3 initialVelocity;
        protected override void OnEnter(Player player)
        {
            initialVelocity = player.lateralVelocity.normalized;
            Logging.Log("InitialVelocity: " + initialVelocity);
            player.ResizeCollider(player.stats.current.slideHeight);
            
            PlatformerPlayer platformerPlayer = player as PlatformerPlayer;
            if (!platformerPlayer.isBoosting)
            {
                platformerPlayer.platformerEvents.onUnchargingBoost.Invoke();
            }
        }

        protected override void OnExit(Player player)
        {
            player.ResizeCollider(player.originalHeight);
        }

        protected override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            //player.Jump();
            player.Fall();
            
            player.Accelerate(initialVelocity);
            
            if(timeSinceEntered > player.stats.current.minSlideDuration)
            {
                if(player.canStandUp)
                {
                    player.states.Change<IdlePlayerState>();
                }
            }
        }

        public override void OnContact(Player player, Collider other)
        {
            
        }
    }
}