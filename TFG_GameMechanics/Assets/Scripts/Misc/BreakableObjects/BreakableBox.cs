using UnityEngine;

namespace Misc.BreakableObjects
{
    public class BreakableBox : MonoBehaviour, IBreakable
    {
        public GameObject brokenBoxInstance;
        public GameObject unbrokenBoxInstance;
        public float explosionForce = 150;
        public float explosionRadius = 5;
        private Rigidbody[] rbs;
        
        public void Break()
        {
            gameObject.GetComponent<Rigidbody>().Sleep();
            GetComponent<Collider>().enabled = false;
            brokenBoxInstance.SetActive(true);
            foreach (var rb in rbs)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            unbrokenBoxInstance.SetActive(false);
        }

        protected void Awake()
        {
            rbs = brokenBoxInstance.GetComponentsInChildren<Rigidbody>();
        }
    }
}