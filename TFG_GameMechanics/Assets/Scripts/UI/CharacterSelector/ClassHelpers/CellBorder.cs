using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelector.ClassHelpers
{
    public class CellBorder : MonoBehaviour
    {
        public Image borderImage { get; protected set; }

        protected void SetBorderObject()
        {
            borderImage = GetComponent<Image>();
        }
        
        public void SetBorder(Sprite artwork)
        {
            borderImage.sprite = artwork;
        }
        
        
        
        protected void Awake()
        {
            SetBorderObject();
        }
    }
}