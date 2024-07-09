using System.Collections;
using GameMechanics;
using Misc;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [AddComponentMenu("Mechanics/Level/Level Respawner")]
    public class LevelRespawner : Singleton<LevelRespawner>
    {
        /// <summary>
        /// Called after the respawn routine has finished.
        /// </summary>
        public UnityEvent onRespawn;
        
        public float respawnFadeOutDelay = 1f;
        public float respawnFadeInDelay = 0.5f;
        public float restartFadeOutDelay = 0.5f;
        
        protected Level m_level => Level.instance;
        protected LevelPauser m_pauser => LevelPauser.instance;
        protected Fader m_fader => Fader.instance;

        protected virtual IEnumerator RespawnRoutine()
        {
            m_level.player.Respawn();
            FreezeCameras();
            
            ResetCameras();
            FreezeCameras(false);
            onRespawn?.Invoke();
            yield return new WaitForSeconds(respawnFadeInDelay);
            
            //FADER
            m_fader.FadeIn(() =>
            {
                m_level.player.SetInputEnabled(true);
                m_pauser.canPause = true;
            });
        }
        
        protected virtual IEnumerator RestartRoutine()
        {
            m_pauser.Pause(false);
            m_pauser.canPause = false;
            m_level.player.SetInputEnabled(false);

            yield return new WaitForSeconds(restartFadeOutDelay);
            m_fader.FadeOut(() =>
            {
                GameSceneLoader.instance.Reload();
                m_fader.FadeIn();
            });
            //GameSceneLoader.instance.Reload();
        }

        protected virtual IEnumerator Routine()
        {
            m_pauser.Pause(false);
            m_pauser.canPause = false;
            m_level.player.SetInputEnabled(false);
            
            yield return new WaitForSeconds(respawnFadeOutDelay);
            
            //FADER Y RESPAWNROUTINE
            m_fader.FadeOut(() =>
            {
                StartCoroutine(RespawnRoutine());
            });
        }
        
        protected virtual void ResetCameras()
        {
            Debug.Log("TODO: IMPLEMENT ResetCameras");
            // foreach (var camera in m_cameras)
            // {
            //     camera?.Reset();
            // }
        }
        
        protected virtual void FreezeCameras(bool value = true)
        {
            Debug.Log("TODO: IMPLEMENT FreezeCameras");
            // foreach (var camera in m_cameras)
            // {
            //     if (camera)
            //         camera.freeze = value;
            // }
        }
        
        /// <summary>
        /// Invokes either Respawn routine.
        /// </summary>
        public virtual void Respawn()
        {
            StopAllCoroutines();
            StartCoroutine(Routine());
        }
        
        /// <summary>
        /// Restarts the current Level loading the scene again.
        /// </summary>
        public virtual void Restart()
        {
            StopAllCoroutines();
            StartCoroutine(RestartRoutine());
        }

        protected void Start()
        {
            //InitializeCameras()
            //PlayerEventsToRespawn()
        }
    }
}