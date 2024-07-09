using UnityEngine;

namespace GameMechanics.EntitiesSystem.States
{
    public class FloatingBeforeShinesparkPlayerState : PlayerState
    {
        protected override void OnEnter(Player player)
        {
            player.transform.position = player.position + new Vector3(0, 0.5f, 0);
        }

        protected override void OnExit(Player player)
        {
            
        }

        protected override void OnStep(Player player)
        {
            
        }

        public override void OnContact(Player entity, Collider other)
        {
            throw new System.NotImplementedException();
        }
    }
}