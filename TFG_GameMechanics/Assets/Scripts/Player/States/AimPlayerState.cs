using GameMechanics.EntitiesSystem.GowPlayer;
using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class AimPlayerState : PlayerState
    {
        ArmedPlayer armedPlayer;
        Vector3 m_skinInitialPosition;
        protected override void OnEnter(Player player)
        {
            armedPlayer = player as ArmedPlayer;
            armedPlayer.aiming = true;
            player.events.onAim?.Invoke(true);
            m_skinInitialPosition = player.skin.localPosition;
        }

        protected override void OnExit(Player player)
        {
            armedPlayer.aiming = false;
            //player.events.onStopAim?.Invoke();
            player.events.onAim?.Invoke(false);
            player.skin.localPosition = m_skinInitialPosition;
            //player.skin.localRotation = m_skinInitialRotation;
            //player.skin.localRotation = Quaternion.Euler(0, player.skin.localRotation.eulerAngles.y, 0);
        }

        protected override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            armedPlayer.Aim();
            armedPlayer.Shoot();
            if (armedPlayer.aiming)
            {
                armedPlayer.AimPlayerRotation();
            }
        }

        public override void OnContact(Player player, Collider other) { }
    }
}