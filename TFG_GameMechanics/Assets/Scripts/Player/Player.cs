using GameMechanics.EntitiesSystem.States;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    [AddComponentMenu("Mechanics/Entities/Player/Player")]
    //[RequireComponent(typeof(PlayerInputManager))]
    [RequireComponent(typeof(PlayerStatsManager))]
    [RequireComponent(typeof(PlayerStateManager))]
    public class Player : Entity<Player>
    {
        public PlayerEvents events;

        public Transform skin;

        protected Vector3 m_respawnPosition;
        protected Quaternion m_respawnRotation;

        protected Vector3 m_skinInitialPosition;
        protected Quaternion m_skinInitialRotation;
        protected Vector3 m_skinInitialScale;
        
        /// <summary>
        /// Returns the Player Input Manager instance.
        /// </summary>
        public virtual PlayerInputManager playerInputs { get; protected set; }
        /// <summary>
        /// Returns the Player Stats Manager instance.
        /// </summary>
        public PlayerStatsManager stats { get; protected set; }
        
        /// <summary>
        /// Returns how many times the Player jumped.
        /// </summary>
        public int jumpCounter { get; protected set; }
        
        /// <summary>
        /// The last time the Player performed an dash.
        /// </summary>
        /// <value></value>
        public float lastDashTime { get; protected set; }
        
        /// <summary>
        /// Returns the normal of the last wall the Player touched.
        /// </summary>
        public Vector3 lastWallNormal { get; protected set; }
        
        /// <summary>
        /// Returns the Collider of the ledge the Player is hanging.
        /// </summary>
        public Collider ledge { get; protected set; }
        
        /// <summary>
        /// Returns true if the Player can stand up.
        /// </summary>
        public virtual bool canStandUp => !SphereCast(transform.up,
            originalHeight * 0.5f + radius - controller.skinWidth);
        
        public virtual bool canSlide => playerInputs.GetSlideDown() && stats.current.canSlide;
        
        /// <summary>
        /// Resizes the Character Controller to a given height.
        /// </summary>
        /// <param name="height">The desired height.</param>
        public virtual void ResizeCollider(float height) => controller.Resize(height);
        
        #region --- INITIALIZERS ---

        protected virtual void InitializeInputs() => playerInputs = GetComponent<PlayerInputManager>();
        protected virtual void InitializeStats() => stats = GetComponent<PlayerStatsManager>();
        protected virtual void InitializeTag() => tag = GameTags.Player;
        
        protected virtual void InitializeRespawn()
        {
            m_respawnPosition = transform.position;
            m_respawnRotation = transform.rotation;
        }
        
        protected virtual void InitializeSkin()
        {
            if(!skin) return;
            m_skinInitialPosition = skin.localPosition;
            m_skinInitialRotation = skin.localRotation;
            m_skinInitialScale = skin.localScale;
        }
        
        #endregion

        /// <summary>
        /// Sets the Skin parent to a given transform.
        /// </summary>
        /// <param name="parent">The transform you want to parent the skin to.</param>
        public virtual void SetSkinParent(Transform parent, Vector3 offset = default)
        {
            if(!skin) return;
            
            skin.parent = parent;
            skin.position += transform.rotation * offset;
        }
        
        /// <summary>
        /// Resets the Skin parenting to its initial one, with original position and rotation.
        /// </summary>
        public virtual void ResetSkinParent()
        {
            if (!skin) return;
            Logging.Log("ResetSkinParent");
            skin.parent = transform;
            skin.localPosition = m_skinInitialPosition;
            skin.localRotation = m_skinInitialRotation;
        }
        
        public virtual void SetInputEnabled(bool value)
        {
            playerInputs.enabled = value;
        }
        
        /// <summary>
        /// Resets Player state, health, position, and rotation.
        /// </summary>
        public virtual void Respawn()
        {
            velocity = Vector3.zero;
            transform.SetPositionAndRotation(m_respawnPosition, m_respawnRotation);
            states.Change<IdlePlayerState>();
        }
        
        /// <summary>
        /// Moves the Player smoothly in a given direction.
        /// </summary>
        /// <param name="direction">The direction you want to move.</param>
        public virtual void Accelerate(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0)
            {   
                var turningDrag = isGrounded && playerInputs.GetRun() ? stats.current.runningTurningDrag : stats.current.turningDrag;
                var acceleration = isGrounded && playerInputs.GetRun() ? stats.current.runningAcceleration : stats.current.acceleration;
                var finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;
                var topSpeed = playerInputs.GetRun()? stats.current.runningTopSpeed : stats.current.topSpeed;
            
                Accelerate(direction, turningDrag, finalAcceleration, topSpeed);

                if (playerInputs.GetRunUp())
                {
                    lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed); //Clamp the speed to the top speed
                }
            }
        }
        
        /// <summary>
        /// Moves the Player smoothly in the input direction relative to the camera.
        /// </summary>
        public virtual void AccelerateToInputDirection()
        {
            var inputDirection = playerInputs.GetMovementCameraDirection();
            Accelerate(inputDirection);
        }
        
        /// <summary>
        /// Applies the standard slope factor to the Player.
        /// </summary>
        public virtual void RegularSlopeFactor()
        {
            if (stats.current.applySlopeFactor)
                SlopeFactor(stats.current.slopeUpwardForce, stats.current.slopeDownwardForce);
        }

        /// <summary>
        /// Smoothly sets Lateral Velocity to zero by its friction stats.
        /// </summary>
        public virtual void ApplyFriction()
        {
            if(OnSlopingGround())
                Decelerate(stats.current.slopeFriction);
            else
                Decelerate(stats.current.friction);
        }
        
        /// <summary>
        /// Moves the Player smoothly in a given direction with crawling stats.
        /// </summary>
        /// <param name="direction">The direction you want to move.</param>
        public virtual void SlidingAccelerate(Vector3 direction) =>
            Accelerate(direction, stats.current.crawlingTurningSpeed, stats.current.crawlingAcceleration, stats.current.crawlingTopSpeed);
        
        /// <summary>
        /// Smoothly sets Lateral Velocity to zero by its deceleration stats.
        /// </summary>
        public virtual void Decelerate() => Decelerate(stats.current.deceleration);
        
        /// <summary>
        /// Applies a downward force by its gravity stats.
        /// </summary>
        public virtual void ApplyGravity()
        {
            if (!isGrounded && verticalVelocity.y > -stats.current.gravityTopSpeed)
            {
                var speed = verticalVelocity.y;
                var force = verticalVelocity.y > 0 ? stats.current.gravity : stats.current.fallGravity;
                speed -= force * gravityMultiplier * Time.deltaTime;
                speed = Mathf.Max(speed, -stats.current.gravityTopSpeed);
                verticalVelocity = new Vector3(0, speed, 0);
            }
        }
        
        /// <summary>
        /// Applies a downward force when ground by its snap stats.
        /// </summary>
        public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);
        
        public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.rotationSpeed);

        /// <summary>
        /// Makes a transition to the Fall State if the Player is not grounded.
        /// </summary>
        public virtual void Fall()
        {
            if (!isGrounded)
            {
                states.Change<FallPlayerState>();
            }
        }


        /// <summary>
        /// Handles ground jump with proper evaluations and height control.
        /// </summary>
        public virtual void Jump()
        {
            var canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
            var canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);
            if (isGrounded|| canMultiJump || canCoyoteJump)
            {
                if(playerInputs.GetJumpDown())
                {
                    Jump(stats.current.maxJumpHeight);
                }
            }
            if (playerInputs.GetJumpUp() && (jumpCounter > 0) && (verticalVelocity.y > stats.current.minJumpHeight))
            {
                verticalVelocity = Vector3.up * stats.current.minJumpHeight;
            }
        }
        
        /// <summary>
        /// Applies an upward force to the Player.
        /// </summary>
        /// <param name="height">The force you want to apply.</param>
        public virtual void Jump(float height)
        {
            jumpCounter++;
            verticalVelocity = Vector3.up * height;
            states.Change<FallPlayerState>();
            events.onJump?.Invoke();
        }
        
        /// <summary>
        /// Applies jump force to the Player in a given direction.
        /// </summary>
        /// <param name="direction">The direction that you want to jump.</param>
        /// <param name="height">The upward force that you want to apply.</param>
        /// <param name="distance">The force towards the direction that you want to apply.</param>
        public virtual void DirectionalJump(Vector3 direction, float height, float distance)
        {
            jumpCounter++;
            verticalVelocity = Vector3.up * height;
            lateralVelocity = direction * distance;
            events.onJump?.Invoke();
        }
        
        /// <summary>
        /// Sets the jump counter to zero affecting further jump evaluations.
        /// </summary>
        public virtual void ResetJumps() => jumpCounter = 0;
        
        /// <summary>
        /// Sets the jump couter to a specific value.
        /// </summary>
        /// <param name="amount">The amount of jumps.</param>
        public virtual void SetJumps(int amount) => jumpCounter = amount;
        
        public virtual void LedgeGrab()
        {
            if (stats.current.canLedgeHang && verticalVelocity.y < 0 &&
                states.ContainsStateOfType(typeof(LedgeHangingPlayerState)) &&
                DetectingLedge(stats.current.ledgeMaxForwardDistance,
                    stats.current.ledgeMaxDownwardDistance, out var hit))
            {
                if (Vector3.Angle(hit.normal, transform.up) > 0) return;
                if (hit.collider is CapsuleCollider || hit.collider is SphereCollider) return;

                var ledgeDistance = radius + stats.current.ledgeMaxForwardDistance;
                var lateralOffset = transform.forward * ledgeDistance;
                var verticalOffset = -transform.up * height * 0.5f - center;
                ledge = hit.collider;
                velocity = Vector3.zero;
                transform.position = hit.point - lateralOffset + verticalOffset;
                //HandlePlatform(hit.collider);
                states.Change<LedgeHangingPlayerState>();
                events.onLedgeGrabbed?.Invoke();
            }
        }
        
        protected virtual bool DetectingLedge(float forwardDistance, float downwardDistance, out RaycastHit ledgeHit)
        {
            var contactOffset = Physics.defaultContactOffset + positionDelta;
            var ledgeMaxDistance = radius + forwardDistance;
            var ledgeHeightOffset = height * 0.5f + contactOffset;
            var upwardOffset = transform.up * ledgeHeightOffset;
            var forwardOffset = transform.forward * ledgeMaxDistance;

            if (Physics.Raycast(position + upwardOffset, transform.forward, ledgeMaxDistance,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ||
                Physics.Raycast(position + forwardOffset * .01f, transform.up, ledgeHeightOffset,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                ledgeHit = new RaycastHit();
                return false;
            }

            var origin = position + upwardOffset + forwardOffset;
            var distance = downwardDistance + contactOffset;

            return Physics.Raycast(origin, -transform.up, out ledgeHit, distance,
                stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore);
        }
        
        public virtual void WallDrag(Collider other)
        {
            if (!stats.current.canWallDrag || verticalVelocity.y > 0) return;

            var maxWallDistance = radius + stats.current.ledgeMaxForwardDistance;
            var minGroundDistance = height * 0.5f + stats.current.minGroundDistanceToDrag;

            var detectingLedge = DetectingLedge(maxWallDistance, height, out _);
            var detectingWall = SphereCast(transform.forward, maxWallDistance,
                out var hit, stats.current.wallDragLayers);
            var detectingGround = SphereCast(-transform.up, minGroundDistance);
            var wallAngle = Vector3.Angle(transform.up, hit.normal);

            if (!detectingWall || detectingGround || detectingLedge ||
                wallAngle < stats.current.minWallAngleToDrag)
                return;

            //HandlePlatform(hit.collider);
            lastWallNormal = hit.normal;
            states.Change<WallDragPlayerState>();
        }
        
        // protected override void HandleGround()
        // {
        //     var distance = (height * 0.5f) + m_groundOffset;
        //     var sphereColliding = SphereCast(-transform.up, distance, out var sphereHit);
        //     var hitColliding = Physics.Raycast(position, -transform.up, out var rayHit,
        //         distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        //
        //     var colliding = sphereColliding || hitColliding;
        //     var hit = SortGroundHit(rayHit, sphereHit);
        //     var movingTowardGround = colliding && Vector3.Dot(velocity, hit.normal) <= 0;
        //     var validAngle = colliding && Vector3.Angle(hit.normal, CurrentWorldUp()) <= controller.slopeLimit;
        //
        //     var canFall = rotateToGround;
        //     var steepGround = isGrounded && groundAngle >= 90;
        //     var falling = canFall && steepGround && lateralVelocity.magnitude < minSpeedToFall;
        //     var landing = colliding && movingTowardGround && validAngle && (!rotateToGround || !falling);
        //
        //     if (landing)
        //     {
        //         if (!isGrounded && EvaluateLanding(hit))
        //             EnterGround(hit);
        //
        //         UpdateGround(hit);
        //     }
        //     else
        //         ExitGround();
        // }

        public virtual void IdleStepSpecificLogic(){}
        public virtual void WalkStepSpecificLogic(){}
        
        protected override void Awake()
        {
            base.Awake(); // Inicializamos controller, estados y parent
            InitializeInputs();
            InitializeStats();
            InitializeSkin();
            InitializeRespawn();
            InitializeTag();
            
            entityEvents.OnGroundEnter.AddListener(() =>
            {
                ResetJumps();
            });
        }
    }
}