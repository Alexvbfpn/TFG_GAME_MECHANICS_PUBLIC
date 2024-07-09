using System;
using GameMechanics.EntitiesSystem;
using Misc.BreakableObjects;
using Patterns.ObjectPool.Interfaces;
using UnityEngine;

namespace Weapons
{
    public class LanceWeapon : AWeapon, IPooleableObject
    {
        
        public override void Shoot()
        {
            TrajectoryCorrection();
            //_isOnAir = true;
            transform.parent = null;
            //transform.position += (FindObjectOfType<Player>().radius) * FindObjectOfType<Player>().transform.forward;
            
            _player.isHoldingWeapon = false;
            _weaponRb.isKinematic = false;
            _weaponRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            
            //transform.eulerAngles = new Vector3(0, -90 +transform.eulerAngles.y, 0);
            transform.position += transform.right/5;
            transform.forward = Camera.main.transform.forward;
            _weaponRb.AddForce(Camera.main.transform.forward * weaponStats.shootForce, ForceMode.Impulse);
            _weaponCollider.enabled = true;
            _isOnAir = true;
        }

        public override void SpecialWeaponAbility()
        {
            Explode();
        }

        public override void InitializeAxis() => axis = 0;
        
        public override void OnStep()
        {
            if (_isOnAir)
            {
                transform.localEulerAngles += Vector3.forward * (weaponStats.rotationSpeed * Time.deltaTime);
                
                // Drift towards trajectory override (this is so that projectiles can be centered 
                // with the camera center even though the actual weapon is offset)
                CorrectionDrift();
            }
        }

        public void ChangePhysicsSettings(bool active)
        {
            if (!active)
                _weaponRb.Sleep();
            else
                _weaponRb.WakeUp();
            _weaponRb.isKinematic = !active;
            _weaponRb.collisionDetectionMode = active ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
            _weaponCollider.enabled = active;
        }

        public void Explode()
        {
            Logging.Log("Lance Exploded");
            //Reset();
        }

        public IPooleableObject Clone()
        {
            GameObject clone = Instantiate(gameObject, transform.parent);
            return clone.GetComponent<IPooleableObject>();
        }

        public bool Active { get; set; }
        public IObjectPool elementPool { get; set; }
        public void Reset()
        {
            _isOnAir = false;
            Explode();
            ChangePhysicsSettings(false);
            ResetParent();
            gameObject.SetActive(false);
        }
        
        #region --- COLLISIONS ---

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 11)
            {
                _isOnAir = false;
                print(collision.gameObject.name);
                _weaponRb.Sleep();
                _weaponRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                _weaponRb.isKinematic = true;
            }

        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IBreakable> (out var breakable))
            {
                breakable.Break();
            }
        }

        #endregion
    }
}