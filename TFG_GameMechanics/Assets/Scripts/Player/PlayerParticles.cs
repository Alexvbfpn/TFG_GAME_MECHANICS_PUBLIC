using System;
using GameMechanics.EntitiesSystem.GowPlayer;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    public class PlayerParticles<T> : MonoBehaviour where T : Player
    {
        protected T _player;
        protected void InitializePlayer() => _player = GetComponent<T>();
        
        /// <summary>
        /// Start playing a given particle.
        /// </summary>
        /// <param name="particle">The particle you want to play.</param>
        public virtual void Play(ParticleSystem particle)
        {
            if (!particle.isPlaying)
            {
                particle.Play();
            }
        }

        /// <summary>
        /// Stop a given particle.
        /// </summary>
        /// <param name="particle">The particle you want to stop.</param>
        public virtual void Stop(ParticleSystem particle, bool clear = false)
        {
            if (particle.isPlaying)
            {
                var mode = clear ? ParticleSystemStopBehavior.StopEmittingAndClear :
                    ParticleSystemStopBehavior.StopEmitting;
                particle.Stop(true, mode);
            }
        }

        protected virtual void InitializePlayerEvents(){}
        
        protected virtual void Awake()
        {
            InitializePlayer();
        }

        protected virtual void Start()
        {
            InitializePlayerEvents();
        }
    }
}