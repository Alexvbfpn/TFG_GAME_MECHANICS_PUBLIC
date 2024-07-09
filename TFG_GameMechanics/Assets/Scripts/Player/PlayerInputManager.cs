using UnityEngine;
using UnityEngine.InputSystem;

namespace GameMechanics.EntitiesSystem
{
    public class PlayerInputManager : MonoBehaviour
    {
        public InputActionAsset actions;

        protected InputAction Move;
        protected InputAction Run;
        protected InputAction Look;
        protected InputAction Pause;
        protected InputAction Interact;
        protected InputAction Jump;
        protected InputAction Slide;
        protected InputAction Dash;

        protected InputAction Down;
        // protected InputAction Attack;
        // protected InputAction Aim;
        // protected InputAction Shoot;
        // protected InputAction SpecialAbility;
        // protected InputAction WeaponSwap;

        protected Camera camera;
        protected float movementDirectionUnlockTime;
        protected const string mouseDeviceName = "Mouse";
        
        protected float? m_lastJumpTime;
        protected const float k_jumpBuffer = 0.15f;
        
        protected virtual void CacheActions()
        {
            Move = actions["Move"];
            Run = actions["Run"];
            Look = actions["Look"];
            // Attack = actions["Attack"];
            // Aim = actions["Aim"];
            // Shoot = actions["Shoot"];
            // SpecialAbility = actions["SpecialAbility"];
            // WeaponSwap = actions["WeaponSwap"];
            Interact = actions["Interact"];
            Pause = actions["Pause"];
            Jump = actions["Jump"];
            Slide = actions["Slide"];
            Down = actions["Down"];
            Dash = actions["Dash"];
        }
        
        
        #region -- GET DIRECTIONS --
        public virtual Vector3 GetMovementDirection()
        {
            if (Time.time < movementDirectionUnlockTime) return Vector3.zero;

            var value = Move.ReadValue<Vector2>();
            return GetAxisWithCrossDeadZone(value);
        }

        public virtual Vector3 GetLookDirection()
        {
            var value = Look.ReadValue<Vector2>();
            if (IsLookingWithMouse())
                return new Vector3(value.x, 0, value.y);

            return GetAxisWithCrossDeadZone(value);
        }

        public virtual Vector3 AdjustSensitivity(Vector3 direction, float sensitivity) =>
            direction * sensitivity; // Adjust the sensitivity of the direction
        
        public virtual Vector3 GetMovementCameraDirection(bool localSpace = true) // Get the direction of the movement relative to the camera
        {
            var direction = GetMovementDirection();

            if (direction.sqrMagnitude > 0) // If we are moving
            {
                var rotation = Quaternion.FromToRotation(camera.transform.up, transform.up); // Necessary rotation from camera rot to player rot
                direction = rotation * camera.transform.rotation * direction; // Rotate the direction to match the camera

                if (localSpace)
                {
                    direction = Vector3.ProjectOnPlane(direction, transform.up); // Project the direction on the plane of the player
                    direction = Quaternion.FromToRotation(transform.up, Vector3.up) * direction;
                }
                direction = direction.normalized;
                
            }

            return direction;
        }
        #endregion
        
        /// <summary>
        /// Remaps a given axis considering the Input System's default deadzone.
        /// This method uses a cross shape instead of a circle one to evaluate the deadzone range.
        /// </summary>
        /// <param name="axis">The axis you want to remap.</param>
        public virtual Vector3 GetAxisWithCrossDeadZone(Vector2 axis)
        {
            var deadzone = InputSystem.settings.defaultDeadzoneMin;
            axis.x = Mathf.Abs(axis.x) > deadzone ? RemapToDeadzone(axis.x, deadzone) : 0; // If the axis is greater than the deadzone, remap it
            axis.y = Mathf.Abs(axis.y) > deadzone ? RemapToDeadzone(axis.y, deadzone) : 0;
            return new Vector3(axis.x, 0, axis.y);
        }

        protected float RemapToDeadzone(float value, float deadzone) => // Is like normalize a vector between a custom values
            Mathf.Sign(value) * ((Mathf.Abs(value) - deadzone) / (1 - deadzone)); // Remap the value to the deadzone

        /// <summary>
        /// Temporally locks the movement direction input.
        /// </summary>
        /// <param name="duration">The duration of the locking state in seconds.</param>
        public virtual void LockMovementDirection(float duration = 0.25f)
        {
            movementDirectionUnlockTime = Time.time + duration;
        }

        public virtual bool IsLookingWithMouse()
        {
            if (Look.activeControl == null)
            {
                return false;
            }

            return Look.activeControl.device.name.Equals(mouseDeviceName);
        }
        
        #region -- GET INPUT PRESSED --

        public virtual bool GetRun() => Run.IsPressed();
        public virtual bool GetRunDown() => Run.WasPressedThisFrame();
        public virtual bool GetRunUp() => Run.WasReleasedThisFrame();
        public virtual bool GetInteractDown() => Interact.WasPerformedThisFrame();
        public virtual bool GetPauseDown() => Pause.WasPressedThisFrame();
        public virtual bool GetSlideDown() => Slide.WasPressedThisFrame();
        public virtual bool GetDownDown() => Down.WasPressedThisFrame();
        public virtual bool GetDashDown() => Dash.WasPressedThisFrame();
        public virtual bool GetJumpDown()
        {
            if (m_lastJumpTime != null &&
                Time.time - m_lastJumpTime < k_jumpBuffer)
            {
                m_lastJumpTime = null;
                return true;
            }

            return false;
        }
        public virtual bool GetJumpUp() => Jump.WasReleasedThisFrame();
        public virtual bool EscapeKeyPressed()
        {
#if UNITY_STANDALONE
            return Keyboard.current.escapeKey.wasPressedThisFrame;
#else
			return false;
#endif
        }
        
        #endregion
        
        public virtual void LockedMovementDirection(float duration = 0.25f) =>
            movementDirectionUnlockTime = Time.time + duration;

        public virtual void Awake() => CacheActions();

        protected virtual void Start()
        {
            camera = Camera.main;
            actions.Enable();
        }

        protected virtual void Update()
        {
            if (Jump.WasPressedThisFrame())
            {
                m_lastJumpTime = Time.time;
            }
        }
        
        protected void OnEnable() => actions?.Enable();
        protected void OnDisable() => actions?.Disable();
    }
}