using UnityEngine;

namespace Enemy
{
    public class BodyRagdollPart : MonoBehaviour
    {
        protected Rigidbody m_rigidbody;
        public EnemyProtScript enemyOwner {get; protected set;}
        protected Renderer m_bodyPartRenderer;
        //protected GameObject m_bodyPartPrefab;
        public bool replaced {get; protected set;}
        protected void InitializeRenderer() => m_bodyPartRenderer = GetComponent<Renderer>();

        protected void InitializeRigidbody()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            ChangePhysicsSettings(true);
            
        }

        public void ChangePhysicsSettings(bool kinematic)
        {
            m_rigidbody.isKinematic = kinematic;
            m_rigidbody.interpolation = !kinematic? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }
        
        protected void Awake()
        {
            InitializeRigidbody();
            enemyOwner = GetComponentInParent<EnemyProtScript>();
            //InitializeRenderer();
        }
    }
}