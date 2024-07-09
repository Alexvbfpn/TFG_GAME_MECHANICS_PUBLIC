using System;
using Patterns.ObjectPool.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;

namespace Patterns.ObjectPool
{
    public class AObjectPoolManager<T> : MonoBehaviour where T : MonoBehaviour, IPooleableObject
    {
        public T elementPrototype;
        public int initialNumberOfElements = 5;
        public bool allowNewElements = true;
        public bool spawning = false;
        public float elementsPerSecond = 1;

        protected ObjectPool _elementsPool;

        protected void Start()
        {
            Assert.IsTrue(elementPrototype is IPooleableObject);
            
            _elementsPool = new ObjectPool(elementPrototype, initialNumberOfElements, allowNewElements);
        }
        
        protected T CreatePoolElement(T prototype)
        {
            T element = (T)_elementsPool.Get();
            if(element)
            {
                element.elementPool = _elementsPool;
            }

            return element;
        }

    }
}