using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IEnumerable
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public void Add(TKey key, TValue value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public int Count
        {
            get { return keys.Count; }
        }

        public TKey GetKey(int index)
        {
            return keys[index];
        }

        public TValue GetValue(int index)
        {
            return values[index];
        }
        
        public TValue GetValueByKey(TKey key)
        {
            int index = keys.IndexOf(key);
            if (index != -1)
            {
                return values[index];
            }
            else
            {
                throw new KeyNotFoundException("Key not found in dictionary.");
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}