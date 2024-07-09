using System.Collections.Generic;
using Misc;
using UnityEngine;

namespace GameMechanics
{
    [AddComponentMenu("Mechanics/Game/Game")]
    public class Game : Singleton<Game>
    {
        public List<GameLevel> levels;

        /// <summary>
        /// Sets the cursor lock and hide state
        /// </summary>
        /// <param name="value">If true, the cursor will be hidden.</param>
        public static void LockCursor(bool value = true)
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            Cursor.visible = !value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
#endif
        }
        /// <summary>
        /// Returns a Game Level if the current scene is a Level. If its not, return null.
        /// </summary>
        public virtual GameLevel GetCurrentLevel()
        {
            var scene = GameSceneLoader.instance.currentScene;
            return levels.Find((level) => level.scene == scene);
        }

        /// <summary>
        /// Returns the index from the levels list of the current scene.
        /// </summary>
        /// <returns></returns>
        public virtual int GetCurrentLevelIndex()
        {
            var scene = GameSceneLoader.instance.currentScene;
            return levels.FindIndex((level) => level.scene == scene);
        }

        /// <summary>
        /// Unlocks a given Game Level by its scene name.
        /// </summary>
        /// <param name="sceneName">The scene name of the level you want to unlock.</param>
        public virtual void UnlockLevelBySceneName(string sceneName)
        {
            var level = levels.Find((level) => level.scene == sceneName);

            if (level != null)
            {
                level.locked = false;
            }
        }
        
        // <summary>
        /// Unlocks the next level from the levels list.
        /// </summary>
        public virtual void UnlockNextLevel()
        {
            var index = GetCurrentLevelIndex() + 1;

            if (index >= 0 && index < levels.Count)
            {
                levels[index].locked = false;
            }
        }
        
        /// <summary>
        /// Locks a given Game Level by its scene name.
        /// </summary>
        /// <param name="sceneName">The scene name of the level you want to unlock.</param>
        public virtual void LockLevelBySceneName(string sceneName)
        {
            var level = levels.Find((level) => level.scene == sceneName);

            if (level != null)
            {
                level.locked = true;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}