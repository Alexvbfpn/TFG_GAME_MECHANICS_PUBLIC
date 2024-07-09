using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    public class PlayerAimController : MonoBehaviour
    {
        [Header("Parameters")]
        public float cameraZoomOffset = .3f;
        
        public CanvasGroup reticle;
        
        [Space]
        //Cinemachine Shake
        public CinemachineFreeLook virtualCamera;
        public CinemachineImpulseSource impulseSource;
        
        protected Player m_player;
        
        protected virtual void InitializePlayer() => m_player = GetComponent<Player>(); 

        protected void Aim(bool state, bool changeCamera, float delay)
        {

            // if (walking)
            //     return;
            
            //UI
            float fade = state ? 1 : 0;
            reticle.DOFade(fade, .2f);

            if (!changeCamera)
                return;

            //Camera Offset
            float newAim = state ? cameraZoomOffset : 0;
            float originalAim = !state ? cameraZoomOffset : 0;
            DOVirtual.Float(originalAim, newAim, .5f, CameraOffset).SetDelay(delay);

            virtualCamera.m_XAxis.m_MaxSpeed = state? m_player.stats.current.cameraHorizontalAimingSpeed : m_player.stats.current.cameraHorizontalRotationSpeed;
            virtualCamera.m_YAxis.m_MaxSpeed = state? m_player.stats.current.cameraVerticalAimingSpeed : m_player.stats.current.cameraVerticalRotationSpeed;
        }
        
        void CameraOffset(float offset)
        {
            virtualCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(offset, 0.4f, 0);
            virtualCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(offset, 0.5f, 0);
            virtualCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(offset, 0.4f, 0);
            
            
        }
        
        protected void Start()
        {
            InitializePlayer();
            
            m_player.events.onAim.AddListener((x) => Aim(x, true, 0));
            //m_player.events.onStopAim.AddListener(() => Aim(false, true, 0));
            
            Cursor.visible = false;
            
            reticle.DOFade(0, 0);
        }
    }
}