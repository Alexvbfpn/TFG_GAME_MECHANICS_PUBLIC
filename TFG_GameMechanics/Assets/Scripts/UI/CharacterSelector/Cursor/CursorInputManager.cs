using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInputManager = GameMechanics.EntitiesSystem.PlayerInputManager;

namespace UI.CharacterSelector
{
    [AddComponentMenu("Mechanics/Character Selector/Cursor Input Manager")]
    public class CursorInputManager : MonoBehaviour
    {
        public InputActionAsset actions;

        protected InputAction Move;
        protected InputAction Pause;
        protected InputAction Interact;
        protected InputAction Cancel;
        
        protected float movementDirectionUnlockTime;
        
        protected virtual void CacheActions()
        {
            Move = actions["Move"];
            Interact = actions["Interact"];
            Cancel = actions["Cancel"];
            // Pause = actions["Pause"];
        }
        
        public virtual bool GetInteractDown() => Interact.WasPerformedThisFrame();
        public virtual bool GetCancelDown() => Cancel.WasPerformedThisFrame();
        public virtual bool GetPauseDown() => Pause.WasPressedThisFrame();
        
        public virtual Vector3 GetMovementDirection()
        {
            if (Time.time < movementDirectionUnlockTime) return Vector3.zero;

            var value = Move.ReadValue<Vector2>();
            return GetAxisWithCrossDeadZone(value);
        }
        
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
        
        public virtual void Awake() => CacheActions();

        protected virtual void Start()
        {
            actions.Enable();
        }

        protected void OnEnable() => actions?.Enable();
        protected void OnDisable() => actions?.Disable();
    }
}