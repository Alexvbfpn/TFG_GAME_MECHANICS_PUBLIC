using GameMechanics.EntitiesSystem.PlatformerPlayerLogic;
using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class FallPlayerState : PlayerState
    {
        protected override void OnEnter(Player player)
        {
            PlatformerPlayer platformerPlayer = player as PlatformerPlayer;
            if (!platformerPlayer.isBoosting)
            {
                platformerPlayer.platformerEvents.onUnchargingBoost.Invoke();
            }
        }

        protected override void OnExit(Player player)
        {
        }

        protected override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            player.FaceDirectionSmooth(player.lateralVelocity);
            player.AccelerateToInputDirection();
            player.Jump();
            player.LedgeGrab();
            
            if (player.isGrounded)
            {
                player.states.Change(0);
            }
        }

        public override void OnContact(Player player, Collider other)
        {
            player.WallDrag(other);
        }
    }
}