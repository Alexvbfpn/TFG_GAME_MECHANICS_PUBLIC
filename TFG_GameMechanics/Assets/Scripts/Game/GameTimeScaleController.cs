using System;
using UnityEngine;

namespace GameMechanics
{
    public class GameTimeScaleController : MonoBehaviour
    {
        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.B))
                Time.timeScale = 0.1f;
            
            if(Input.GetKeyDown(KeyCode.N))
                Time.timeScale = 1f;
        }
    }
}