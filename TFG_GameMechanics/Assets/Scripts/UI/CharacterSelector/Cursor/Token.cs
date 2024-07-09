using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelector
{
    public class Token : MonoBehaviour
    {
        public bool isGrabbed;
        
        public Transform followTransform;
        
        public Image tokenImage { get; protected set; }
        public int playerIndex { get; protected set; }
        protected void InitializeTokenImage() => tokenImage = GetComponent<Image>();
        
        public void SetPlayerIndex(int index)
        {
            playerIndex = index;
        }
        
        public void SetGrabbed(bool value)
        {
            isGrabbed = value;
            tokenImage.raycastTarget = !value;
        }

        protected void Awake()
        {
            InitializeTokenImage();
        }

        protected void Update()
        {
            if (isGrabbed)
            {
                transform.position = followTransform.position;
            }
        }
        
        
    }
}