using GameMechanics;
using Misc;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [AddComponentMenu("Mechanics/Level/Level Pauser")]
    public class LevelPauser : Singleton<LevelPauser>
    {
        /// <summary>
        /// Called when the Level is Paused.
        /// </summary>
        public UnityEvent onPause;
        
        /// <summary>
        /// Called when the Level is resumed.
        /// </summary>
        public UnityEvent onUnpause;
        
        //PAUSE SCREEN REFERENCE
        public UIAnimator pauseScreen;
        
        /// <summary>
        /// Returns true if it's possible to pause the Level.
        /// </summary>
        public bool canPause { get; set; }
        
        /// <summary>
        /// Returns true if the Level is paused.
        /// </summary>
        public bool isPaused { get; protected set; }
        
        /// <summary>
        /// Returns the last time the pause state was toggled.
        /// </summary>
        public float lastToggleTime { get; protected set; }

        /// <summary>
        /// Sets the pause state based on a given value.
        /// </summary>
        /// <param name="value">The state you want to set the pause to.</param>
        public virtual void Pause(bool value)
        {
            if(isPaused == value)
                return;
            if (!isPaused)
                Pause();
            else
                Unpause();
            
            lastToggleTime = Time.unscaledTime;
        }

        protected virtual void Pause()
        {
            if(!canPause)
                return;
            
            Game.LockCursor(false);
            isPaused = true;
            Time.timeScale = 0; //SUSTITUIR POR MANAGER DE TIEMPO PARA COSAS COMO SUPERHOT
            //Activar pantalla de pausa
            pauseScreen?.SetActive(true);
            //Mostrar pantalla de pausa
            pauseScreen?.Show();
            onPause?.Invoke();
        }

        protected virtual void Unpause()
        {
            Game.LockCursor();
            isPaused = false;
            Time.timeScale = 1; //SUSTITUIR POR MANAGER DE TIEMPO PARA COSAS COMO SUPERHOT
            //Esconder pantalla de pausa
            pauseScreen?.Hide();
            onUnpause?.Invoke();
        }
    }
}