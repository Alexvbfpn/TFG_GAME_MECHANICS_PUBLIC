using System.Collections;
using Items.Weapons.Controllers;
using Misc.PickableObjects;
using UnityEngine;

namespace Enemy
{
    public class EnemyProtScript : MonoBehaviour
    {
        Animator m_animator;
        public Transform weaponHolder;
        public bool dead { get; protected set; }
        public GameObject hitParticle;

        [ContextMenu("Ragdoll")]
        public void Ragdoll()
        {
            m_animator.enabled = false;
            BodyRagdollPart[] ragdollParts = GetComponentsInChildren<BodyRagdollPart>();
            foreach (BodyRagdollPart ragdollPart in ragdollParts)
            {
                ragdollPart.ChangePhysicsSettings(false);
            }
            dead = true;

            if (weaponHolder.GetComponentInChildren<WeaponController>() != null)
            {
                WeaponController weaponController = weaponHolder.GetComponentInChildren<WeaponController>();
                weaponController.GetComponent<PickableWeapon>().ReleaseFromEnemy();
            }
            
        }
        
        protected IEnumerator ShootWeapon()
        {
            yield return new WaitForSeconds(Random.Range(1.5f, 2.75f));
            if (weaponHolder.GetComponentInChildren<WeaponController>() != null)
            {
                WeaponController weaponController = weaponHolder.GetComponentInChildren<WeaponController>();
                if (weaponController.GetCurrentAmmo() > 0 && weaponController.TryUse())
                {
                    m_animator.SetTrigger("Shoot");
                }
                else if (weaponController.GetCurrentAmmo() <= 0 && weaponController.TryUse())
                {
                    weaponController.GetComponent<PickableWeapon>().ReleaseFromEnemy();
                }
            }
            else
            {
                yield break;
            }

            if (!dead)
                StartCoroutine(ShootWeapon());

            yield break;
        }

        public void Hit(Vector3 position)
        {
            hitParticle.transform.position = position;
            hitParticle.GetComponent<ParticleSystem>().Play();
        }
        
        protected void Awake()
        {
            m_animator = GetComponentInChildren<Animator>();
            
            // if(weaponHolder.GetComponentInChildren<WeaponController>() != null)
            //         weaponHolder.GetComponentInChildren<WeaponController>().ChangeActiveState();
            weaponHolder.GetComponentInChildren<WeaponController>().SetShootOriginAndOffset(true);
        }

        protected void Start()
        {
            if (weaponHolder.GetComponentInChildren<WeaponController>() != null)
            {
                StartCoroutine(ShootWeapon());
            }
        }

        protected void Update()
        {
            if (!dead)
            {
                transform.LookAt(new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z));
            }
        }
    }
}