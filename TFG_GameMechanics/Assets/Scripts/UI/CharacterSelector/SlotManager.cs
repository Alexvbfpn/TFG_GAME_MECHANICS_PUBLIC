using UI.CharacterSelector.ClassHelpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelector
{
    public class SlotManager : MonoBehaviour
    {
        public Image[] slotColorImages;
        
        public SlotArtwork slotArtwork { get; protected set; }
        public ElementName elementName { get; protected set; }
        public GameLogo[] gameLogos { get; protected set; }
        public PlayerNumber playerNumber { get; protected set; }
        public PlayerNickName playerNickName { get; protected set; }
        
        
        protected void InitializeSlotComponents()
        {
            slotArtwork = GetComponentInChildren<SlotArtwork>();
            elementName = GetComponentInChildren<ElementName>();
            gameLogos = GetComponentsInChildren<GameLogo>();
            playerNumber = GetComponentInChildren<PlayerNumber>();
            playerNickName = GetComponentInChildren<PlayerNickName>();
        }
        
        public void SetSlotData(CellData elementData)
        {
            slotArtwork.SetArtwork(elementData != null ? elementData.elementSprite : null);
            if (elementData != null)
            {
                slotArtwork.CenterArtworkPivot(slotArtwork);
                slotArtwork.ZoomElement(slotArtwork, elementData.slotZoom);
            }
            elementName.SetName(elementData != null ? elementData.elementName : string.Empty);
            foreach (GameLogo g in gameLogos)
            {
                g.SetArtwork(elementData != null ? elementData.gameLogo : null);
            }
        }

        public void SetPlayerData(int player)
        {
            playerNumber.SetName("P" + (player +1));
            playerNickName.SetName("Player " + (player +1));
        }
        
        public void SetSlotColor(Color color)
        {
            foreach (Image a in slotColorImages)
            {
                a.color = color;
            }
        }
        
        protected void Awake()
        {
            InitializeSlotComponents();
        }
        
        
    }
}