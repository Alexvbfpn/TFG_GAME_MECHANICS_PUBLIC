using UnityEngine;

namespace Misc.SeparableObjects
{
    // [RequireComponent(typeof(Renderer))]
    // [RequireComponent(typeof(MeshFilter))]
    [AddComponentMenu("Mechanics/SeparableObjects/Cuttable")]
    public class Cuttable : MonoBehaviour
    {
        private static Material DefaultMaterial;
        static readonly string ShaderString = "Universal Render Pipeline/Lit";
        
        
        [Tooltip("The material assigned to the new triangles created by the planes intersections")]
        [SerializeField] Material CuttingMaterial = null;

        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

        public bool isCopy { get; private set; } = false;
        
        public Renderer renderer {get; protected set;} = null;
        
        protected void InitializeRenderer() => renderer = GetComponent<Renderer>()? GetComponent<Renderer>() : GetComponentInChildren<Renderer>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeMaterials()
        {
            if(DefaultMaterial) return;

            if (!Shader.Find(ShaderString))
            {
                Debug.LogError($"Shader {ShaderString} not found.");
                return;
            }
            
            // We create a default material for the cuttable objects
            DefaultMaterial = new Material(Shader.Find(ShaderString))
            {
                color = Color.gray
            };
            
            DefaultMaterial.SetInt(Smoothness, 0);
        }

        protected void Awake()
        {
            InitializeRenderer();
        }
        
        protected void Start()
        {
            if(!CuttingMaterial || !CuttingMaterial.shader)
                CuttingMaterial = DefaultMaterial;
        }
        
        public void SetAsCopy()
        {
            if(isCopy) return;
            
            isCopy = true;
            Debug.Log(this.gameObject);
            renderer.materials = new Material[]
            {
                renderer.material,
                CuttingMaterial
            };
        }
        
    }
}