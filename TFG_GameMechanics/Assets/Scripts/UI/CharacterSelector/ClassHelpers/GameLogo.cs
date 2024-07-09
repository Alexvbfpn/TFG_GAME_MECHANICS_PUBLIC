using DG.Tweening;
using UnityEngine;

namespace UI.CharacterSelector.ClassHelpers
{
    public class GameLogo : Artwork
    {
        protected float m_originalAlpha;
        public override void SetArtwork(Sprite artwork)
        {
            if (artwork == null)
            {
                artworkImage.DOFade(0, 0);
            }
            else
            {
                base.SetArtwork(artwork);
                artworkImage.DOFade(m_originalAlpha, 0f);
            }
            
        }
        
        protected override void Awake()
        {
            base.Awake();
            m_originalAlpha = artworkImage.color.a;
        }
    }
}