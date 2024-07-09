using System;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI
{
    public class UIAmmoCounter : MonoBehaviour
    {
        public Image[] ammoImages;
        
        public Color offColor = Color.gray;
        public Color onColor = Color.white;
        
        protected LanceManager m_lanceManager;
        
        
        protected void InitializeLanceManager()
        {
            m_lanceManager = FindObjectOfType<LanceManager>();
            m_lanceManager.onAmmoChange.AddListener(UpdateAmmoCounter);
        }

        protected void UpdateAmmoCounter(int ammo)
        {
            if (ammo == -1 || ammo == 0)
            {
                for (int i = 0; i < ammoImages.Length; i++)
                {
                    ammoImages[i].color = offColor;
                }
            }
            else
            {
                ammoImages[ammo - 1].color = onColor;
            }
        }
        
        protected void Awake()
        {
            InitializeLanceManager();
            foreach (Image i in ammoImages)
            {
                i.color = offColor;
            }
        }
    }
}