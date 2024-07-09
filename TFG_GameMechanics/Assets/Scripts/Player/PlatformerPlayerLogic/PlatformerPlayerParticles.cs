using DG.Tweening;
using UnityEngine;

namespace GameMechanics.EntitiesSystem.PlatformerPlayerLogic
{
    public class PlatformerPlayerParticles : PlayerParticles<PlatformerPlayer>
    {
        [SerializeField] private Renderer distortionRenderer;
        [SerializeField] private Renderer[] characterRenderers;

        private Material[] m_rendererMaterials;
        
        public float fresnelAmount = 5f;
        public float fresnelEdge = 4f;
        public float chargingFresnelSpeed = 12;
        public float boostingFresnelSpeed = 10;
        [Header("Fresnel Colors")]
        [ColorUsage(true,true)]
        [SerializeField]
        private Color runColor, sparkColor;

        private readonly int m_distortionEffectAmount = Shader.PropertyToID("_EffectAmount");
        
        private readonly int m_fresnelAmount = Shader.PropertyToID("_FresnelAmount");
        private readonly int m_fresnelEdge = Shader.PropertyToID("_FresnelEdge");
        private readonly int m_blinkFresnel = Shader.PropertyToID("_BlinkFresnel");
        private readonly int m_extraBlink = Shader.PropertyToID("_ExtraBlink");
        private readonly int m_fresnelColor = Shader.PropertyToID("_FresnelColor");
        private readonly int m_extraShineAmount = Shader.PropertyToID("_ExtraShineAmount");

        protected void InitializeCharacterRenderers()
        {
            m_rendererMaterials = new Material[characterRenderers.Length];
            for (int i = 0; i < characterRenderers.Length; i++)
            {
                m_rendererMaterials[i] = characterRenderers[i].material;
            }
        }
        
        protected override void InitializePlayerEvents()
        {
            _player.platformerEvents.onBoost.AddListener(BoostEffectsLogic);
            _player.platformerEvents.onUnchargingBoost.AddListener(() => ChangeRenderersState(SpeedBoosterTypes.None));
            _player.platformerEvents.onChargingBoost.AddListener(() => ChangeRenderersState(SpeedBoosterTypes.ChargingSpeedBooster));
            
            _player.platformerEvents.onBoost.AddListener(x => ChangeDistortionEffect(x? 1.0f : 0.0f));
            
            
            _player.platformerEvents.onShinesparkCharged.AddListener(() => ChangeRenderersState(SpeedBoosterTypes.ShineSpark));
            _player.platformerEvents.onShinesparkUncharged.AddListener(() => ChangeRenderersState(SpeedBoosterTypes.None));
        }

        protected void BoostEffectsLogic(bool x)
        {
            if (!_player.isShinesparkStored)
            {
                ChangeRenderersState(x? SpeedBoosterTypes.Boosting : SpeedBoosterTypes.None);
            }
        }
        
        public void ChangeRenderersState(SpeedBoosterTypes type)
        {
            switch (type)
            {
                case SpeedBoosterTypes.None:
                    MaterialChange(0, fresnelEdge, 0, 0, runColor);
                    break;
                
                case SpeedBoosterTypes.Boosting:
                    MaterialChange(fresnelAmount, fresnelEdge, 1, 1, runColor);
                    break;
                
                case SpeedBoosterTypes.ChargingSpeedBooster:
                    MaterialChange(fresnelAmount, fresnelEdge, 1, 0, runColor);
                    break;
                
                case SpeedBoosterTypes.ShineSpark:
                    MaterialChange(fresnelAmount, fresnelEdge, 0, 0, sparkColor);
                    DOVirtual.Float(0, 1, 0.1f, BlinkMaterial).OnComplete(() => DOVirtual.Float(1, 0, 0.3f, BlinkMaterial));
                    break;
            }
        }
        
        protected void MaterialChange(float fresnelAmount, float fresnelEdge, int blinkFresnel,int extraBlink, Color fresnelColor)
        {
            //Logging.Log("Material Change");
            foreach (Material m in m_rendererMaterials)
            {
                m.SetFloat(m_fresnelAmount, fresnelAmount);
                m.SetFloat(m_fresnelEdge, fresnelEdge);
                m.SetInt(m_blinkFresnel, blinkFresnel);
                m.SetInt(m_extraBlink, extraBlink);
                m.SetColor(m_fresnelColor, fresnelColor);
            }
        }

        protected void ChangeDistortionEffect(float amount)
        {
            distortionRenderer.material.SetFloat(m_distortionEffectAmount, amount);
        }

        #region --- SHINESPARK ---
        
        protected void ChargeShinespark()
        {
            
        }
        
        protected void BlinkMaterial(float x)
        {
            foreach (Material mat in m_rendererMaterials)
            {
                mat.SetFloat(m_extraShineAmount, x);
            }
        }
        
        void FresnelChange(float fresnelAmount)
        {
            foreach (Material m in m_rendererMaterials)
            {
                m.SetFloat(m_fresnelAmount, fresnelAmount);
            }
        }
        
        #endregion
        
        protected override void Awake()
        {
            base.Awake();
            InitializeCharacterRenderers();            
        }
    }
}