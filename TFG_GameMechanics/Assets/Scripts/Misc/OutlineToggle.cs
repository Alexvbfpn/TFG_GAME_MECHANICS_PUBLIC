using UnityEngine;

namespace Misc
{
    public class OutlineToggle : MonoBehaviour
    {
        protected Material m_material;
        protected float m_outlineDefaultWidth;
        
        protected void InitializeMaterial() => m_material = TryGetComponent(out MeshRenderer meshRenderer) ? meshRenderer.material : GetComponentInChildren<MeshRenderer>().material;
        
        protected void InitializeOutlineDefaultWidth() => m_outlineDefaultWidth = m_material.GetFloat("_OutlineWidth");
        
        public void ToggleOutline(bool state)
        {
            SetOutlineWidth(state ? m_outlineDefaultWidth : 0.0f);
        }
        
        protected void SetOutlineWidth(float width)
        {
            m_material.SetFloat("_OutlineWidth", width);
        }
        
        protected void Awake()
        {
            InitializeMaterial();
            InitializeOutlineDefaultWidth();
            ToggleOutline(false);
        }
        
        
    }
}