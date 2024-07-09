using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelector.ClassHelpers
{
    public class CellArtwork : Artwork
    {
        public override void ZoomElement(Artwork cellArtwork, float zoom)
        {
            cellArtwork.artworkImage.rectTransform.sizeDelta *= (zoom);
        }
    }
}