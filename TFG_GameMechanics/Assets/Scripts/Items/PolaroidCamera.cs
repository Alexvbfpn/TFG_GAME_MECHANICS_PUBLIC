using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMechanics;
using Misc;
using Misc.SeparableObjects;
using UnityEngine;
using Utils;

namespace Items
{
    public class PolaroidCamera : ItemController
    {
        [SerializeField] protected Camera m_camera;
        [Tooltip("Reference to a Camera that can only see the Skybox, for the photo in background.")]
        [SerializeField] protected Camera m_backgroundCamera;
        [SerializeField] protected Photo m_photo;
       
        public Plane[] m_frustumPlanes = new Plane[6]; //= GeometryUtility.CalculateFrustumPlanes(Camera.main);

        public List<GameObject> m_planes;

        public ItemPositionInfo offsetPhoto;

        protected Mover m_renderPlane;

        protected void InitializeRenderPlane() => m_renderPlane = GetComponentInChildren<Mover>();
        
        protected void Start()
        {
            // Calculate the planes from the main camera's view frustum
            m_frustumPlanes = GeometryUtility.CalculateFrustumPlanes(m_camera);
            // Create a "Plane" GameObject aligned to each of the calculated planes
            for (int i = 0; i < 6; ++i)
            {
                GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
                p.layer = LayerMask.NameToLayer("FrustumPlane");
                p.transform.parent = transform;
                p.GetComponent<MeshCollider>().enabled = false;
                p.name = "Plane " + i.ToString();
                p.transform.position = -m_frustumPlanes[i].normal * m_frustumPlanes[i].distance;
                p.transform.rotation = Quaternion.FromToRotation(Vector3.up, m_frustumPlanes[i].normal);
                m_planes.Add(p);
            }
        }
        
        [ContextMenu("Cheese")]
        protected void SayCheese()
        {
            m_frustumPlanes = GeometryUtility.CalculateFrustumPlanes(m_camera);
            m_photo.ResetPhotoElements();
            
            Cuttable[] cuttableProjections = FindObjectsOfType<Cuttable>().Where(c => c.isActiveAndEnabled).ToArray();

            // Picture
            m_photo.SetRendererMaterialTexture(CameraUtils.TakeScreenShot(m_camera));
            // Background
            m_photo.backgroundTexture = CameraUtils.TakeScreenShot(m_backgroundCamera);
            
            foreach (var p in cuttableProjections)
            {
                if(!p.gameObject.activeInHierarchy) continue;
                
                Renderer renderer = p.TryGetComponent(out renderer) ? renderer : GetComponentInChildren<Renderer>();

                if (!renderer) continue;

                var bounds = renderer.bounds;
                bounds = p.GetComponent<Collider>().bounds;
                if(GeometryUtility.TestPlanesAABB(m_frustumPlanes, bounds))
                    m_photo.AddElementToPhoto(p.gameObject);
            }

            ShowPhoto();
            
            m_photo.CopyObjectsInPhoto(m_frustumPlanes);

            m_photo.SetBackgroundImage();

            Debug.Log("Cheese");
        }

        protected void ShowPhoto()
        {
            m_photo.Show();
        }

        protected override void OnUse()
        {
            SayCheese();
            StartCoroutine(SpecialAction());
        }
        
        protected override IEnumerator SpecialAction()
        {
            MoveToSpecialPosition();
            yield return new WaitForSeconds(1.5f);
            MoveToInactivePosition();
        }

        public override void ItemInitializations()
        {
            InitializeRenderPlane();
            m_photo.SetCamera(m_camera.transform);
            
            // EVENTS SUBSCRIPTIONS
            itemEvents.onMoveAimPosition.AddListener(() => m_renderPlane.ApplyOffset());
            itemEvents.onMoveDefaultPosition.AddListener(() => m_renderPlane.ResetMover());
            itemEvents.onMoveSpecialPosition.AddListener(() => m_renderPlane.ResetMover());
        }

        public override void MoveToSpecialPosition()
        {
            itemEvents.onMoveSpecialPosition?.Invoke();
            
            GameLayers.SetLayerToGameObject(skin, _inactivePositionLayer);
            MoveItem(itemPositions.GetValueByKey(ItemPositionType.Special))
                .OnComplete
                (() => m_photo.MoveOffset(offsetPhoto.transform.localPosition, offsetPhoto.movementDuration));
        }
    }
}