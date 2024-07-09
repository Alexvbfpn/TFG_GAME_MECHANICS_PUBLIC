using System;
using UnityEngine;

namespace UI.CharacterSelector
{
    [Serializable]
    [CreateAssetMenu(fileName = "Cell Data", menuName = "Smash/Cell Data", order = 0)]
    public class CellData : ScriptableObject
    {
        public string elementName;
        public Sprite elementSprite;
        public Sprite gameLogo;
        public float cellZoom = 1.0f;
        public float slotZoom = 1.0f;
    }
}