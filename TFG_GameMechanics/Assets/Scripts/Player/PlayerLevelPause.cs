using Level;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    [RequireComponent(typeof(Player))]
    public class PlayerLevelPause : MonoBehaviour
    {
        protected Player m_player;
        protected LevelPauser m_pauser;
        
        protected virtual void Start()
        {
            m_player = GetComponent<Player>();
            m_pauser = LevelPauser.instance;
        }
        
        protected virtual void Update()
        {
            var pausing = m_player.playerInputs.GetPauseDown() ||
                m_player.playerInputs.EscapeKeyPressed();

            if (pausing && Time.unscaledTime != m_pauser.lastToggleTime)
            {
                var value = m_pauser.isPaused;
                m_pauser.Pause(!value);
            }
        }
    }
}