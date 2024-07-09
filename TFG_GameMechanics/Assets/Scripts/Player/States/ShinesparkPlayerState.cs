using System.Collections;
using GameMechanics.EntitiesSystem.PlatformerPlayerLogic;
using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class ShinesparkPlayerState : PlayerState
    {
        PlatformerPlayer _player;
        protected override void OnEnter(Player player)
        {
            _player = (PlatformerPlayer) player;
            _player.topSpeedMultiplier = 3;
        }

        protected override void OnExit(Player player)
        {
            _player.topSpeedMultiplier = 1;
        }

        protected override void OnStep(Player player)
        {
            player.Accelerate(_player.dashDirection);
            
            player.velocity = new Vector3(_player.dashDirection.x * player.stats.current.topSpeed * player.topSpeedMultiplier, 
                _player.dashDirection.y * player.stats.current.topSpeed * player.topSpeedMultiplier);
            Logging.Log("velocity: " + player.velocity);
        }

        public override void OnContact(Player entity, Collider other)
        {
            Logging.Log("Contactando con " + other.name + " con layer " + other.gameObject.layer);
            if (other.gameObject.layer == 11 || other.gameObject.layer == 17)
            {
                _player.velocity = Vector3.zero;
                _player.platformerEvents.onImpactSideSet.Invoke(_player.impactSide);
                _player.FinishShinesparkDash();
            }
        }
        
       
    }
}