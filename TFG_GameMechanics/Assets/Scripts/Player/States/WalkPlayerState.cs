using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class WalkPlayerState: PlayerState
    {
        protected override void OnEnter(Player player)
        {
          
        }

        protected override void OnExit(Player player)
        {
          
        }

        protected override void OnStep(Player player)
        {
            var inputDirection = player.playerInputs.GetMovementCameraDirection();
            player.ApplyGravity();
            
            player.SnapToGround();
            player.Jump();
            player.Fall();
          
            if (inputDirection.sqrMagnitude > 0)
            {
                var dot = Vector3.Dot(inputDirection, player.lateralVelocity); // Difference between input direction and current velocity

                //PONER FUNCION DE ACELERAR O DE MOVIMIENTO AL MENOS
                
                if (dot >= player.stats.current.brakeThreshold)
                {
                    player.Accelerate(inputDirection);
                    player.FaceDirectionSmooth(player.lateralVelocity);
                }
                else if (player.states.ContainsStateOfType(typeof(BrakePlayerState)))
                {
                    player.states.Change<BrakePlayerState>();
                }
                // player.Accelerate(inputDirection);
                // player.FaceDirectionSmooth(player.lateralVelocity);
            }
            else
            {
                player.ApplyFriction();
                if (player.lateralVelocity.sqrMagnitude <= 0)
                {
                    player.states.Change(0);
                }
            }

            player.WalkStepSpecificLogic();
        }

        public override void OnContact(Player player, Collider other)
        {
           
        }
    }
}