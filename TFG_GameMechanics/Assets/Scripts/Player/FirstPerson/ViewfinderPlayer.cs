using GameMechanics.EntitiesSystem.GowPlayer;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    [RequireComponent(typeof(ArmedPlayerInputManager))]
    public class ViewfinderPlayer : FirstPersonPlayer
    {
        /// <summary>
        /// Returns the Player Input Manager instance.
        /// </summary>
        public ArmedPlayerInputManager armedPlayerInputs
        {
            get => (ArmedPlayerInputManager)playerInputs; 
            protected set => playerInputs = value;
        }
        
        public override PlayerInputManager playerInputs
        {
            get => base.playerInputs;
            protected set
            {
                if (value is ArmedPlayerInputManager armedPlayerInputManager)
                    base.playerInputs = armedPlayerInputManager;
                else
                    Debug.LogError("Player Inputs must be of type ArmedPlayerInputManager.");
            }
        }
        
        protected override void InitializeInputs() => playerInputs = GetComponent<ArmedPlayerInputManager>();

        public void StartAimItem()
        {
            if(armedPlayerInputs.GetAimDown())
            {
                // Aim the polaroid camera
                events.onAim?.Invoke(true);
            }
        }
        
        public void StopAimItem()
        {
            if (armedPlayerInputs.GetAimUp())
            {
                // Stop aiming the polaroid camera
                events.onAim?.Invoke(false);
            }
            
            // Stop aiming the polaroid camera
            //events.onAim?.Invoke(false);
        }
        
        public void ShootItem()
        {
            if (armedPlayerInputs.GetShootDown())
            {
                // Take a picture with the polaroid camera
                events.onShoot?.Invoke();
            }
        }
        
        public override void IdleStepSpecificLogic()
        {
            StartAimItem();
            StopAimItem();
            ShootItem();
        }
        
        public override void WalkStepSpecificLogic()
        {
            StartAimItem();
            StopAimItem();
            ShootItem();
        }
    }
}