using System.Collections.Generic;
using System.Linq;
using Items;
using Misc.SeparableObjects;
using UnityEngine;
using Utils;

namespace Misc
{
    public class TestCamera : MonoBehaviour
    {
        [SerializeField] protected Camera m_camera;
        [SerializeField] protected Photo m_photo;
       
        public Plane[] m_frustumPlanes = new Plane[6]; //= GeometryUtility.CalculateFrustumPlanes(Camera.main);

        public List<GameObject> m_planes;
        [SerializeField]
        protected List<Cuttable> m_cuttablesProjected;

        protected GameObject m_PhotoParent;

        public void Start()
        {
            // Calculate the planes from the main camera's view frustum
            m_frustumPlanes = GeometryUtility.CalculateFrustumPlanes(m_camera);
            // Create a "Plane" GameObject aligned to each of the calculated planes
            for (int i = 0; i < 6; ++i)
            {
                GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
                p.layer = LayerMask.NameToLayer("FrustumPlane");
                p.transform.parent = transform;
                p.name = "Plane " + i.ToString();
                p.transform.position = -m_frustumPlanes[i].normal * m_frustumPlanes[i].distance;
                p.transform.rotation = Quaternion.FromToRotation(Vector3.up, m_frustumPlanes[i].normal);
                m_planes.Add(p);
            }
        }
        [ContextMenu("Cheese")]
        public void TestCheese()
        {
            m_frustumPlanes = GeometryUtility.CalculateFrustumPlanes(m_camera);
            ResetProjections();
            
            Cuttable[] cuttableProjections = FindObjectsOfType<Cuttable>().Where(c => c.isActiveAndEnabled).ToArray();

            // Background
            // Picture
            m_photo.SetRendererMaterialTexture(CameraUtils.TakeScreenShot(m_camera));
            
            foreach (var p in cuttableProjections)
            {
                if(!p.gameObject.activeInHierarchy) continue;
                
                Renderer renderer = p.TryGetComponent(out renderer) ? renderer : GetComponentInChildren<Renderer>();

                if (!renderer) continue;

                var bounds = renderer.bounds;
                bounds = p.GetComponent<Collider>().bounds;
                if(GeometryUtility.TestPlanesAABB(m_frustumPlanes, bounds))
                    m_cuttablesProjected.Add(p);
                    //objectsInCamera.Add(p.gameObject);
            }
            
            m_photo.gameObject.SetActive(true);

            CopyObjects();
            Debug.Log("Cheese");
        }
        
        protected void CopyObjects()
        {
            m_PhotoParent = new GameObject("PhotoParent");
            m_PhotoParent.transform.position = m_camera.transform.position;
            m_PhotoParent.transform.rotation = m_camera.transform.rotation;
            
            foreach (var cuttable in m_cuttablesProjected)
            {
                var copy = Instantiate(cuttable, m_PhotoParent.transform, true);
                var rendererCopy = copy.renderer;
                var rendererOriginal = cuttable.renderer;
                copy.SetAsCopy();
                
                //Cortamiento (Creo)
                
            }
            m_PhotoParent.SetActive(false);
        }
        
        protected void ResetProjections()
        {
            m_cuttablesProjected.Clear();
        }
        
        private void Update()
        {
            // m_frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            // int i = 0;
            // foreach (GameObject p in m_planes)
            // {
            //     p.transform.position = -m_frustumPlanes[i].normal * m_frustumPlanes[i].distance;
            //     p.transform.rotation = Quaternion.FromToRotation(Vector3.up, m_frustumPlanes[i].normal);
            //     i++;
            // }
        }

        protected void OnDrawGizmos()
        {
            // float near = m_camera.nearClipPlane;
            // float far = m_camera.farClipPlane;
            // float fov = m_camera.fieldOfView;
            //
            // float height = Mathf.Tan(fov * Mathf.Deg2Rad * 0.5f) * near;
            // Vector3 s = new Vector3(0, height * near, near);
            // Vector3 e = new Vector3(0, height * far, far);
            //
            // Gizmos.DrawLine(s, e);
            //
            // float width = height * 16f / 9f;//m_camera.aspect;
            // Vector3 s1 = new Vector3(width * near, height * near, near);
            // Vector3 e1 = new Vector3(width * far, height * far, far);
            //
            // Gizmos.DrawLine(s1, e1);
        }
    }
}