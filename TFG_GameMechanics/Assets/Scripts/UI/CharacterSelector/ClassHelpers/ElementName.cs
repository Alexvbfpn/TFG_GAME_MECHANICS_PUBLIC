using TMPro;
using UnityEngine;

namespace UI.CharacterSelector.ClassHelpers
{
    public class ElementName : MonoBehaviour
    {
        public TextMeshProUGUI nameText { get; protected set; }
        
        protected void SetNameObject()
        {
            nameText = GetComponent<TextMeshProUGUI>();
        }
        
        public void SetName(string name)
        {
            nameText.text = name;
        }

        protected void Awake()
        {
            SetNameObject();
        }
    }
}