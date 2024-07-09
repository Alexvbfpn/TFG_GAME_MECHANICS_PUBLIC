using DG.Tweening;
using GameMechanics.EntitiesSystem;
using Items.Weapons.Controllers;
using UnityEngine;

namespace Misc.PickableObjects
{
    public class PickableWeapon : APickable
    {
        [Tooltip("Duration of first movement of the weapon when detach")]
        public float throwWeaponDetachDuration = 0.01f;
        [Tooltip("Force of weapon throw")]
        public float throwForce = 10;
        [Tooltip("Torque of weapon throw")]
        public float throwTorque = 20;
        
        public WeaponController weaponController { get; protected set;}
        
        public override void PickUp(Transform slot)
        {
            if(beingHold) return;
            beingHold = true;
            
            weaponController.SetShootOriginAndOffset(false); //CAMBIAR SI LUEGO LOS ENEMIGOS COGERAN ARMAS
            
            ChangePhysicsSettings(false);
            
            transform.parent = slot;
            
            transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
            transform.DOLocalRotate(Vector3.zero, 0.25f).SetUpdate(true);
           
            // transform.localPosition = Vector3.zero + offset;

            // m_collider.isTrigger = true;
            // m_interpolation = m_rigidBody.interpolation;
            // m_rigidBody.interpolation = RigidbodyInterpolation.None;
            // m_rigidBody.isKinematic = true;
            
            //ChangeLayer(beingHold);           
            
            onPicked?.Invoke();
        }
        
        public override void Release(Vector3 direction, float force = default)
        {
            if (!beingHold) return;
            
            //weaponController.ThrowWeapon();

            Sequence throwSequence = DOTween.Sequence();
            throwSequence.AppendCallback(() => m_collider.enabled = false);
            throwSequence.AppendCallback(() => m_rigidbody.Sleep());
            throwSequence.Append(transform.DOMove(transform.position - transform.forward, throwWeaponDetachDuration)).SetUpdate(true);
            throwSequence.AppendCallback(() => transform.parent = null);
            throwSequence.AppendCallback(() =>
                transform.position = Camera.main.transform.position + (Camera.main.transform.right * 0.1f+ FindObjectOfType<Player>().radius * Camera.main.transform.forward));// +(FindObjectOfType<Player>().radius * Camera.main.transform.forward)
            throwSequence.AppendCallback(() => ChangePhysicsSettings(true));
            throwSequence.AppendCallback(() => m_rigidbody.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse));
            throwSequence.AppendCallback(() => m_rigidbody.AddTorque(transform.right + transform.up * throwTorque, ForceMode.Impulse));
            throwSequence.AppendCallback(() => m_collider.enabled = true);
            throwSequence.AppendCallback(() => m_rigidbody.WakeUp());
            //throwSequence.AppendCallback(() => isThrown = false);
            
            
            beingHold = false;
            
            //ChangeLayer(beingHold);            
            
            onReleased?.Invoke();
        }

        public void ReleaseFromEnemy()
        {
            Sequence throwSequence = DOTween.Sequence();
            throwSequence.AppendCallback(() => m_collider.enabled = false);
            throwSequence.AppendCallback(() => m_rigidbody.Sleep());
            throwSequence.AppendCallback(() => transform.parent = null);
            throwSequence.AppendCallback(() =>
                transform.position = transform.position + (Camera.main.transform.position - transform.position).normalized * 0.5f);
            throwSequence.AppendCallback(() => m_collider.enabled = true);
            throwSequence.AppendCallback(() => m_rigidbody.WakeUp());
            throwSequence.AppendCallback(() => ChangePhysicsSettings(true));
            throwSequence.AppendCallback(() => m_rigidbody.AddForce((Camera.main.transform.position - transform.position).normalized * 15f, ForceMode.Impulse));
            throwSequence.AppendCallback(() => m_rigidbody.AddForce(Vector3.up * 2, ForceMode.Impulse));
            throwSequence.AppendCallback(() => m_rigidbody.AddTorque(-transform.right + transform.up * throwTorque, ForceMode.Impulse));
            throwSequence.AppendCallback(() => beingHold = false);
            //beingHold = false;
            //transform.parent = null;
            //ChangePhysicsSettings(true);
            
            
            // m_rigidbody.AddForce(Vector3.up * 2, ForceMode.Impulse);
            // m_rigidbody.AddTorque(-transform.right * throwTorque, ForceMode.Impulse);
        }
        
        public void ChangePhysicsSettings(bool detach = true)
        {
            if(transform.parent != null)
                return;
            m_rigidbody.isKinematic = !detach;
            m_rigidbody.interpolation = detach ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
            m_collider.isTrigger = !detach;
        }
        
        protected override void Awake()
        {
            base.Awake();
            weaponController = GetComponent<WeaponController>();
        }
    }
}