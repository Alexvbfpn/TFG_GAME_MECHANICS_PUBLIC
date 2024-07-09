using GameMechanics.EntitiesSystem.States;
using UnityEngine;
using UnityEngine.Events;

namespace GameMechanics.EntitiesSystem.GowPlayer
{
    [RequireComponent(typeof(ArmedPlayerInputManager))]
    public class ArmedPlayer : Player
    {
        public bool isHoldingWeapon = true;
        public bool isPulling = false;

        public UnityEvent<float> onWeaponSwap;
        public UnityEvent onWeaponCatch;

        public bool aiming = false;
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
        
        public override void IdleStepSpecificLogic()
        {
            Aim();
            SpecialAbility();
            WeaponSwap();
        }

        //SPECIFIC METHODS
        public virtual void Aim()
        {
            if (isHoldingWeapon && armedPlayerInputs.GetAimDown())
            {
                states.Change<AimPlayerState>();
            }

            if (isHoldingWeapon && armedPlayerInputs.GetAimUp())
            {
                states.Change<IdlePlayerState>();
            }
        }

        public virtual void AimPlayerRotation()
        {
            var camera = Camera.main;
            
            var forward = camera.transform.forward;
            var right = camera.transform.right;

            var desiredMoveDirection = forward;
            desiredMoveDirection.y= 0;
            //transform.forward =desiredMoveDirection;
            
            Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, stats.current.rotationSpeed);
            //transform.rotation =  Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), stats.current.rotationSpeed);
            //skin.rotation = Quaternion.Slerp(skin.rotation, Quaternion.LookRotation(desiredMoveDirection), stats.current.rotationSpeed);
            //skin.rotation = Quaternion.Slerp(skin.rotation, Quaternion.LookRotation(desiredMoveDirection), stats.current.rotationSpeed);
        }
        
        public virtual void Shoot()
        {
            if (isHoldingWeapon && armedPlayerInputs.GetShootDown())
            {
                isHoldingWeapon = false;
                events.onShoot?.Invoke();
                //states.Change<IdlePlayerState>();
            }
        }

        public virtual void SpecialAbility()
        {
            if (armedPlayerInputs.GetSpecialAbilityDown())
            {
                events.onSpecialAbility?.Invoke();
            }
        }

        public virtual void WeaponSwap()
        {
            if (isHoldingWeapon && armedPlayerInputs.GetWeaponSwapDown())
            {
                Debug.Log(armedPlayerInputs.GetWeaponSwapDirection());
                onWeaponSwap?.Invoke(armedPlayerInputs.GetWeaponSwapDirection());
                //events.onWeaponSwap?.Invoke();
            }
        }
    }
}