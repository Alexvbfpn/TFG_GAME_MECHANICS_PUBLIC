using UnityEngine;

namespace Items.Weapons
{
    [System.Serializable]
    public struct CrosshairData
    {
        [Tooltip("Image used as this weapon's crosshair")]
        public Sprite CrosshairSprite;
        
        [Tooltip("Size of the crosshair image")]
        public int CrosshairSize;
        
        [Tooltip("Color of the crosshair image")]
        public Color CrosshairColor;
        
        [Tooltip("Rotation of the crosshair image")]
        public Vector3 CrosshairRotation;
    }
}