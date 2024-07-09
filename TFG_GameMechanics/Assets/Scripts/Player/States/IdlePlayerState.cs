using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class IdlePlayerState : PlayerState
    {
        protected override void OnEnter(Player player)
        {
        }

        protected override void OnExit(Player player) { }

        protected override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            player.Jump();
            player.Fall();
            
            player.IdleStepSpecificLogic();
            
            var inputDirection = player.playerInputs.GetMovementDirection();

            if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude > 0)
            {
                player.states.Change(1);
            }
        }

        public override void OnContact(Player player, Collider other) { }
    }
}