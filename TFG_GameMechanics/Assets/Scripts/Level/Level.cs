using GameMechanics.EntitiesSystem;
using GameMechanics.EntitiesSystem.PlayerCameras;
using Misc;
using UnityEngine;

namespace Level
{
    [AddComponentMenu("Mechanics/Level/Level")]
    public class Level : Singleton<Level>
    {
        protected Player _player;
        protected PlayerCameraThirdPerson _camera;
        
        /// <summary>
        /// Returns true if the Level has been finished.
        /// </summary>
        public bool isFinished { get; set; }
        
        /// <summary>
        /// Returns the Player activated in the current Level.
        /// </summary>
        public Player player
        {
            get
            {
                if (!_player)
                    _player = FindObjectOfType<Player>();
                
                return _player;
            }
        }
        
        /// <summary>
        /// Returns the Player Camera activated in the current Level.
        /// </summary>
        public new PlayerCameraThirdPerson camera
        {
            get
            {
                if(!_camera) 
                    _camera = FindObjectOfType<PlayerCameraThirdPerson>();

                return _camera;
            }
        }
        
    }
}