using System.Collections;
using GameMechanics;
using Misc;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [AddComponentMenu("Mechanics/Level/Level Starter")]
    public class LevelStarter : Singleton<LevelStarter>
    {
        /// <summary>
        /// Called when the starter routine has endend.
        /// </summary>
        public UnityEvent onLevelStart;
        
        public float enablePlayerDelay = 1f;
        
        protected Level m_level => Level.instance;
        protected LevelPauser m_pauser => LevelPauser.instance;

        protected virtual IEnumerator Routine()
        {
            Game.LockCursor();
            m_level.player.controller.enabled = false;
            m_level.player.SetInputEnabled(false);
            yield return new WaitForSeconds(enablePlayerDelay);
            m_level.player.controller.enabled = true;
            m_level.player.SetInputEnabled(true);
            m_pauser.canPause = true;
            onLevelStart?.Invoke();
        }

        protected void Start()
        {
            StartCoroutine(Routine());
        }
    }
}