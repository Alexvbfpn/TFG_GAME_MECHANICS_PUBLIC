using System.Collections.Generic;

namespace GameMechanics.EntitiesSystem
{
    public class PlayerStateManager : EntityStateManager<Player>
    {
        [ClassTypeName(typeof(PlayerState))]
        public string[] states;
        protected override List<EntityState<Player>> GetStateList()
        {
            return PlayerState.CreateListFromStringArray(states);
        }
    }
}