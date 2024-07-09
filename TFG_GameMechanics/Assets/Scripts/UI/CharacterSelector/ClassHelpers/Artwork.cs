using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelector.ClassHelpers
{
    public class Artwork : MonoBehaviour
    {
        public Image artworkImage { get; protected set; }

        protected Vector2 originalSizeDelta;
        
        protected void SetArtworkObject()
        {
            artworkImage = GetComponent<Image>();
        }
        
        public virtual void SetArtwork(Sprite artwork)
        {
            artworkImage.sprite = artwork;
        }
        
        
        /// <summary>
        /// Con esta funcion conseguimos que el pivot de la imagen se centre en el centro de la imagen.
        /// Se hace que en funcion de sus dimensiones y la posicion de su pivot en pixeles, se normalice y se ponga en el centro.
        /// </summary>
        /// <param name="cellArtwork"></param>
        public void CenterArtworkPivot(Artwork cellArtwork)
        {
            Image artworkImage = cellArtwork.artworkImage;
            Vector2 pixelSize = new Vector2(artworkImage.sprite.texture.width, artworkImage.sprite.texture.height);
            Vector2 pixelPivot = artworkImage.sprite.pivot;
            Vector2 uiPivot = new Vector2(pixelPivot.x / pixelSize.x, pixelPivot.y / pixelSize.y);
            
            artworkImage.rectTransform.pivot = uiPivot;
            
            cellArtwork.SetArtwork(artworkImage.sprite);
        }
        
        public virtual void ZoomElement(Artwork cellArtwork, float zoom)
        {
            cellArtwork.artworkImage.rectTransform.sizeDelta = originalSizeDelta * (zoom);
        }
        
        
        protected virtual void Awake()
        {
            SetArtworkObject();
            originalSizeDelta = artworkImage.rectTransform.sizeDelta;
        }
    }
}