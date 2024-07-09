using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Misc.SeparableObjects;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Items
{
    public class Photo : ItemController
    {
        [SerializeField] protected Renderer m_renderer;
        [SerializeField] protected RawImage m_rawImage;
        protected GameObject m_photoOutputParent;

        List<GameObject> m_elementsInPhoto = new List<GameObject>();
        protected List<GameObject> m_elementsInFrustum = new List<GameObject>();

        protected Transform m_camera;
        
        protected Transform m_initialParent;
        
        #region --- PHOTO BACKGROUND VARIABLES ---
        [SerializeField] protected string m_backgroundShaderString = "Unlit/Texture";
        [HideInInspector] public Texture backgroundTexture;
        [HideInInspector] public Material backgroundMaterial = null;
        [HideInInspector] public Mesh backgroundMesh;
        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

        #endregion

        #region --- INITIALIZATION ---

        protected void InitializeRenderer() => m_renderer = GetComponent<Renderer>()? GetComponent<Renderer>() : GetComponentInChildren<Renderer>();
        protected void InitializeBackground()
        {
            InitializeBackgroundMaterial();
            InitializeBackgroundMesh();
        }
        protected void InitializeBackgroundMaterial()
        {
            //Debug.Log("JUST BEFORE Initializing Background Material");
            if (backgroundMaterial == null)
            {
                //Debug.Log("Initializing Background Material");
                backgroundMaterial = new Material(Shader.Find(m_backgroundShaderString))
                {
                    mainTexture = backgroundTexture,
                    color = Color.white
                };
                backgroundMaterial.SetInt(Smoothness, 0);
            }
        }
        protected void InitializeBackgroundMesh()
        {
            if(backgroundMesh) return;

            Camera camera = m_camera.GetComponent<Camera>();
            
            var distance = camera.farClipPlane / 4;
            var height = distance * Mathf.Tan(camera.fieldOfView * (0.5f) * Mathf.Deg2Rad); // old number 0.3175f

            backgroundMesh = new Mesh
            {
                vertices = new[]
                {
                    new Vector3(-height, -height, distance),
                    new Vector3(height, -height, distance),
                    new Vector3(height, height, distance),
                    new Vector3(-height, height, distance)
                },
                //WE USE THIS METHOD BECAUSE WE WANT TO SEE DE BACK FACE OF THE PLANE
                triangles = new[]
                {
                    2, 1, 0, // Primer triángulo (2, 1, 0) en sentido antihorario
                    3, 2, 0  // Segundo triángulo (3, 2, 0) en sentido antihorario
                },
                uv = new[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1)
                }
            };
            
            // backgroundMesh = new Mesh
            // {
            //     vertices = new[]
            //     {
            //         new Vector3(-height, -height, distance),
            //         new Vector3(height, -height, distance),
            //         new Vector3(height, height, distance),
            //         new Vector3(-height, height, distance)
            //     },
            //     triangles = new[]
            //     {
            //         0, 1, 2,
            //         0, 2, 3
            //     },
            //     uv = new[]
            //     {
            //         new Vector2(0, 0),
            //         new Vector2(1, 0),
            //         new Vector2(1, 1),
            //         new Vector2(0, 1)
            //     }
            // };
        }
        
        #endregion
        
        public GameObject GetPhotoOutputParent() => m_photoOutputParent;
        public void SetCamera(Transform objectsCamera) => m_camera = objectsCamera;
        
        #region -- RENDERER -- 
        
        public Renderer GetRenderer() => m_renderer;

        public void SetRendererMaterial(Material material)
        {
            if (!m_renderer) return;
            m_renderer.material = material;
        }
        
        public void SetRendererMaterialTexture(Texture texture)
        {
            if (!m_renderer) return;
            m_renderer.material.mainTexture = texture;
            m_renderer.material.mainTextureOffset = new Vector2(0.25f, 0.25f);
            m_renderer.material.DOTiling(new Vector2(0.5f, 0.5f), 0.5f);
            m_rawImage.texture = texture;
        }
        
        #endregion

        #region --- PHOTO OUTPUT OBJECTS REGION ---

        public void AddElementToPhoto(GameObject element)
        {
            m_elementsInPhoto.Add(element);
        }

        public void ResetPhotoElements()
        {
            m_elementsInPhoto.Clear();
        }

        public void CopyObjectsInPhoto(Plane[] planes)
        {
            m_photoOutputParent = new GameObject("PhotoOutput");
            m_photoOutputParent.transform.position = m_camera.position;
            m_photoOutputParent.transform.rotation = m_camera.rotation;
            foreach (var element in m_elementsInPhoto)
            {
                if (element.TryGetComponent(out Cuttable cuttable))
                {
                    var copy = Instantiate(cuttable, m_photoOutputParent.transform, true);
                    var rendererCopy = copy.renderer;
                    var rendererOriginal = cuttable.renderer;
                    copy.SetAsCopy();
                    
                    //Lightmaps
                    SetCopyLightmaps(rendererCopy, rendererOriginal);
                    //Cortamiento (Creo)
                    MeshUtils.CutByPlanes(copy, planes);
                }
            }
            m_photoOutputParent.SetActive(false);
        }
        
        protected void SetCopyLightmaps(Renderer rendererCopy, Renderer rendererOriginal)
        {
            rendererCopy.lightmapIndex = rendererOriginal.lightmapIndex;
            rendererCopy.lightmapScaleOffset = rendererOriginal.lightmapScaleOffset;
        }
        
        #endregion
        
        #region --- BACKGROUND IMAGE ---
        public void SetBackgroundImage()
        {
            var backgroundRenderer = GetPhotoOutputParent().AddComponent<MeshRenderer>();
            backgroundRenderer.material = backgroundMaterial;
            backgroundRenderer.material.mainTexture = backgroundTexture;
            GetPhotoOutputParent().AddComponent<MeshFilter>().sharedMesh = backgroundMesh;
        }
        #endregion
        
        public override void MoveToDefaultPosition()
        {
            ChangeParent(true);
            base.MoveToDefaultPosition();
        }
        
        public override void MoveToInactivePosition()
        {
            ChangeParent(false);
            base.MoveToInactivePosition();
        }
        
        public override void MoveToAimingPosition()
        {
            base.MoveToAimingPosition();
            m_rawImage.color = Color.white;
        }
        
        protected override void OnUse()
        {
            m_photoOutputParent.SetActive(true);
            SetOutputPosition();
            // m_photoOutputParent.transform.position = m_camera.position; // + m_camera.transform.forward * 4.424f;
            // m_photoOutputParent.transform.rotation = m_camera.rotation;
            MoveToInactivePosition();
            isUsing = false;
            
            m_rawImage.color = Color.clear;
        }

        protected void SetOutputPosition()
        {
            m_photoOutputParent.transform.position = m_camera.position; // + m_camera.transform.forward * 4.424f;
            m_photoOutputParent.transform.rotation = m_camera.rotation;
        }
        
        public void MoveOffset(Vector3 offset, float duration)
        { 
            Debug.Log("Offset: " + offset);
            transform.DOLocalMove(offset, duration).SetEase(Ease.InOutSine).OnComplete(MoveToDefaultPosition);
            Debug.Log("Moving to offset");
        }

        protected void ChangeParent(bool free)
        {
            Debug.Log("Change Parent");
            transform.parent = free ? m_initialParent : FindObjectOfType<PolaroidCamera>().transform;
        }

        protected override IEnumerator SpecialAction()
        {
            throw new NotImplementedException();
        }

        public override void ItemInitializations()
        {
            InitializeRenderer();
            InitializeBackground();
            m_initialParent = transform.parent;
            ChangeParent(false);
        }
    }
}