using System;
using GameMechanics.EntitiesSystem;
using GameMechanics.EntitiesSystem.GowPlayer;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ThirdPersonCrosshairManager : MonoBehaviour
    {
        public Image crosshairImage;
        public GameObject axeCrosshair;
        public GameObject lanceCrosshair;
        
        protected ArmedPlayer _player;
        protected PlayerAimController _aimController;
        protected void InitializePlayer() => _player = FindObjectOfType<ArmedPlayer>();

        protected void InitializeAimController() => _aimController = FindObjectOfType<PlayerAimController>();
        
        protected void UpdateCrosshair(float axis)
        {
            if (axis > 0)
            {
                lanceCrosshair.SetActive(false);
                axeCrosshair.SetActive(true);
                _aimController.reticle = axeCrosshair.GetComponent<CanvasGroup>();
            }
            else
            {
                axeCrosshair.SetActive(false);
                lanceCrosshair.SetActive(true);
                _aimController.reticle = lanceCrosshair.GetComponent<CanvasGroup>();
            }
        }

        protected void Awake()
        {
            InitializePlayer();
            InitializeAimController();
            _player.onWeaponSwap.AddListener(UpdateCrosshair);
        }
    }
}