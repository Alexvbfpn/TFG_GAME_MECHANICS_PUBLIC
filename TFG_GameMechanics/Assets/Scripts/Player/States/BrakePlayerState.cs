using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class BrakePlayerState : PlayerState
    {
        protected override void OnEnter(Player player)
        {
            
        }

        protected override void OnExit(Player player)
        {
            
        }

        protected override void OnStep(Player player)
        {
            player.SnapToGround();
            player.Jump();
            player.Fall();
            player.Decelerate();
            
            if (player.lateralVelocity.sqrMagnitude == 0)
            {
                player.states.Change<IdlePlayerState>();
            }
        }

        public override void OnContact(Player player, Collider other)
        {
            
        }
    }
}