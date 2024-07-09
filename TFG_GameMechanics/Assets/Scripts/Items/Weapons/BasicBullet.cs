using Enemy;
using GameMechanics;
using UnityEngine;

namespace Items.Weapons
{
    public class BasicBullet : MonoBehaviour
    {
        public float speed = 8f;
        public float lifeTime = 20f;
        public float distance = 1f;
        Rigidbody rb;
        
        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        protected virtual void Update()
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                Destroy(gameObject);
            }
        }

        protected void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(GameTags.Enemy))
            {
                Debug.Log("Enemy hit");
                //EnemyCollisonLogic
                BodyRagdollPart ragdollPart = collision.gameObject.GetComponent<BodyRagdollPart>();
                
                ragdollPart.enemyOwner.Ragdoll();
                ragdollPart.enemyOwner.Hit(collision.contacts[0].point);
            }
            
            Destroy(gameObject);
        }
    }
}