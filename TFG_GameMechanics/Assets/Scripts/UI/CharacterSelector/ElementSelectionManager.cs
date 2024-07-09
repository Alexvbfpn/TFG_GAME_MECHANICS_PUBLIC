using System;
using DG.Tweening;
using Misc;
using UnityEngine;

namespace UI.CharacterSelector
{
    public class ElementSelectionManager : Singleton<ElementSelectionManager>
    {
        public Transform playerSlotsContainer;

        public CellData confirmedElement { get; protected set; }
        
        public void ShowElementInSlot(int player, CellData elementData)
        {
            bool nullElement = elementData == null;
            
            SlotManager slotManager = playerSlotsContainer.GetChild(player).GetComponent<SlotManager>();
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(slotManager.slotArtwork.transform.DOLocalMoveX(-300, 0.05f).SetEase(Ease.OutCubic));
            sequence.AppendCallback(() => slotManager.SetSlotData(elementData));
            sequence.AppendCallback(() => slotManager.slotArtwork.artworkImage.color = nullElement ? Color.clear : Color.white);
            sequence.Append(slotManager.slotArtwork.transform.DOLocalMoveX(300, 0f));
            sequence.Append(slotManager.slotArtwork.transform.DOLocalMoveX(0, 0.05f).SetEase(Ease.OutCubic));
            
            slotManager.SetPlayerData(player);
            
            //playerSlot.SetSlotData(elementData);
            
            
        }
        
        public void ConfirmCharacter(int player, CellData elementData)
        {
            if (confirmedElement == null)
            {
                confirmedElement = elementData;
                playerSlotsContainer.GetChild(player).DOPunchPosition(Vector3.down * 5, 0.3f, 10, 1);
            }
        }

        protected void CancelCharacter()
        {
            confirmedElement = null;
        }
        
        protected void Start()
        {
            FindObjectOfType<CursorLogic>().onConfirm.AddListener(ConfirmCharacter);
            FindObjectOfType<CursorLogic>().onCancel.AddListener(CancelCharacter);
        }
    }
}