using System;
using UnityEngine;

namespace Items
{
    [Serializable]
    public class ItemPositionInfo
    {
        public Transform transform;
        public float movementDuration;
        public AnimationCurve movementEase;
    }
}