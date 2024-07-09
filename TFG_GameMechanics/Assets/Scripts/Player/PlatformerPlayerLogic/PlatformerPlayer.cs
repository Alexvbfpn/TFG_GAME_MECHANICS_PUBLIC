using System.Collections;
using System.Numerics;
using GameMechanics.EntitiesSystem.States;
using UnityEngine;
using UnityEngine.Events;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace GameMechanics.EntitiesSystem.PlatformerPlayerLogic
{
    public class PlatformerPlayer : Player
    {
        public PlatformerPlayerEvents platformerEvents;

        [HideInInspector] public bool isBoosting;
        [HideInInspector] public bool chargingSpeedBooster;
        [HideInInspector] public bool isShinesparkStored;
        public bool usingShinespark;
        
        [Header("Speed Boost Settings")]
        [SerializeField] protected float chargeTime = 1.5f;
        
        [Header("Shinespark Settings")]
        [SerializeField] protected float shinesparkEnergyStoredDuration = 1.5f;
        
        
        protected Coroutine ChargeSpeedBoosterCoroutine;
        protected Coroutine ShinesparkEnergyStoredCoroutine;
        
        
        public int direction {get; set;} = 1;
        
        public int storedDirection {get; set;} = 1;

        public Vector2 dashDirection { get; protected set; }

        public int impactSide { get; protected set; }
        public float dashAngle { get; protected set; }
        
        public override void Accelerate(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0)
            {   
                var turningDrag = isGrounded && playerInputs.GetRun() ? stats.current.runningTurningDrag : stats.current.turningDrag;
                var acceleration = isGrounded && playerInputs.GetRun() ? stats.current.runningAcceleration : stats.current.acceleration;
                var finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;
                var topSpeed = playerInputs.GetRun()? stats.current.runningTopSpeed : stats.current.topSpeed;

                direction = Vector3.right * direction.x;
                
                Accelerate(direction, turningDrag, finalAcceleration, topSpeed);

                if (playerInputs.GetRunUp())
                {
                    lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed); //Clamp the speed to the top speed
                }
            }
        }

        public void StartSlide()
        {
            if (canSlide)
            {
                states.Change<SlidingPlayerState>();
                platformerEvents.onStartSlide?.Invoke();
            }
        }

        #region --- SPEED BOOSTER ---

        public void CheckChargeSpeedBooster()
        {
            if(isBoosting) return;

            if (playerInputs.GetRunDown() && !chargingSpeedBooster)
            {
                ChargeSpeedBooster(true);
                platformerEvents.onChargingBoost?.Invoke();
            }
            else if (playerInputs.GetRunUp())
            {
                ChargeSpeedBooster(false);
                platformerEvents.onUnchargingBoost?.Invoke();
            }
        }
        
        protected void ChargeSpeedBooster(bool charge)
        {
            SetChargingSpeedBooster(charge);
            if (charge)
            {
                ChargeSpeedBoosterCoroutine = StartCoroutine(ChargeCoroutine());
            }
            else
            {
                StopCoroutine(ChargeSpeedBoosterCoroutine);
            }
        }
        protected IEnumerator ChargeCoroutine()
        {
            yield return new WaitForSeconds(chargeTime);
            SetChargingSpeedBooster(false);
            SetSpeedBoostState(true);
        }
        
        public void SetSpeedBoostState(bool state)
        {
            isBoosting = state;
            platformerEvents.onBoost?.Invoke(state);
            SetSpeedMultiplier(state);
        }

        public void SetSpeedMultiplier(bool boost)
        {
            topSpeedMultiplier = boost ? 2 : 1;
            if (!boost)
            {
                lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, stats.current.topSpeed);
            }
            
            //Logging.Log("Speed Multiplier" + topSpeedMultiplier);
        }
        
        public void SetChargingSpeedBooster(bool state)
        {
            chargingSpeedBooster = state;
        }

        #endregion

        #region --- SHINESPARK ---

        public void CheckChargeShinespark()
        {
            if (!isBoosting) return;

            if (playerInputs.GetDownDown())
            {
                SetSpeedBoostState(false);
                StoreShinesparkEnergy(true, false, false);

                StartCoroutine(CrouchCoroutine());
                
                ShinesparkEnergyStoredCoroutine = StartCoroutine(ShinesparkEnergyStoredCooldown());
            }
            
            IEnumerator CrouchCoroutine()
            {
                // movement.canMove = false;
                playerInputs.enabled = false;
                yield return new WaitForSeconds(.2f);
                playerInputs.enabled = true;
                // movement.canMove = true;
            }

            IEnumerator ShinesparkEnergyStoredCooldown()
            {
                yield return new WaitForSeconds(shinesparkEnergyStoredDuration);
                StoreShinesparkEnergy(false, true, false);
            }
        }

        protected void StoreShinesparkEnergy(bool state, bool fadeOut, bool impact)
        {
            isShinesparkStored = state;
            
            if (state)
            {
                platformerEvents.onShinesparkCharged?.Invoke();
            }
            else
            {
                StopCoroutine(ShinesparkEnergyStoredCoroutine);
                if (fadeOut)
                {
                    platformerEvents.onShinesparkUncharged?.Invoke();
                }
            }
            
        }

        public void ShinesparkDash()
        {
            if(!isShinesparkStored || usingShinespark) return;
            
            if (playerInputs.GetDashDown())
            {
                usingShinespark = true;
                StopCoroutine(ShinesparkEnergyStoredCoroutine);
                StartCoroutine(DashCoroutine());
                platformerEvents.onShinesparkStarted?.Invoke();
            }
            
            IEnumerator DashCoroutine()
            {
                states.Change<FloatingBeforeShinesparkPlayerState>();
                yield return new WaitForSeconds(1f);
                
                SetDashDirection();
                //usingShinespark = false;
                isShinesparkStored = false;
                states.Change<ShinesparkPlayerState>();
            }
        }

        protected void SetDashDirection()
        {
            float angle = Mathf.Atan2( playerInputs.GetMovementDirection().x, playerInputs.GetMovementDirection().z) * Mathf.Rad2Deg;
            
            angle = Mathf.Round(angle / 45) * 45;

            switch (angle)
            {
                case 45: 
                    dashDirection = new Vector2(0.75f, 0.75f);
                    impactSide = 0;
                    dashAngle = 3;
                    break;
                case -45: dashDirection = new Vector2(-0.75f, 0.75f);
                    impactSide = 0;
                    dashAngle = 3;
                    break;
                case 90: 
                    dashDirection = Vector2.right;
                    impactSide = 1;
                    dashAngle = 0;
                    break;
                case -90: 
                    dashDirection = Vector2.left; 
                    impactSide = 1;
                    dashAngle = 0;
                    break;
                case 135: 
                    dashDirection = new Vector2(0.75f, -0.75f); 
                    impactSide = 2;
                    dashAngle = 4;
                    break;
                case -135: 
                    dashDirection = new Vector2(-0.75f, -0.75f); 
                    impactSide = 2;
                    dashAngle = 4;
                    break;
                case 180: 
                    dashDirection = Vector2.down; 
                    impactSide = 2;
                    dashAngle = 2;
                    break;
                case -180:
                    dashDirection = Vector2.down;
                    impactSide = 2;
                    dashAngle = 2;
                    break;
                default: 
                    dashDirection = Vector2.up; 
                    impactSide = 0;
                    dashAngle = 1;
                    break;
            }
            Logging.Log("Seteamos dash direction a " + dashDirection);
            
            platformerEvents.onShinesparkAngleSet?.Invoke(Mathf.Abs(angle));
            //platformerEvents.onImpactSideSet?.Invoke(impactSide);
            platformerEvents.onDashAngleSet?.Invoke(dashAngle);
        }
        
        public void FinishShinesparkDash()
        {
            if (!usingShinespark) return;
            
            StartCoroutine(FinishShinespark());
        }
        
        IEnumerator FinishShinespark()
        {
            usingShinespark = false;
            isShinesparkStored = false;
            yield return new WaitForSeconds(1f);
            states.Change<IdlePlayerState>();
            platformerEvents.onShinesparkUncharged?.Invoke();
            impactSide = -1;
            platformerEvents.onImpactSideSet?.Invoke(impactSide);
        }
        
        #endregion
        
        public void CheckDirection()
        {
            if(direction != storedDirection)
            {
                platformerEvents.onDirectionChange?.Invoke();
                if (isGrounded) // || (direction == 0 && !isShinesparkStored))
                {
                    SetSpeedBoostState(false);
                }
                storedDirection = direction;
            }
        }
        
        protected bool TouchingWall()
        {
            return SphereCast((position - transform.forward * 0.2f) + Vector3.up, radius, out RaycastHit info);
        }
        
        public override void IdleStepSpecificLogic()
        {
            ShinesparkDash();
        }
        
        public override void WalkStepSpecificLogic()
        {
            StartSlide();
            CheckChargeSpeedBooster();
            CheckDirection();
            CheckChargeShinespark();
        }

        protected override void Awake()
        {
            base.Awake();
            platformerEvents.onUnchargingBoost.AddListener(() => ChargeSpeedBooster(false));
        }
        
        protected override void OnUpdate()
        {
            //direction = playerInputs.GetMovementDirection().x > 0 ? 1 : -1;
            direction = playerInputs.GetMovementDirection().x > 0 ? 1 : playerInputs.GetMovementDirection().x < 0? -1 : 0;
        }
    }
}