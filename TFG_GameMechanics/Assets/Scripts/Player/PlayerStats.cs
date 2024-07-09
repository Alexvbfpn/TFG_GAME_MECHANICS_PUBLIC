using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    [CreateAssetMenu(fileName = "NewPlayerStats", menuName = "GameMechanics/Player/New Player Stats")]
    public class PlayerStats : EntityStats<PlayerStats>
    {
        [Header("General Stats")]
        public float pushForce = 4f;
        public float snapForce = 15f;
        public float slideForce = 10f;
        public float rotationSpeed = 970f;
        public float gravity = 38f;
        public float fallGravity = 65f;
        public float gravityTopSpeed = 50f;

        [Header("Pick'n Throw Stats")]
        public bool canPickUp = true;
        public bool canPickUpOnAir = false;
        public bool canJumpWhileHolding = true;
        public float pickDistance = 0.5f;
        public float throwVelocityMultiplier = 1.5f;

        [Header("Motion Stats")]
        public bool applySlopeFactor = true;
        public float acceleration = 13f;
        public float deceleration = 28f;
        public float friction = 28f;
        public float slopeFriction = 18f;
        public float topSpeed = 6f;
        public float turningDrag = 28f;
        public float airAcceleration = 32f;
        public float brakeThreshold = -0.8f;
        public float slopeUpwardForce = 25f;
        public float slopeDownwardForce = 28f;

        [Header("Running Stats")]
        public float runningAcceleration = 16f;
        public float runningTopSpeed = 7.5f;
        public float runningTurningDrag = 14f;
        
        [Header("Camera Stats")]
        public float cameraHorizontalRotationSpeed = 300f;
        public float cameraVerticalRotationSpeed = 1f;
        public float cameraHorizontalAimingSpeed = 120f;
        public float cameraVerticalAimingSpeed = 0.5f;
        
        [Header("Jump Stats")]
        public int multiJumps = 1;
        public float coyoteJumpThreshold = 0.15f;
        public float maxJumpHeight = 17f;
        public float minJumpHeight = 10f;

        [Header("Slide Stats")]
        public bool canSlide = false;
        public float slideHeight = 1f;
        public float minSlideDuration = 0.5f;
        public float crouchFriction = 10f;
        
        [Header("Slide Stats")]
        public float crawlingAcceleration = 8f;
        public float crawlingFriction = 32f;
        public float crawlingTopSpeed = 2.5f;
        public float crawlingTurningSpeed = 3f;
        
        [Header("Wall Drag Stats")]
        public bool canWallDrag = true;
        public bool wallJumpLockMovement = true;
        public float minGroundDistanceToDrag = 0.5f;
        public float minWallAngleToDrag = 60;
        public LayerMask wallDragLayers;
        public Vector3 wallDragSkinOffset;
        public float wallDragGravity = 12f;
        public float wallJumpDistance = 8f;
        public float wallJumpHeight = 15f;
        
        [Header("Ledge Hanging Stats")]
        public bool canLedgeHang = true;
        public LayerMask ledgeHangingLayers;
        public Vector3 ledgeHangingSkinOffset;
        public float ledgeMaxForwardDistance = 0.25f;
        public float ledgeMaxDownwardDistance = 0.25f;
        public float ledgeSideMaxDistance = 0.5f;
        public float ledgeSideHeightOffset = 0.15f;
        public float ledgeSideCollisionRadius = 0.25f;
        public float ledgeMovementSpeed = 1.5f;
        
        [Header("Ledge Climbing Stats")]
        public bool canClimbLedges = true;
        public LayerMask ledgeClimbingLayers;
        public Vector3 ledgeClimbingSkinOffset;
        public float ledgeClimbingDuration = 1f;
        
        [Header("Dash Stats")]
        public bool canAirDash = true;
        public bool canGroundDash = true;
        public float dashForce = 25f;
        public float dashDuration = 0.3f;
        public float groundDashCoolDown = 0.5f;
        public float allowedAirDashes = 1;
    }
}