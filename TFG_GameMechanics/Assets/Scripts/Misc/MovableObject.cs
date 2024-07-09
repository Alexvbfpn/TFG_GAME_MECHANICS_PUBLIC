using System;
using UnityEngine;


namespace Misc
{
    public class MovableObject : MonoBehaviour
    {
        public float speed = 10f;

        protected void Update()
        {
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
    }
}