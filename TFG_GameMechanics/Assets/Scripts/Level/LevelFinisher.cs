using System.Collections;
using GameMechanics;
using Misc;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [AddComponentMenu("Mechanics/Level/Level Finisher")]
    public class LevelFinisher : Singleton<LevelFinisher>
    {
        /// <summary>
        /// Called when the Level is finished.
        /// </summary>
        public UnityEvent onFinish;
        
        /// <summary>
        /// Called when exit from the level.
        /// </summary>
        public UnityEvent onExit;
        
        public bool unlockNextLevel;

        [Tooltip("The scene to load when this level is finished. If its a level, its 'locking' properties are ignored.")]
        public string nextScene;
        
        [Tooltip("The scene to load when exiting this level.")]
        public string exitScene;
        
        public float loadingDelay = 1f;
        
        protected Game m_game => Game.instance;
        protected GameSceneLoader m_loader => GameSceneLoader.instance;
        protected Level m_level => Level.instance;
        protected LevelPauser m_pauser => LevelPauser.instance;

        protected virtual IEnumerator FinishRoutine()
        {
            m_pauser.Pause(false);
            m_pauser.canPause = false;
            m_level.isFinished = true;
            //m_level.player.canTakeDamage = false;
            m_level.player.SetInputEnabled(false);

            yield return new WaitForSeconds(loadingDelay);
            
            if (unlockNextLevel)
            {
                m_game.UnlockNextLevel();
            }
            
            Game.LockCursor(false);
            m_loader.Load(nextScene);
            onFinish?.Invoke();
        }
        
        protected virtual IEnumerator ExitRoutine()
        {
            m_pauser.Pause(false);
            m_pauser.canPause = false;
            m_level.player.SetInputEnabled(false);

            yield return new WaitForSeconds(loadingDelay);
            
            Game.LockCursor(false);
            m_loader.Load(exitScene);
            onExit?.Invoke();
        }
        
        /// <summary>
        /// Invokrs the Level Finisher routine to load the next scene.
        /// </summary>
        public virtual void Finish()
        {
            StopAllCoroutines();
            StartCoroutine(FinishRoutine());
        }
        
        /// <summary>
        /// Invokes the Level exit routine.
        /// </summary>
        public virtual void Exit()
        {
            StopAllCoroutines();
            StartCoroutine(ExitRoutine());
        }
    }
}