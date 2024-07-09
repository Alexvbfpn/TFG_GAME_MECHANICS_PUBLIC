using System;
using UnityEngine;

namespace GameMechanics
{
    [Serializable]
    public class GameLevel
    {
        [Header("General Settings")]
        public string name;
        public string scene;
        public string description;
        public Sprite image;
        
        [Header("Locking Settings")]
        //This variable can be used for the levels that are not accessible for WEBGL version.
        [Tooltip("This level will be inaccessible from the level selection unless manually unlocked from code.")]
        public bool locked;
        
        
    }
}