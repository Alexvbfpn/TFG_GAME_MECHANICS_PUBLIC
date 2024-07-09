using GameMechanics.EntitiesSystem;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Misc
{
    public enum AimType
    {
        Default,
        Interact,
        Holding,
        Aim
    }
    public class AimSightChanger : Singleton<AimSightChanger>
    {
        public Image interactSight;
        
        public SerializableDictionary<AimType, Sprite> sightSprites = new SerializableDictionary<AimType, Sprite>();

        
        protected Player m_player;
        
        protected void InitializePlayer() => m_player = FindObjectOfType<Player>();
        
        public void ChangeSightSprite(AimType aimType)
        {
            interactSight.sprite = sightSprites.GetValueByKey(aimType);
        }
        
        protected void Awake()
        {
            InitializePlayer();
            ChangeSightSprite(AimType.Default);

            m_player.events.onPickUp.AddListener(() => ChangeSightSprite(AimType.Holding));
            m_player.events.onThrowPickable.AddListener(() => ChangeSightSprite(AimType.Interact));
        }
        
        
    }
   
}