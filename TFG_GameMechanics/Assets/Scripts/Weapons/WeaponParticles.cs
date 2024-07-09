using System;
using Items.Weapons.Controllers;
using UnityEngine;

namespace Weapons
{
    public class WeaponParticles : MonoBehaviour
    {
        public ParticleSystem shootParticle;
        
        public void PlayShootParticle()
        {
            shootParticle.Play();
        }
        
        public void StopShootParticle()
        {
            shootParticle.Stop();
        }

        public void Awake()
        {
            GetComponent<WeaponController>().OnShoot += PlayShootParticle;
        }
    }
}