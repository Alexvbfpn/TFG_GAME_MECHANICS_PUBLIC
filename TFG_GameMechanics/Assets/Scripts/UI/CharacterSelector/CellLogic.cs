using System;
using DG.Tweening;
using UI.CharacterSelector.ClassHelpers;
using UnityEngine;

namespace UI.CharacterSelector
{
    public class CellLogic : MonoBehaviour
    {
        public CellArtwork cellArtwork { get; protected set; }
        public CellName cellName { get; protected set; }
        public CellBorder cellBorder { get; protected set; }
        
        public CellData cellData { get; protected set; }
        
        public float tweenDuration = 1.0f;
        protected Tween m_borderColorTween;
        
        protected void InitializeCellComponents()
        {
            cellArtwork = GetComponentInChildren<CellArtwork>();
            cellName = GetComponentInChildren<CellName>();
            cellBorder = GetComponentInChildren<CellBorder>();
        }
        
        public void SetCellData(CellData data)
        {
            cellData = data;
            cellArtwork.SetArtwork(data.elementSprite);
            cellName.SetName(data.elementName);
            cellArtwork.CenterArtworkPivot(cellArtwork);
            cellArtwork.ZoomElement(cellArtwork, data.cellZoom);
        }

        public void StartBorderTween()
        {
            cellBorder.borderImage.color = Color.white;
            m_borderColorTween = cellBorder.borderImage.DOColor(Color.red, tweenDuration).SetLoops(-1, LoopType.Yoyo);
        }
        
        public void StopBorderTween()
        {
            m_borderColorTween.Kill();
            cellBorder.borderImage.color = Color.clear;
        }
        
        
        protected void Awake()
        {
            InitializeCellComponents();
        }
    }
}