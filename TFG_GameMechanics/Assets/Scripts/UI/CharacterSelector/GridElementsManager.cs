using System;
using System.Collections.Generic;
using GameMechanics;
using TMPro;
using UI.CharacterSelector.ClassHelpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelector
{
    public class GridElementsManager : MonoBehaviour
    {
        public List<CellData> elements = new List<CellData>();
        
        public GameObject cellPrefab;
        
        protected void SpawnElements()
        {
            foreach (var element in elements)
            {
                SpawnElementCell(element);
            }
        }
        
        protected void SpawnElementCell(CellData element)
        { 
            GameObject cell = Instantiate(cellPrefab, transform);
            cell.name = element.elementName;
            
            CellLogic cellLogic = cell.GetComponent<CellLogic>();
            cellLogic.SetCellData(element);
            
            // CellArtwork cellArtwork = cell.GetComponentInChildren<CellArtwork>();
            // CellName cellName = cell.GetComponentInChildren<CellName>();
            //
            // cellArtwork.SetArtwork(element.elementSprite);
            // cellName.SetName(element.elementName);
            
            // CenterArtworkPivot(cellLogic.cellArtwork);
            // ZoomElement(cellLogic.cellArtwork, element.zoom);
        }
        // /// <summary>
        // /// Con esta funcion conseguimos que el pivot de la imagen se centre en el centro de la imagen.
        // /// Se hace que en funcion de sus dimensiones y la posicion de su pivot en pixeles, se normalice y se ponga en el centro.
        // /// </summary>
        // /// <param name="cellArtwork"></param>
        // protected void CenterArtworkPivot(CellArtwork cellArtwork)
        // {
        //     Image artworkImage = cellArtwork.artworkImage;
        //     Vector2 pixelSize = new Vector2(artworkImage.sprite.texture.width, artworkImage.sprite.texture.height);
        //     Vector2 pixelPivot = artworkImage.sprite.pivot;
        //     Vector2 uiPivot = new Vector2(pixelPivot.x / pixelSize.x, pixelPivot.y / pixelSize.y);
        //     
        //     artworkImage.rectTransform.pivot = uiPivot;
        //     
        //     cellArtwork.SetArtwork(artworkImage.sprite);
        // }
        //
        // protected void ZoomElement(CellArtwork cellArtwork, float zoom)
        // {
        //     cellArtwork.artworkImage.rectTransform.sizeDelta *= zoom;
        // }
        
        protected void Start()
        {
            SpawnElements();
        }
    }
}